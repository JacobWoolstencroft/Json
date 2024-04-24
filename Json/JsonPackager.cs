using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Json
{
    public class JsonPackager
    {
        public enum JsonPackagerErrors
        {
            [FriendlyName("Parse error: missing type")] ParseErrorMissingType,
            [FriendlyName("Parse error: missing value")] ParseErrorMissingValue,
            [FriendlyName("Parse error: unrecognized type \"$TypeName\"")] ParseErrorUnrecognizedType,
            [FriendlyName("Parse error: invalid object type, cannot cast $CastFrom to $CastTo")] ParseErrorInvalidCast,
            [FriendlyName("Parse error: value is not a list")] ParseErrorNotAList,

            [FriendlyName("Encode error: Invalid type \"$TypeName\"")] EncodeErrorInvalidType
        }
        public class JsonPackagerException : Exception
        {
            private JsonPackagerErrors error;
            private string message;
            public JsonPackagerException(JsonPackagerErrors error)
            {
                this.error = error;

                message = "Json Packager Error - " + error.FriendlyName();
            }
            public JsonPackagerException(JsonPackagerErrors error, string TypeName)
            {
                this.error = error;

                message = "Json Packager Error - " + error.FriendlyName();
                message = message.Replace("$TypeName", TypeName);
            }
            public JsonPackagerException(JsonPackagerErrors error, Type castFrom, Type castTo)
            {
                this.error = error;

                message = "Json Packager Error - " + error.FriendlyName();
                message = message.Replace("$CastFrom", (castFrom == null ? "null" : castFrom.Name));
                message = message.Replace("$CastTo", (castTo == null ? "null" : castTo.Name));
            }
            public override string Message
            {
                get
                {
                    return message;
                }
            }
        }

        public JsonPackager(params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
                throw new ArgumentNullException();
            Packages = new Dictionary<string, Type>();
            Type[] DefaultConstructor = new Type[0];
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IJsonPackable).IsAssignableFrom(t) && t.GetConstructor(DefaultConstructor) != null))
                {
                    Packages.Add(type.Name, type);
                }
            }
        }

        public JsonToken Package(object ob)
        {
            if (ob == null)
                return new JsonNull();

            Type type = ob.GetType();

            if (ob is IJsonPackable packable)
            {
                JsonMapping map = new JsonMapping();
                map["T"] = GetPackageName(packable.GetType());
                map["V"] = packable.Pack();
                return map;
            }
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>) && ob is IList list)
                {
                    JsonMapping map = new JsonMapping();
                    JsonArray array = new JsonArray();

                    map["T"] = "List<>";

                    foreach (object val in list)
                    {
                        array.Add(Package(val));
                    }
                    map["V"] = array;
                    return map;
                }
                if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>) && ob is IDictionary dict)
                {
                    Type dictType = ob.GetType();
                    Type keyType = dictType.GenericTypeArguments[0];

                    if (keyType != typeof(string))
                        throw new JsonPackagerException(JsonPackagerErrors.EncodeErrorInvalidType, ob.GetType(), typeof(string));

                    JsonMapping map = new JsonMapping();
                    JsonMapping dictionary = new JsonMapping();

                    map["T"] = "Dictionary<>";

                    foreach (object key in dict.Keys)
                    {
                        dictionary[key.ToString()] = Package(dict[key]);
                    }
                    map["V"] = dictionary;
                    return map;
                }
            }

            throw new JsonPackagerException(JsonPackagerErrors.EncodeErrorInvalidType, ob.GetType().Name);
        }

        public object Unpackage(string json)
        {
            return Unpackage(JsonToken.Parse(json));
        }
        public object Unpackage(JsonToken token)
        {
            if (token == null || token.IsNull)
                return null;

            string PackageName = token.GetString("T", null);
            if (PackageName == null)
                throw new JsonPackagerException(JsonPackagerErrors.ParseErrorMissingType);
            JsonToken val = token.GetToken("V");
            if (val == null)
                throw new JsonPackagerException(JsonPackagerErrors.ParseErrorMissingValue);

            if (PackageName == "List<>")
            {
                if (val is JsonArray array)
                {
                    List<object> list = new List<object>();
                    foreach (JsonToken t in array)
                    {
                        object ob = Unpackage(t);
                        list.Add(ob);
                    }
                    return list;
                }
                else
                    throw new Exception("Json Packager error: value is not an array");
            }
            if (PackageName == "Dictionary<>")
            {
                if (val is JsonMapping dictionary)
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (KeyValuePair<string, JsonToken> t in dictionary)
                    {
                        object ob = Unpackage(t.Value);
                        dict.Add(t.Key, ob);
                    }
                    return dict;
                }
                else
                    throw new Exception("Json Packager error: value is not a mapping");
            }

            {
                Type type = GetPackageType(PackageName);
                if (type == null || !typeof(IJsonPackable).IsAssignableFrom(type))
                    throw new JsonPackagerException(JsonPackagerErrors.ParseErrorUnrecognizedType, PackageName);

                IJsonPackable ob = (IJsonPackable)Activator.CreateInstance(type);
                ob.Unpack(val);
                return ob;
            }
        }
        public T Unpackage<T>(string json) where T : class
        {
            return Unpackage<T>(JsonToken.Parse(json));
        }
        public T Unpackage<T>(JsonToken token) where T : class
        {
            object ob = Unpackage(token);
            if (ob == null)
                return null;

            return (T)Cast(ob, typeof(T));
        }
        private object Cast(object ob, Type type)
        {
            if (ob == null)
            {
                if (type.IsClass)
                    return null;
                else
                    throw new JsonPackagerException(JsonPackagerErrors.ParseErrorInvalidCast, null, type);
            }

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>) && ob is IList obList)
                {
                    Type elementType = type.GenericTypeArguments[0];
                    IList list = (IList)Activator.CreateInstance(type);
                    foreach (object val in obList)
                    {
                        list.Add(Cast(val, elementType));
                    }
                    return list;
                }
                if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>) && ob is IDictionary obDict)
                {
                    Type keyType = type.GenericTypeArguments[0];
                    Type valType = type.GenericTypeArguments[1];
                    IDictionary dict = (IDictionary)Activator.CreateInstance(type);
                    foreach (object key in obDict.Keys)
                    {
                        object val = obDict[key];
                        dict.Add(Cast(key, keyType), Cast(val, valType));
                    }
                    return dict;
                }
            }

            if (type.IsAssignableFrom(ob.GetType()))
                return ob;
            throw new JsonPackagerException(JsonPackagerErrors.ParseErrorInvalidCast, ob.GetType(), type);
        }

        private Dictionary<string, Type> Packages = null; //package name to type
        public string GetPackageName(Type type)
        {
            JsonPackableAttribute Package = GetCustomAttribute<JsonPackableAttribute>(type);
            if (Package != null && Package.Name != null)
                return Package.Name;
            return type.Name;
        }
        public Type GetPackageType(string name)
        {
            if (Packages.TryGetValue(name, out Type t))
                return t;
            return null;
        }
        public T GetCustomAttribute<T>(Type type) where T : Attribute
        {
            object[] obs = type.GetCustomAttributes(typeof(T), false);
            if (obs != null && obs.Length == 1 && obs[0] is T attribute)
                return attribute;
            return null;
        }
    }
}
