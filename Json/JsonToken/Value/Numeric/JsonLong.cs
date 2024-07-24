namespace Json
{
    public class JsonLong : JsonNumeric
    {
        private long value = 0;

        public JsonLong()
        {
        }
        public JsonLong(long value)
        {
            this.value = value;
        }

        public override string Text => value.ToString();
        protected internal override string ToJsonString()
        {
            return value.ToString();
        }
        public long Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public override bool TryGetInt(out int val)
        {
            if (value < int.MinValue || value > int.MaxValue)
            {
                val = 0;
                return false;
            }
            val = (int)value;
            return true;
        }
        public override bool TryGetUInt(out uint val)
        {
            if (value < uint.MinValue || value > uint.MaxValue)
            {
                val = 0;
                return false;
            }
            val = (uint)value;
            return true;
        }
        public override bool TryGetLong(out long val)
        {
            val = value;
            return true;
        }
        public override bool TryGetDecimal(out decimal val)
        {
            val = value;
            return true;
        }
    }
}
