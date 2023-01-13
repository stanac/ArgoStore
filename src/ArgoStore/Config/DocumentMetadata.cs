using System.Reflection;

namespace ArgoStore.Config;

internal class DocumentMetadata
{
    private static readonly Type[] _allowedKeyTypes =
    {
        typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(string), typeof(Guid)
    };

    private static readonly Type[] _intTypes =
    {
        typeof(int), typeof(uint), typeof(long), typeof(ulong)
    };

    private const string AllowedPrimaryKeyTypeNames = "`int32`, `uint32`, `int64`, `uint64`, `string`, `Guid`";

    private readonly PropertyInfo _keyProperty;

    public Type DocumentType { get; }
    public string DocumentName { get; }
    public List<DocumentIndexMetadata> Indexes { get; }
    public bool IsKeyPropertyInt { get; }
    public bool IsKeyPropertyString { get; }
    public bool IsKeyPropertyGuid { get; }
    public Type KeyPropertyType => _keyProperty.PropertyType;
    public string KeyPropertyName => _keyProperty.Name;
    
    public DocumentMetadata(Type documentType, string pkPropertyName, string documentTableName, List<DocumentIndexMetadata> indexes)
    {
        if (string.IsNullOrWhiteSpace(pkPropertyName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(pkPropertyName));
        if (string.IsNullOrWhiteSpace(documentTableName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(documentTableName));

        DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
        DocumentName = documentTableName;
        Indexes = indexes ?? throw new ArgumentNullException(nameof(indexes));
        _keyProperty = documentType.GetProperty(pkPropertyName, BindingFlags.Public | BindingFlags.Instance)
                       ?? throw new InvalidOperationException($"Failed to find identity property `{pkPropertyName}` on type `{documentType.FullName}`.");

        EnsureTypeIsValid(documentType, documentTableName);

        IsKeyPropertyInt = _intTypes.Contains(_keyProperty.PropertyType);

        if (!IsKeyPropertyInt)
        {
            IsKeyPropertyGuid = _keyProperty.PropertyType == typeof(Guid);
            IsKeyPropertyString = !IsKeyPropertyGuid;
        }
    }
    
    public object GetPrimaryKeyValue(object doc, out bool isDefaultValue)
    {
        if (doc == null) throw new ArgumentNullException(nameof(doc));

#if DEBUG
        if (doc.GetType() != DocumentType)
        {
            throw new InvalidOperationException(
                $"Document of type `{doc.GetType().FullName}` not expected. Expected `{DocumentType.FullName}`"
            );
        }
#endif

        object key = _keyProperty.GetValue(doc);

        if (IsKeyPropertyGuid)
        {
            isDefaultValue = (Guid)key == default;
            return key;
        }

        if (IsKeyPropertyString)
        {
            isDefaultValue = key == null;
            return key;
        }

#if DEBUG
        if (!IsKeyPropertyInt)
        {
            throw new InvalidOperationException("Expected key property to be integer");
        }
#endif

        if (_keyProperty.PropertyType == typeof(int))
        {
            isDefaultValue = (int)key == 0;
        }
        else if (_keyProperty.PropertyType == typeof(long))
        {
            isDefaultValue = (long)key == 0;
        }
        else if (_keyProperty.PropertyType == typeof(uint))
        {
            isDefaultValue = (uint)key == 0;
        }
        else if (_keyProperty.PropertyType == typeof(ulong))
        {
            isDefaultValue = (ulong)key == 0;
        }
        else
        {
            throw new InvalidOperationException("Unreachable code");
        }

        return key;
    }

    public void SetKey(object document, object key)
    {
        _keyProperty.SetValue(document, key);
    }

    public object SetIfNeededAndGetPrimaryKeyValue(object doc, out bool shouldBeInserted)
    {
        if (doc == null) throw new ArgumentNullException(nameof(doc));

#if DEBUG
        if (doc.GetType() != DocumentType)
        {
            throw new InvalidOperationException(
                $"Document of type `{doc.GetType().FullName}` not expected. Expected `{DocumentType.FullName}`"
                );
        }
#endif

        object pk = _keyProperty.GetValue(doc);

        if (IsKeyPropertyGuid)
        {
            shouldBeInserted = true;

            Guid g = (Guid)pk;

            if (g == Guid.Empty)
            {
                Guid newValue = Guid.NewGuid();
                _keyProperty.SetValue(doc, newValue);

                return newValue;
            }

            return g;
        }

        if (IsKeyPropertyString)
        {
            shouldBeInserted = true;

            string s = (string)pk;

            if (s == null)
            {
                s = Guid.NewGuid().ToString();
                _keyProperty.SetValue(doc, s);
            }

            return s;
        }

#if DEBUG
        if (!IsKeyPropertyInt)
        {
            throw new InvalidOperationException(
                $"Expected key property for document `{DocumentType.FullName}` to be integer but got `{_keyProperty.PropertyType.FullName}`."
                );
        }
#endif

        if (_keyProperty.PropertyType == typeof(int))
        {
            int i = (int)pk;
            shouldBeInserted = i != 0;
        }
        else if (_keyProperty.PropertyType == typeof(uint))
        {
            uint i = (uint)pk;
            shouldBeInserted = i != 0;
        }
        else if (_keyProperty.PropertyType == typeof(long))
        {
            long i = (long)pk;
            shouldBeInserted = i != 0;
        }
        else if (_keyProperty.PropertyType == typeof(ulong))
        {
            ulong i = (ulong)pk;
            shouldBeInserted = i != 0;
        }
        else
        {
            throw new InvalidOperationException(
                $"Expected key property for document `{DocumentType.FullName}` to be integer but got `{_keyProperty.PropertyType.FullName}`."
            );
        }

        return pk;
    }

    internal static PropertyInfo GetKeyProperty(Type entityType)
    {
        PropertyInfo[] props = entityType.GetProperties();

        List<string> expectedKeyPropertyNames = new()
        {
            "Id",
            "Key",
            entityType.Name + "Id",
            entityType.Name + "Key"
        };

        List<PropertyInfo> prop = props.Where(x => x.CanRead && x.CanWrite && expectedKeyPropertyNames.Contains(x.Name)).ToList();

        if (prop.Count == 1)
        {
            return prop[0];
        }

        string expectedNames = "`" + string.Join("`, `", expectedKeyPropertyNames) + "`";

        if (prop.Count == 0)
        {
            throw new InvalidOperationException(
                "Cannot find public property with public getter and setter to use as primary key " +
                $"for `{entityType.Name}`, looked for {expectedNames}.");
        }

        throw new InvalidOperationException(
            "Found multiple public properties with public getter and setter to use as primary key " +
            $"for `{entityType.Name}`, looked for {expectedNames}.");
    }

    private static void EnsureTypeIsValid(Type documentType, string documentName)
    {
        void ThrowType(string error)
        {
            throw new ArgumentException($"Document type `{documentType.FullName}` is not valid. {error}", nameof(documentType));
        }

        void ThrowName(string error)
        {
            throw new ArgumentException($"Document name `{documentName}` is not valid. {error}");
        }

        if (!documentType.IsClass) ThrowType("It must be a class.");
        if (documentType.IsGenericType) ThrowType("It cannot be a generic class.");

        if (documentName is null)
        {
            if (documentType.IsSpecialName) ThrowType("It cannot have a special name. Alternatively provide a document name.");
        }

        documentName = (documentName ?? documentType.Name).Trim();

        if (documentName.Length == 0) ThrowName("It cannot be 0 length.");
        if (documentName.Any(c => c > 127)) ThrowName("It cannot contain non ASCII chars.");
        if (documentName.Any(char.IsWhiteSpace)) ThrowName("It cannot contain white-space.");
        if (documentName.Any(c => !char.IsDigit(c) && !char.IsLetter(c))) ThrowName("It can contain only letters and numbers.");
        if (!char.IsLetter(documentName[0])) ThrowName("It must start with letter.");
    }
}