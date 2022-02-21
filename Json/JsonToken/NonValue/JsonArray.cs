using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Json
{
    public class JsonArray : JsonNonValue, IEnumerable<JsonToken>
    {
        private List<JsonToken> Children = new List<JsonToken>();

        public JsonArray()
        {
        }
        public JsonArray(List<JsonToken> list)
        {
            Children.AddRange(list);
        }
        public JsonArray(List<string> list)
        {
            foreach (string val in list)
                Children.Add(new JsonString(val));
        }

        public override string ToJsonString()
        {
            bool first = true;
            StringBuilder r = new StringBuilder();
            r.Append('[');
            foreach (JsonToken token in Children)
            {
                if (first)
                    first = false;
                else
                    r.Append(',');
                r.Append(token.ToJsonString());
            }
            r.Append(']');
            return r.ToString();
        }
        public JsonToken this[int index]
        {
            get
            {
                if (index < 0 || index >= Children.Count)
                    throw new IndexOutOfRangeException();
                return Children[index];
            }
            set
            {
                if (index < 0 || index >= Children.Count)
                    throw new IndexOutOfRangeException();
                Children[index] = value;
            }
        }
        public int Count
        {
            get
            {
                return Children.Count;
            }
        }
        public void RemoveAt(int index)
        {
            Children.RemoveAt(index);
        }
        public void Add(JsonToken token)
        {
            Children.Add(token);
        }
        public void AddRange(List<JsonToken> tokens)
        {
            Children.AddRange(tokens);
        }

        #region Array indexers
        public override bool TryGetToken(int index, out JsonToken val)
        {
            if (index >= 0 && index < Children.Count)
            {
                val = Children[index];
                if (val != null)
                    return true;
            }
            val = null;
            return false;
        }
        public override bool TryGetMapping(int index, out JsonMapping val)
        {
            if (TryGetToken(index, out JsonToken token) && token is JsonMapping map)
            {
                val = map;
                return true;
            }
            val = null;
            return false;
        }
        public override bool TryGetArray(int index, out JsonArray val)
        {
            if (TryGetToken(index, out JsonToken token) && token is JsonArray array)
            {
                val = array;
                return true;
            }
            val = null;
            return false;
        }
        public override bool TryGetInt(int index, out int val)
        {
            if (TryGetToken(index, out JsonToken token))
                return token.TryGetInt(out val);
            val = 0;
            return false;
        }
        public override bool TryGetLong(int index, out long val)
        {
            if (TryGetToken(index, out JsonToken token))
                return token.TryGetLong(out val);
            val = 0;
            return false;
        }
        public override bool TryGetDecimal(int index, out decimal val)
        {
            if (TryGetToken(index, out JsonToken token))
                return token.TryGetDecimal(out val);
            val = 0;
            return false;
        }
        public override bool TryGetString(int index, out string val)
        {
            if (TryGetToken(index, out JsonToken token))
                return token.TryGetString(out val);
            val = null;
            return false;
        }
        public override bool TryGetBool(int index, out bool val)
        {
            if (TryGetToken(index, out JsonToken token))
                return token.TryGetBool(out val);
            val = false;
            return false;
        }
        #endregion
        #region Mapping indexers
        public override bool TryGetToken(string key, out JsonToken val)
        {
            val = null;
            return false;
        }
        public override bool TryGetMapping(string key, out JsonMapping val)
        {
            val = null;
            return false;
        }
        public override bool TryGetArray(string key, out JsonArray val)
        {
            val = null;
            return false;
        }
        public override bool TryGetInt(string key, out int val)
        {
            val = 0;
            return false;
        }
        public override bool TryGetLong(string key, out long val)
        {
            val = 0;
            return false;
        }
        public override bool TryGetDecimal(string key, out decimal val)
        {
            val = 0;
            return false;
        }
        public override bool TryGetString(string key, out string val)
        {
            val = null;
            return false;
        }
        public override bool TryGetBool(string key, out bool val)
        {
            val = false;
            return false;
        }
        #endregion

        public IEnumerator<JsonToken> GetEnumerator()
        {
            return Children.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }
}
