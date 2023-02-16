using System.Diagnostics;
using System.Reflection;

namespace ArgoStore.Config;

internal class DocumentMetadata
{
    private readonly PropertyInfo _keyProperty;

    public Type DocumentType { get; }
    public string DocumentName { get; }
    public List<DocumentIndexMetadata> Indexes { get; }
    public bool IsKeyPropertyString { get; }
    public bool IsKeyPropertyGuid => !IsKeyPropertyString;
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

        EnsureKeyPropertyIsValidForIdentity();

        IsKeyPropertyString = _keyProperty.PropertyType == typeof(string);

        EnsureTypeIsValid(documentType, documentTableName);
    }
    
    /// <summary>
    /// If primary key is of type Guid checks if it is set, if not it's set to new value.
    /// If primary key is of type String checks if it is set, if not throws exception.
    /// Returns value of primary key (newly set or existing).
    /// </summary>
    /// <param name="doc">Document</param>
    /// <returns>PK value</returns>
    public object SetIfNeededAndGetPrimaryKeyValue(object doc)
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

        // TODO: optimize reflection
        object pk = _keyProperty.GetValue(doc)
            ?? throw new InvalidOperationException("Primary key value cannot be null. Document type: " +
                                                   $"`{DocumentType.FullName}`, property name: `{_keyProperty.Name}`.");
        
        if (IsKeyPropertyGuid)
        {
            Guid g = (Guid) pk;

            if (g == Guid.Empty)
            {
                g = Guid.NewGuid();
                _keyProperty.SetValue(doc, g);
            }

            return g;
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

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
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

    private void EnsureKeyPropertyIsValidForIdentity()
    {
        if (!_keyProperty.CanRead || !_keyProperty.CanWrite)
        {
            throw new InvalidOperationException($"Identity property `{_keyProperty.Name}` must have public `get` and `set` methods.");
        }

        if (_keyProperty.PropertyType != typeof(string) && _keyProperty.PropertyType != typeof(Guid))
        {
            throw new InvalidOperationException($"Identity property `{_keyProperty.Name}` must be of type `System.String` or `System.Guid`.");
        }
    }

}