namespace Json
{
    public abstract class JsonNumeric : JsonValue
    {
        public override bool IsNull => false;
        public override bool TryGetString(out string val)
        {
            val = ToJsonString();
            return true;
        }
        public override bool TryGetBool(out bool val)
        {
            val = false;
            return false;
        }
    }
}
