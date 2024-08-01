using System.Text;

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
        protected internal override void BuildJsonString(StringBuilder builder, bool indent, int indentLevel)
        {
            builder.Append(value.ToString());
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
        public override bool TryGetUInt(out uint val)
        {
            try
            {
                val = (uint)value;
                return true;
            }
            catch
            {
                val = 0;
                return false;
            }
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
