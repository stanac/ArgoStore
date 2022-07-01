using System.Globalization;

namespace ArgoStore;

public static class ArgoStoreConvert
{
    private static readonly ArgoStoreSerializer _serializer = new();

    public static object To(Type toType, object value)
    {
        if (toType is null) throw new ArgumentNullException(nameof(toType));

        if (value is DBNull) value = null;

        bool isTypeNullable = toType == typeof(string) || (toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>));

        if (value == null)
        {
            if (isTypeNullable)
            {
                return null;
            }

            throw new ArgumentException($"Cannot convert null to non nullable type {toType.FullName}");
        }

        // todo: add other types

        if (toType == typeof(string)) return ToString(value);
        if (toType == typeof(int)) return ToInt(value);
        if (toType == typeof(DateTime)) return ToDateTime(value);
        if (toType == typeof(DateTime?)) return ToDateTimeNullable(value);
        if (toType == typeof(DateTimeOffset)) return ToDateTimeOffset(value);
        if (toType == typeof(DateTimeOffset?)) return ToDateTimeOffsetNullable(value);


        if (value is string json)
        {
            return _serializer.Deserialize(json, toType);
        }

        throw new NotImplementedException();
    }


    public static string ToString(object value)
    {
        if (value == null) return null;

        if (value is string s) return s;

        return value.ToString();
    }

    public static int ToInt(object value)
    {
        if (value == null) throw new ArgumentException($"Cannot convert NULL to int");

        if (value is int) return (int)value;
        if (value is long) return (int)(long)value;

        if (value is string) return int.Parse(value as string, NumberStyles.Integer, CultureInfo.InvariantCulture);

        throw new ArgumentException($"Cannot convert type {value.GetType().Name} to int");
    }

    public static int? ToIntNullable(object value)
    {
        if (value == null || value is DBNull) return null;

        if (value is int) return (int)value;
        if (value is long) return (int)(long)value;

        if (value is string) return int.Parse(value as string, NumberStyles.Integer, CultureInfo.InvariantCulture);

        throw new ArgumentException($"Cannot convert type {value.GetType().Name} to Nullable<int>");
    }

    public static DateTime ToDateTime(object value)
    {
        if (value == null || value is DBNull) throw new ArgumentException($"Cannot convert NULL to DateTime");

        if (value is string)
        {
            string s = value as string;

            return DateTime.Parse(s, CultureInfo.InvariantCulture);
        }

        throw new ArgumentException($"Cannot convert type {value.GetType().Name} to DateTime");
    }

    public static DateTimeOffset ToDateTimeOffset(object value)
    {
        if (value == null || value is DBNull) throw new ArgumentException($"Cannot convert NULL to DateTimeOffset");

        if (value is string)
        {
            string s = value as string;

            return DateTime.Parse(s, CultureInfo.InvariantCulture);
        }

        throw new ArgumentException($"Cannot convert type {value.GetType().Name} to DateTimeOffset");
    }

    public static DateTime? ToDateTimeNullable(object value)
    {
        if (value == null || value is DBNull) return null;

        if (value is string)
        {
            string s = value as string;

            return DateTime.Parse(s, CultureInfo.InvariantCulture);
        }

        throw new ArgumentException($"Cannot convert type {value.GetType().Name} to Nullable<DateTime>");
    }

    public static DateTimeOffset? ToDateTimeOffsetNullable(object value)
    {
        if (value == null || value is DBNull) return null;


        throw new ArgumentException($"Cannot convert type {value.GetType().Name} to Nullable<DateTimeOffset>");
    }
}