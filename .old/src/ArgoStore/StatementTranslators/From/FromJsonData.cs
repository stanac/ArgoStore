using ArgoStore.Config;

namespace ArgoStore.StatementTranslators.From;

internal class FromJsonData : FromStatementBase
{
    public DocumentMetadata DocumentMetadata { get; }
    
    public FromJsonData(DocumentMetadata documentMetadata)
    {
        DocumentMetadata = documentMetadata ?? throw new ArgumentNullException(nameof(documentMetadata));
    }
}