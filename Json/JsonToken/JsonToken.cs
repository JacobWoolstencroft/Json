using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json
{
    public abstract class JsonToken
    {
        public abstract string ToJsonString();

        public static implicit operator JsonToken(int? value)
        {
            if (value == null)
                return new JsonNull();
            return new JsonInt(value.Value);
        }
        public static implicit operator JsonToken(long? value)
        {
            if (value == null)
                return new JsonNull();
            return new JsonLong(value.Value);
        }
        public static implicit operator JsonToken(decimal? value)
        {
            if (value == null)
                return new JsonNull();
            return new JsonDecimal(value.Value);
        }
        public static implicit operator JsonToken(string value)
        {
            return new JsonString(value);
        }
        public static implicit operator JsonToken(bool value)
        {
            return new JsonBool(value);
        }
        public static implicit operator JsonToken(Enum value)
        {
            if (value == null)
                return new JsonNull();
            return new JsonString(Enum.GetName(value.GetType(), value));
        }
        public static implicit operator JsonToken(List<string> list)
        {
            if (list == null)
                return new JsonNull();
            return new JsonArray(list);
        }

        #region Mapping getters
        public enum NullValueOptions
        {
            Empty,
            Null
        }

        public JsonToken GetToken(string key, JsonToken defaultValue = null)
        {
            if (TryGetToken(key, out JsonToken val))
                return val;
            return defaultValue;
        }
        public int? GetNullableInt(string key, int? defaultValue)
        {
            if (TryGetToken(key, out JsonToken token))
            {
                if (token.IsNull)
                    return null;
                if (token.TryGetInt(out int val))
                    return val;
            }
            return defaultValue;
        }
        public long? GetNullableLong(string key, long? defaultValue)
        {
            if (TryGetToken(key, out JsonToken token))
            {
                if (token.IsNull)
                    return null;
                if (token.TryGetLong(out long val))
                    return val;
            }
            return defaultValue;
        }
        public decimal? GetNullableDecimal(string key, decimal? defaultValue)
        {
            if (TryGetToken(key, out JsonToken token))
            {
                if (token.IsNull)
                    return null;
                if (token.TryGetDecimal(out decimal val))
                    return val;
            }
            return defaultValue;
        }

        public int GetInt(string key, int defaultValue)
        {
            if (TryGetToken(key, out JsonToken token))
            {
                if (token.IsNull)
                    return defaultValue;
                if (token.TryGetInt(out int val))
                    return val;
            }
            return defaultValue;
        }
        public long GetLong(string key, long defaultValue)
        {
            if (TryGetToken(key, out JsonToken token))
            {
                if (token.IsNull)
                    return defaultValue;
                if (token.TryGetLong(out long val))
                    return val;
            }
            return defaultValue;
        }

        public string GetString(string key, string defaultValue)
        {
            if (TryGetString(key, out string val))
                return val;
            return defaultValue;
        }
        public bool GetBool(string key, bool defaultValue)
        {
            if (TryGetBool(key, out bool val))
                return val;
            return defaultValue;
        }

        public List<string> GetStringList(string key, NullValueOptions ListNotFound = NullValueOptions.Null, bool ExcludeInvalidValues = false, string InvalidValue = null)
        {
            if (TryGetArray(key, out JsonArray array))
            {
                List<string> list = new List<string>();
                foreach (JsonToken token in array)
                {
                    if (token.TryGetString(out string value))
                        list.Add(value);
                    else
                    {
                        if (ExcludeInvalidValues)
                            continue;
                        list.Add(InvalidValue);
                    }
                }
                return list;
            }
            else
            {
                switch (ListNotFound)
                {
                    case NullValueOptions.Empty:
                        return new List<string>();
                    case NullValueOptions.Null:
                    default:
                        return null;
                }
            }
        }
        public T GetEnum<T>(string key, T defaultValue) where T : struct
        {
            if (TryGetToken(key, out JsonToken token))
            {
                if (token.TryGetString(out string str))
                {
                    if (Enum.TryParse<T>(str, out T val))
                        return val;
                }
            }
            return defaultValue;
        }
        public T? GetNullableEnum<T>(string key, T? defaultValue) where T : struct
        {
            if (TryGetToken(key, out JsonToken token))
            {
                if (token.TryGetString(out string str))
                {
                    if (Enum.TryParse<T>(str, out T val))
                        return val;
                }
            }
            return defaultValue;
        }
        #endregion

        #region Value Indexers
        public string GetString(int index, string defaultValue)
        {
            if (TryGetString(index, out string val))
                return val;
            return defaultValue;
        }

        public abstract bool TryGetInt(out int val);
        public abstract bool TryGetLong(out long val);
        public abstract bool TryGetDecimal(out decimal val);
        public abstract bool TryGetString(out string val);
        public abstract bool TryGetBool(out bool val);

        public abstract bool IsNull { get; }
        #endregion

        #region Mapping Indexers
        public abstract bool TryGetToken(string key, out JsonToken val);
        public abstract bool TryGetMapping(string key, out JsonMapping val);
        public abstract bool TryGetArray(string key, out JsonArray val);
        public abstract bool TryGetInt(string key, out int val);
        public abstract bool TryGetLong(string key, out long val);
        public abstract bool TryGetDecimal(string key, out decimal val);
        public abstract bool TryGetString(string key, out string val);
        public abstract bool TryGetBool(string key, out bool val);
        #endregion

        #region Array Indexers
        public abstract bool TryGetToken(int index, out JsonToken val);
        public abstract bool TryGetMapping(int index, out JsonMapping val);
        public abstract bool TryGetArray(int index, out JsonArray val);
        public abstract bool TryGetInt(int index, out int val);
        public abstract bool TryGetLong(int index, out long val);
        public abstract bool TryGetDecimal(int index, out decimal val);
        public abstract bool TryGetString(int index, out string val);
        public abstract bool TryGetBool(int index, out bool val);
        #endregion

        public static JsonToken Parse(string json)
        {
            int current_position = 0;
            return Parse(json.ToCharArray(), ref current_position);
        }
        private static JsonToken Parse(char[] chars, ref int current)
        {
            IgnoreWhitespace(chars, ref current);

            switch (chars[current])
            {
                case '{':
                    {
                        current++;
                        bool first = true;
                        JsonMapping mapping = new JsonMapping();

                        while (current < chars.Length)
                        {
                            IgnoreWhitespace(chars, ref current);
                            if (chars[current] == '}')
                            {
                                current++;
                                return mapping;
                            }

                            if (first)
                                first = false;
                            else
                            {
                                if (!NextCharIs(chars, ref current, ','))
                                    throw new Exception("JSON error: Expected mapping separator ','");
                                IgnoreWhitespace(chars, ref current);
                            }

                            string key = ParseString(chars, ref current);
                            IgnoreWhitespace(chars, ref current);
                            if (!NextCharIs(chars, ref current, ':'))
                                throw new Exception("JSON error: Expected mapping key/value separator ':'");
                            JsonToken value = Parse(chars, ref current);

                            mapping[key] = value;
                        }
                    }
                    break;
                case '[':
                    {
                        current++;
                        bool first = true;
                        JsonArray array = new JsonArray();

                        while (current < chars.Length)
                        {
                            IgnoreWhitespace(chars, ref current);
                            if (chars[current] == ']')
                            {
                                current++;
                                return array;
                            }

                            if (first)
                                first = false;
                            else
                            {
                                if (!NextCharIs(chars, ref current, ','))
                                    throw new Exception("JSON error: Expected array separator ','");
                                IgnoreWhitespace(chars, ref current);
                            }

                            JsonToken value = Parse(chars, ref current);
                            array.Add(value);
                        }
                    }
                    break;
                case '\"':
                    {
                        string str = ParseString(chars, ref current);
                        return new JsonString(str);
                    }
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    {
                        JsonNumeric number = ParseNumeric(chars, ref current);
                        return number;
                    }
                default:
                    if (NextStringIs(chars, ref current, "true", StringComparison.CurrentCultureIgnoreCase))
                        return new JsonBool(true);
                    if (NextStringIs(chars, ref current, "false", StringComparison.CurrentCultureIgnoreCase))
                        return new JsonBool(false);
                    if (NextStringIs(chars, ref current, "null", StringComparison.CurrentCultureIgnoreCase))
                        return new JsonNull();

                    throw new Exception("JSON error: Unrecognized JSON object character");
            }

            throw new Exception("JSON error: Unexpected end of string");
        }
        /// <summary>
        /// Returns a value indicating whether the current character is the required character, and advances to the next character
        /// </summary>
        /// <param name="chars">The character array which contains the string being parsed</param>
        /// <param name="current">The current index in the array</param>
        /// <param name="required_char">The required character</param>
        /// <returns>Boolean value indicating whether the current character is equal to the required character</returns>
        private static bool NextCharIs(char[] chars, ref int current, char required_char)
        {
            if (current >= chars.Length)
                throw new Exception("JSON error: Unexpected end of string");
            return chars[current++] == required_char;
        }
        /// <summary>
        /// Returns a value indicating whether the required string is at the current location in the array being parsed, and advances the current character only if it is found.
        /// </summary>
        /// <param name="chars">The character array which contains the string being parsed</param>
        /// <param name="current">The current index in the array</param>
        /// <param name="required_string">The required string</param>
        /// <returns>Boolean value indicating whether the required string was found</returns>
        private static bool NextStringIs(char[] chars, ref int current, string required_string, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            if (current + required_string.Length - 1 >= chars.Length)
                return false;

            string str = new string(chars, current, required_string.Length);

            if (str.Equals(required_string, comparisonType))
            {
                current += required_string.Length;
                return true;
            }
            else
                return false;
        }
        private static void IgnoreWhitespace(char[] chars, ref int current)
        {
            while (current < chars.Length)
            {
                switch (chars[current])
                {
                    case ' ':
                    case '\r':
                    case '\n':
                    case '\t':
                        current++;
                        continue;
                    default:
                        return;
                }
            }
        }
        private static string ParseString(char[] chars, ref int current)
        {
            IgnoreWhitespace(chars, ref current);
            if (!NextCharIs(chars, ref current, '\"'))
                throw new Exception("JSON parsing error: Expected start of string");

            StringBuilder r = new StringBuilder();
            bool escaped = false;

            while (current < chars.Length)
            {
                if (escaped)
                {
                    switch (chars[current])
                    {
                        case '\"':
                            r.Append('\"');
                            break;
                        case '\\':
                            r.Append('\\');
                            break;
                        case '/':
                            r.Append('/');
                            break;
                        case 'b':
                            r.Append('\b');
                            break;
                        case 'f':
                            r.Append('\f');
                            break;
                        case 'n':
                            r.Append('\n');
                            break;
                        case 'r':
                            r.Append('\r');
                            break;
                        case 't':
                            r.Append('\t');
                            break;
                        case 'u':
                            if (current + 4 >= chars.Length)
                                throw new Exception("JSON error: Unexpected end of string");

                            string hex = new string(chars, current + 1, 4);
                            if (int.TryParse(hex, System.Globalization.NumberStyles.AllowHexSpecifier, null, out int i) && i >= 0)
                                r.Append((char)i);
                            else
                                throw new Exception("JSON error: Invalid unicode character encoding in string");

                            current += 4;
                            break;

                        default:
                            throw new Exception("JSON error: Unrecognized escaped character in string");
                    }
                    escaped = false;
                }
                else
                {
                    switch (chars[current])
                    {
                        case '\\':
                            escaped = true;
                            break;
                        case '\"':
                            current++;
                            return r.ToString();
                        default:
                            r.Append(chars[current]);
                            break;
                    }
                }
                current++;
            }

            throw new Exception("JSON error: Unexpected end of string");
        }
        private static JsonNumeric ParseNumeric(char[] chars, ref int current)
        {
            int start = current;

            if (chars[current] == '-')
                current++;

            IgnoreDigits(chars, ref current);
            if (chars[current] == '.')
            {
                current++;
                IgnoreDigits(chars, ref current);
            }
            if (chars[current] == 'e' || chars[current] == 'E')
            {
                current++;
                if (chars[current] == '-' || chars[current] == '+')
                    current++;
                IgnoreDigits(chars, ref current);
            }

            string str = new string(chars, start, current - start);

            System.Globalization.NumberStyles intFormat = System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.AllowLeadingSign;
            System.Globalization.NumberStyles decFormat = System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowDecimalPoint;

            if (int.TryParse(str, intFormat, null, out int i))
                return new JsonInt(i);
            if (long.TryParse(str, intFormat, null, out long l))
                return new JsonLong(l);
            if (decimal.TryParse(str, decFormat, null, out decimal d))
                return new JsonDecimal(d);

            throw new Exception("JSON error: Unable to parse numeric");
        }
        private static void IgnoreDigits(char[] chars, ref int current)
        {
            while (current < chars.Length)
            {
                switch (chars[current])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        current++;
                        break;
                    default:
                        return;
                }
            }
        }

        public static string EncodeString(string str)
        {
            StringBuilder r = new StringBuilder();
            r.Append('\"');
            foreach (char c in str)
            {
                switch (c)
                {
                    case '\"':
                        r.Append("\\\"");
                        break;
                    case '\\':
                        r.Append("\\\\");
                        break;
                    case '/':
                        r.Append("\\/");
                        break;
                    case '\b':
                        r.Append("\\b");
                        break;
                    case '\f':
                        r.Append("\\f");
                        break;
                    case '\n':
                        r.Append("\\n");
                        break;
                    case '\r':
                        r.Append("\\r");
                        break;
                    case '\t':
                        r.Append("\\t");
                        break;

                    default:
                        if (c <= '\x1f')
                            r.Append("\\u" + ((int)c).ToString("X4"));
                        else
                            r.Append(c);
                        break;
                }
            }
            r.Append('\"');
            return r.ToString();
        }
    }
}
