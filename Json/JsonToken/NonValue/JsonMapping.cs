using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Json
{
    public class JsonMapping : JsonNonValue, IEnumerable<KeyValuePair<string, JsonToken>>
    {
        private Dictionary<string, JsonToken> Children = new Dictionary<string, JsonToken>();

        protected internal override void BuildJsonString(StringBuilder r, bool indent, int indentLevel)
        {
            bool first = true;
            r.Append('{');
            if (Children.Count > 0)
            {
                foreach (KeyValuePair<string, JsonToken> token in Children)
                {
                    if (first)
                        first = false;
                    else
                        r.Append(',');

                    if (indent)
                    {
                        r.AppendLine();
                        r.Append('\t', indentLevel + 1);
                    }
                    EncodeString(r, token.Key);
                    r.Append(':');
                    token.Value.BuildJsonString(r, indent, indentLevel + 1);
                }
                if (indent)
                {
                    r.AppendLine();
                    if (indentLevel > 0)
                        r.Append('\t', indentLevel);
                }
            }
            r.Append('}');
        }

        public JsonToken this[string key]
        {
            get
            {
                return Children[key];
            }
            set
            {
                Children[key] = value;
            }
        }
        public bool Remove(string key)
        {
            return Children.Remove(key);
        }

        #region Array indexers
        public override bool TryGetToken(int index, out JsonToken val)
        {
            val = null;
            return false;
        }
        public override bool TryGetMapping(int index, out JsonMapping val)
        {
            val = null;
            return false;
        }
        public override bool TryGetArray(int index, out JsonArray val)
        {
            val = null;
            return false;
        }
        public override bool TryGetInt(int index, out int val)
        {
            val = 0;
            return false;
        }
        public override bool TryGetUInt(int index, out uint val)
        {
            val = 0;
            return false;
        }
        public override bool TryGetLong(int index, out long val)
        {
            val = 0;
            return false;
        }
        public override bool TryGetDecimal(int index, out decimal val)
        {
            val = 0;
            return false;
        }
        public override bool TryGetString(int index, out string val)
        {
            val = null;
            return false;
        }
        public override bool TryGetBool(int index, out bool val)
        {
            val = false;
            return false;
        }
        #endregion
        #region Mapping indexers
        public override bool TryGetToken(string key, out JsonToken val)
        {
            if (Children.TryGetValue(key, out val) && val != null)
                return true;
            val = null;
            return false;
        }
        public override bool TryGetMapping(string key, out JsonMapping val)
        {
            if (TryGetToken(key, out JsonToken token) && token is JsonMapping map)
            {
                val = map;
                return true;
            }
            val = null;
            return false;
        }
        public override bool TryGetArray(string key, out JsonArray val)
        {
            if (TryGetToken(key, out JsonToken token) && token is JsonArray array)
            {
                val = array;
                return true;
            }
            val = null;
            return false;
        }
        public override bool TryGetInt(string key, out int val)
        {
            if (TryGetToken(key, out JsonToken token))
                return token.TryGetInt(out val);
            val = 0;
            return false;
        }
        public override bool TryGetUInt(string key, out uint val)
        {
            if (TryGetToken(key, out JsonToken token))
                return token.TryGetUInt(out val);
            val = 0;
            return false;
        }
        public override bool TryGetLong(string key, out long val)
        {
            if (TryGetToken(key, out JsonToken token))
                return token.TryGetLong(out val);
            val = 0;
            return false;
        }
        public override bool TryGetDecimal(string key, out decimal val)
        {
            if (TryGetToken(key, out JsonToken token))
                return token.TryGetDecimal(out val);
            val = 0;
            return false;
        }
        public override bool TryGetString(string key, out string val)
        {
            if (TryGetToken(key, out JsonToken token))
                return token.TryGetString(out val);
            val = null;
            return false;
        }
        public override bool TryGetBool(string key, out bool val)
        {
            if (TryGetToken(key, out JsonToken token))
                return token.TryGetBool(out val);
            val = false;
            return false;
        }
        #endregion

        public IEnumerator<KeyValuePair<string, JsonToken>> GetEnumerator()
        {
            return Children.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }
}
