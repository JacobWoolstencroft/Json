using System;
using System.Text;

namespace Json
{
    public class JsonString : JsonValue
    {
        private string str = "";

        public JsonString()
        {
        }
        public JsonString(string value)
        {
            str = value;
        }
        public JsonString(Enum value)
        {
            if (value == null)
                str = null;
            else
                str = Enum.GetName(value.GetType(), value);
        }

        public override string Text => str;
        public override bool IsNull => str == null;
        protected internal override void BuildJsonString(StringBuilder builder, bool indent, int indentLevel)
        {
            if (str == null)
                builder.Append("null");
            else
                EncodeString(builder, str);
        }
        public string Value
        {
            get
            {
                return str;
            }
            set
            {
                str = value;
            }
        }

        public override bool TryGetInt(out int val)
        {
            return int.TryParse(str, out val);
        }
        public override bool TryGetUInt(out uint val)
        {
            return uint.TryParse(str, out val);
        }
        public override bool TryGetLong(out long val)
        {
            return long.TryParse(str, out val);
        }
        public override bool TryGetDecimal(out decimal val)
        {
            return decimal.TryParse(str, out val);
        }
        public override bool TryGetString(out string val)
        {
            val = str;
            return true;
        }
        public override bool TryGetBool(out bool val)
        {
            return bool.TryParse(str, out val);
        }
    }
}
