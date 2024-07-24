namespace Json
{
    public class JsonDecimal : JsonNumeric
    {
        private decimal value = 0;

        public JsonDecimal()
        {
        }
        public JsonDecimal(decimal value)
        {
            this.value = value;
        }

        public override string Text => value.ToString();
        protected internal override string ToJsonString()
        {
            return value.ToString();
        }
        public decimal Value
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
            if (decimal.Truncate(value) != value || value < int.MinValue || value > int.MaxValue)
            {
                val = 0;
                return false;
            }
            val = (int)value;
            return true;
        }
        public override bool TryGetUInt(out uint val)
        {
            if (decimal.Truncate(value) != value || value < uint.MinValue || value > uint.MaxValue)
            {
                val = 0;
                return false;
            }
            val = (uint)value;
            return true;
        }
        public override bool TryGetLong(out long val)
        {
            if (decimal.Truncate(value) != value || value < long.MinValue || value > long.MaxValue)
            {
                val = 0;
                return false;
            }
            val = (long)value;
            return true;
        }
        public override bool TryGetDecimal(out decimal val)
        {
            val = value;
            return true;
        }
    }
}
