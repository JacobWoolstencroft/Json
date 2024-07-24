namespace Json
{
    public abstract class JsonValue : JsonToken
    {
        public abstract string Text { get; }

        protected internal override string ToJsonString(bool indent, int indentLevel)
        {
            return ToJsonString();
        }
        protected internal abstract string ToJsonString();

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
        public override bool TryGetUInt(string key, out uint val)
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
    }
}
