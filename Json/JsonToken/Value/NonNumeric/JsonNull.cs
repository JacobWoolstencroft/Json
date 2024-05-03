namespace Json
{
    public class JsonNull : JsonNonNumeric
    {
        public override string Text => null;
        public override bool IsNull => true;
        protected internal override string ToJsonString()
        {
            return "null";
        }
        public override bool TryGetString(out string val)
        {
            val = null;
            return true;
        }
        public override bool TryGetBool(out bool val)
        {
            val = false;
            return false;
        }
    }
}
