using System;
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
            [FriendlyName("Parse error: value is not a list")] ParseErrorNotAList
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
        public JsonToken Package(IJsonPackable ob)
        {
            if (ob == null)
                return new JsonNull();
            JsonMapping map = new JsonMapping();
            map["T"] = GetPackageName(ob.GetType());
            map["V"] = ob.Pack();
            return map;
        }
        public JsonToken Package<T>(List<T> list) where T : IJsonPackable
        {
            JsonMapping map = new JsonMapping();
            JsonArray array;
            if (typeof(T).IsAbstract)
            {
                map["T"] = "List<>";

                if (list == null)
                    map["V"] = new JsonNull();
                else
                {
                    array = new JsonArray();
                    foreach (T ob in list)
                    {
                        array.Add(Package(ob));
                    }
                    map["V"] = array;
                }
                return map;
            }
            else
            {
                map["T"] = "List<" + GetPackageName(typeof(T)) + ">";

                if (list == null)
                    map["V"] = new JsonNull();
                else
                {
                    array = new JsonArray();
                    foreach (T ob in list)
                    {
                        if (ob == null)
                            array.Add(new JsonNull());
                        else
                            array.Add(ob.Pack());
                    }
                    map["V"] = array;
                }
                return map;
            }
        }
        public IJsonPackable Unpackage(string json)
        {
            return Unpackage(JsonToken.Parse(json));
        }
        public IJsonPackable Unpackage(JsonToken token)
        {
            if (token == null || token.IsNull)
                return null;

            string PackageName = token.GetString("T", null);
            if (PackageName == null)
                throw new JsonPackagerException(JsonPackagerErrors.ParseErrorMissingType);
            JsonToken val = token.GetToken("V");
            if (val == null)
                throw new JsonPackagerException(JsonPackagerErrors.ParseErrorMissingValue);

            Type type = GetPackageType(PackageName);
            if (type == null || !typeof(IJsonPackable).IsAssignableFrom(type))
                throw new JsonPackagerException(JsonPackagerErrors.ParseErrorUnrecognizedType, PackageName);

            IJsonPackable ob = (IJsonPackable)Activator.CreateInstance(type);
            ob.Unpack(val);
            return ob;
        }
        public T Unpackage<T>(string json) where T : class, IJsonPackable
        {
            return Unpackage<T>(JsonToken.Parse(json));
        }
        public T Unpackage<T>(JsonToken token) where T : class, IJsonPackable
        {
            IJsonPackable ob = Unpackage(token);
            if (ob == null)
                return null;
            if (typeof(T).IsAssignableFrom(ob.GetType()))
                return (T)ob;
            throw new JsonPackagerException(JsonPackagerErrors.ParseErrorInvalidCast, ob.GetType(), typeof(T) );
        }
        public List<T> UnpackageList<T>(string json) where T : class, IJsonPackable
        {
            return UnpackageList<T>(JsonToken.Parse(json));
        }
        public List<T> UnpackageList<T>(JsonToken token) where T : class, IJsonPackable
        {
            if (token == null || token.IsNull)
                return null;

            string PackageName = token.GetString("T", null);
            if (PackageName == null)
                throw new JsonPackagerException(JsonPackagerErrors.ParseErrorMissingType);

            JsonToken val = token.GetToken("V");
            if (val == null)
                throw new JsonPackagerException(JsonPackagerErrors.ParseErrorMissingValue);
            if (val.IsNull)
                return null;

            if (val is JsonArray array)
            {
                if (PackageName == "List<>")
                {
                    List<T> list = new List<T>();
                    foreach (JsonToken t in array)
                    {
                        IJsonPackable ob = Unpackage(t);
                        if (ob == null)
                            list.Add(null);
                        else if (typeof(T).IsAssignableFrom(ob.GetType()))
                            list.Add((T)ob);
                        else
                            throw new JsonPackagerException(JsonPackagerErrors.ParseErrorInvalidCast, ob.GetType(), typeof(T));
                    }
                    return list;
                }
                else if (PackageName.StartsWith("List<") && PackageName.EndsWith(">"))
                {
                    string ListTypeName = PackageName.SafeSubstring(5, PackageName.Length - 6);
                    Type ListType = GetPackageType(ListTypeName);
                    if (ListType == null)
                        throw new JsonPackagerException(JsonPackagerErrors.ParseErrorUnrecognizedType, ListTypeName);
                    if (typeof(T).IsAssignableFrom(ListType))
                    {
                        List<T> list = new List<T>();
                        foreach (JsonToken t in array)
                        {
                            T ob = (T)Activator.CreateInstance(typeof(T));
                            ob.Unpack(t);
                            list.Add(ob);
                        }
                        return list;
                    }
                    else
                        throw new JsonPackagerException(JsonPackagerErrors.ParseErrorInvalidCast, ListType, typeof(T));
                }
                else
                    throw new JsonPackagerException(JsonPackagerErrors.ParseErrorNotAList);
            }
            else
                throw new Exception("Json Packager error: value is not an array");
        }

        private Dictionary<string, Type> Packages = null; //package name to type
        public string GetPackageName(Type type)
        {
            JsonPackageAttribute Package = GetCustomAttribute<JsonPackageAttribute>(type);
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
