namespace Json
{
    public abstract class JsonNonValue : JsonToken
    {
        public override bool TryGetInt(out int val)
        {
            val = 0;
            return false;
        }
        public override bool TryGetUInt(out uint val)
        {
            val = 0;
            return false;
        }
        public override bool TryGetLong(out long val)
        {
            val = 0;
            return false;
        }
        public override bool TryGetDecimal(out decimal val)
        {
            val = 0;
            return false;
        }
        public override bool TryGetString(out string val)
        {
            val = null;
            return false;
        }
        public override bool TryGetBool(out bool val)
        {
            val = false;
            return false;
        }

        public override bool IsNull => false;
    }
}
