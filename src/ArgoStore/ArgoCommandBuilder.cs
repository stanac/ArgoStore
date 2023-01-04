using System.Text;
using System.Xml.Linq;
using ArgoStore.Statements.Select;
using ArgoStore.Statements.Where;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace ArgoStore;

internal class ArgoCommandBuilder
{
    private readonly QueryModel _model;
    private readonly Type _docType;
    private readonly ArgoCommandParameterCollection _params = new();
    private readonly List<WhereStatement> _whereStatements = new();
    private SelectStatementBase _selectStatement;

    public string ItemName { get; set; }

    public ArgoCommandBuilder(QueryModel model)
        : this (model.MainFromClause.ItemType)
    {
        _model = model;
    }

    public ArgoCommandBuilder(Type docType)
    {
        _docType = docType;
    }

    public ArgoCommand Build(IReadOnlyDictionary<string, DocumentMetadata> documentTypes, string tenantId)
    {
        DocumentMetadata meta = FindDocMeta(documentTypes);
        StringBuilder sb = StringBuilderBag.Default.Get();

        AddSelect(sb);
        AddFrom(sb, meta);
        AddWhere(sb, meta, tenantId);

        string sql = sb.ToString();

        StringBuilderBag.Default.Return(sb);

        QueryModel m = _model;

        // TODO: set command type
        return new ArgoCommand(sql, _params, ArgoCommandTypes.ToList, meta.DocumentType);
    }

    public void AddWhereClause(WhereClause whereClause, QueryModel queryModel)
    {
        _whereStatements.Add(new WhereStatement(whereClause, queryModel));
    }

    public void SetSelectStatement(SelectStatementBase selectStatement)
    {
        _selectStatement = selectStatement;
    }

    private void AddSelect(StringBuilder sb)
    {
        sb.AppendLine("SELECT jsonData");
    }

    private void AddFrom(StringBuilder sb, DocumentMetadata meta)
    {
        sb.Append("FROM ").Append(meta.DocumentName);
        sb.AppendLine();
    }

    private void AddWhere(StringBuilder sb, DocumentMetadata meta, string tenantId)
    {
        sb.Append("WHERE tenantId = @").AppendLine(_params.AddNewParameter(tenantId));

        foreach (WhereStatement w in _whereStatements)
        {
            switch (w.Statement)
            {
                case WhereComparisonStatement wcs:
                    sb.AppendLine($"    AND ({wcs.Left} {wcs.Operator.ToSqlOperator()} {wcs.Right})");
                    break;

                default:
                    throw new NotSupportedException($"Not supported where statement type: `{w.GetType().FullName}`");
            }
        }
    }

    private string ConvertWhereStatement(WhereStatementBase statement)
    {
        throw new NotImplementedException();
    }

    private DocumentMetadata FindDocMeta(IReadOnlyDictionary<string, DocumentMetadata> documentTypes)
    {
        KeyValuePair<string, DocumentMetadata>[] types = documentTypes.Where(x => x.Value.DocumentType == _docType).ToArray();

        if (types.Length == 0)
        {
            throw new InvalidOperationException($"Document metadata for type `{_docType.FullName}` is not registered.");
        }

        return types[0].Value;
    }
}