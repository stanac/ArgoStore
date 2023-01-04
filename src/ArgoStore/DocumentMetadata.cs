using System.ComponentModel.Design;

namespace ArgoStore;

internal class DocumentMetadata
{
    public Type DocumentType { get; }
    public string DocumentName { get; }

    public DocumentMetadata(Type documentType)
    {
        DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
        EnsureTypeIsValid(documentType, null);
        DocumentName = documentType.Name.Trim();
    }

    public DocumentMetadata(Type documentType, string documentName)
    {
        if (string.IsNullOrWhiteSpace(documentName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(documentName));
        DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
        DocumentName = documentName.Trim();
        EnsureTypeIsValid(documentType, DocumentName);
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