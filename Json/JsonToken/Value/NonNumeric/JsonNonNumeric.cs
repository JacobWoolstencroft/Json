namespace Json
{
    public abstract class JsonNonNumeric : JsonValue
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
    }
}
