using System.Reflection;
using ArgoStore.Configurations;

namespace ArgoStore;

internal class PrimaryKeyValue
{
    private static readonly string _emptyGuidString = Guid.Empty.ToString();
    private static readonly Type[] _stringTypes = new[] {typeof(Guid), typeof(string)};
    private static readonly Type[] _intTypes = new[] {typeof(int), typeof(long)};

    public string StringKey { get; private set; }
    public long LongKey { get; private set; }
    public bool IsStringKey { get; }
    public PropertyInfo PkProperty { get; }
    public bool IsLongKey => !IsStringKey;
        
    public PrimaryKeyValue(string stringKey, long longKey, PropertyInfo pkProperty)
    {
        StringKey = stringKey;
        LongKey = longKey;
        PkProperty = pkProperty ?? throw new ArgumentNullException(nameof(pkProperty));
        IsStringKey = _stringTypes.Contains(pkProperty.PropertyType);

        if (!_stringTypes.Contains(pkProperty.PropertyType) && !_intTypes.Contains(pkProperty.PropertyType))
        {
            throw new ArgumentException($"Property type `{pkProperty.PropertyType}` not supported", nameof(pkProperty));
        }
    }

    public static PrimaryKeyValue CreateFromEntity(EntityMetadata meta, object entity)
    {
        if (meta == null) throw new ArgumentNullException(nameof(meta));
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        // todo: remove reflection to improve performance
        object pk = meta.PrimaryKeyProperty.GetValue(entity);

        if (_stringTypes.Contains(meta.PrimaryKeyProperty.PropertyType))
        {
            string stringPk = pk?.ToString();
            return new PrimaryKeyValue(stringPk, 0, meta.PrimaryKeyProperty);
        }

        if (_intTypes.Contains(meta.PrimaryKeyProperty.PropertyType))
        {
            long intPk;

            if (meta.PrimaryKeyProperty.PropertyType == typeof(int))
            {
                intPk = (int) pk;
            }
            else
            {
                intPk = (long) pk;
            }

            return new PrimaryKeyValue(null, intPk, meta.PrimaryKeyProperty);
        }

        throw new InvalidOperationException($"Unsupported PK type `{meta.PrimaryKeyProperty.PropertyType}`");
    }

    public object GetValue()
    {
        if (IsStringKey)
        {
            if (PkProperty.PropertyType == typeof(string))
            {
                return StringKey;
            }

            if (string.IsNullOrWhiteSpace(StringKey))
            {
                return Guid.Empty;
            }

            return Guid.Parse(StringKey);
        }
        else
        {
            if (PkProperty.PropertyType == typeof(long))
            {
                return LongKey;
            }

            return (int)LongKey;
        }
    }

    public void SetRandomStringKey()
    {
        if (IsLongKey) throw new InvalidOperationException("Cannot set string key in long key");

        StringKey = Guid.NewGuid().ToString();
    }

    public void SetLongKey(long key)
    {
        LongKey = key;
    }

    public bool HasDefaultValue()
    {
        if (IsLongKey)
        {
            return LongKey == 0;
        }

        return string.IsNullOrWhiteSpace(StringKey) || StringKey == _emptyGuidString;
    }

    public void SetInEntity(object entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        object value = GetValue();
        PkProperty.SetValue(entity, value);
    }

}