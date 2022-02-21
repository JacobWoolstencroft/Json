namespace Json
{
    public class JsonInt : JsonNumeric
    {
        private int value = 0;

        public JsonInt()
        {
        }
        public JsonInt(int value)
        {
            this.value = value;
        }

        public override string Text => value.ToString();
        public override string ToJsonString()
        {
            return value.ToString();
        }
        public int Value
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
            val = value;
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
