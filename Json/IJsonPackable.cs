using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json
{
    public interface IJsonPackable
    {
        JsonToken Pack();
        void Unpack(JsonToken json);
    }
    public static class IJsonPackableExtensions
    {
        public static string ToJsonString(this IJsonPackable ob)
        {
            return ob.Pack().ToJsonString();
        }
        public static T ParseJson<T>(this string json) where T : class, IJsonPackable, new()
        {
            JsonToken token = JsonToken.Parse(json);

            T ob = new T();
            ob.Unpack(token);

            return ob;
        }
    }
}
