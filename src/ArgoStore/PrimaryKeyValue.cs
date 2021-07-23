using System;

namespace ArgoStore
{
    internal class PrimaryKeyValue
    {
        public string StringKey { get; }
        public long LongKey { get; }
        public bool IsStringKey { get; }
        public bool IsLongKey => !IsStringKey;

        public PrimaryKeyValue(string stringKey, long longKey, bool isStringKey)
        {
            StringKey = stringKey;
            LongKey = longKey;
            IsStringKey = isStringKey;
        }

        public bool HasDefaultValue()
        {
            if (IsLongKey)
            {
                return LongKey == 0;
            }

            return string.IsNullOrWhiteSpace(StringKey) || StringKey == Guid.Empty.ToString();
        }
    }
}
