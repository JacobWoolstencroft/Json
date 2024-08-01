using System.Text;

namespace Json
{
    public class JsonBool : JsonNonNumeric
    {
        private bool value = false;

        public JsonBool()
        {
        }
        public JsonBool(bool value)
        {
            this.value = value;
        }

        public override string Text => (value ? "true" : "false");
        public override bool IsNull => false;
        protected internal override void BuildJsonString(StringBuilder builder, bool indent, int indentLevel)
        {
            builder.Append(value ? "true" : "false");
        }
        public bool Value
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

        public override bool TryGetString(out string val)
        {
            val = Text;
            return true;
        }
        public override bool TryGetBool(out bool val)
        {
            val = value;
            return true;
        }
    }
}
