using System.Text;
using System.Text.Json;
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

    public bool IsSelectCount => _selectStatement is SelectCountStatement;

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

        AppendSelect(sb);
        AppendFrom(sb, meta);
        AppendWhere(sb, meta, tenantId);

        string sql = sb.ToString();

        StringBuilderBag.Default.Return(sb);

        QueryModel m = _model;

        ArgoCommandTypes cmdType = ArgoCommandTypes.ToList;

        if (_selectStatement is SelectCountStatement c)
        {
            cmdType = c.CountLong
                ? ArgoCommandTypes.LongCount
                : ArgoCommandTypes.Count;
        }

        // TODO: set command type
        return new ArgoCommand(sql, _params, cmdType, meta.DocumentType);
    }

    public void AddWhereClause(WhereClause whereClause)
    {
        _whereStatements.Add(new WhereStatement(whereClause));
    }
    
    public void SetSelectStatement(SelectStatementBase selectStatement)
    {
        _selectStatement = selectStatement;
    }

    private void AppendSelect(StringBuilder sb)
    {
        if (_selectStatement is SelectCountStatement)
        {
            sb.Append("SELECT COUNT (1)");
        }
        else
        {
            sb.AppendLine("SELECT jsonData");
        }
    }

    private void AppendFrom(StringBuilder sb, DocumentMetadata meta)
    {
        sb.Append("FROM ").Append(meta.DocumentName);
        sb.AppendLine();
    }

    private void AppendWhere(StringBuilder sb, DocumentMetadata meta, string tenantId)
    {
        sb.Append("WHERE tenantId = @").AppendLine(_params.AddNewParameter(tenantId));

        foreach (WhereStatement w in _whereStatements)
        {
            sb.Append(" AND ( ");
            AppendWhereStatement(sb, w.Statement, meta);
            sb.AppendLine(" ) ");
        }
    }

    private void AppendWhereStatement(StringBuilder sb, WhereStatementBase statement, DocumentMetadata meta)
    {
        sb.Append(" ");
        
        switch (statement)
        {
            case WhereComparisonStatement wcs:
                AppendWhereComparison(sb, wcs, meta);
                break;

            case WhereLogicalAndOrStatement wlaos:
                AppendWhereLogicalAndOr(sb, wlaos, meta);
                break;

            case WhereParameterStatement param:
                sb.Append(" @").Append(param.ParameterName).Append(" ");
                break;
            case WherePropertyStatement prop:
                sb.Append(GetParameterExtraction(prop.PropertyName)).Append(" ");
                break;
                
            default:
                throw new ArgumentOutOfRangeException(nameof(statement));
        }

        sb.Append(" ");
    }

    private void AppendWhereComparison(StringBuilder sb, WhereComparisonStatement s, DocumentMetadata meta)
    {
        sb.Append(" ");

        if (s.Operator.IsEqualOrNotEqual() && (s.Left is WhereNullValueStatement || s.Right is WhereNullValueStatement))
        {
            if (s.Left is WhereNullValueStatement && s.Right is WhereNullValueStatement)
            {
                sb.Append("NULL IS NULL");
            }
            else if (s.Left is WhereNullValueStatement)
            {
                AppendWhereStatement(sb, s.Right, meta);
                sb.Append(" IS NULL");
            }
            else
            {
                AppendWhereStatement(sb, s.Left, meta);
                sb.Append(" IS NULL");
            }
        }
        else
        {
            AppendWhereStatement(sb, s.Left, meta);
            sb.Append(" ").Append(s.Operator.ToSqlOperator()).Append(" ");
            AppendWhereStatement(sb, s.Right, meta);
        }

        sb.Append(" ");
    }

    private void AppendWhereLogicalAndOr(StringBuilder sb, WhereLogicalAndOrStatement s, DocumentMetadata meta)
    {
        string andOr = s.IsAnd ? "AND" : "OR";

        sb.Append(" (( ");
        AppendWhereStatement(sb, s.Left, meta);
        sb.Append(" ) ").Append(andOr).Append(" ( ");
        AppendWhereStatement(sb, s.Right, meta);
        sb.Append(" )) ");
    }

    private string GetParameterExtraction(string propertyName, string alias = null)
    {
        propertyName = ConvertPropertyCase(propertyName);

        if (string.IsNullOrWhiteSpace(alias))
        {
            return $"json_extract(jsonData, '$.{propertyName}')";
        }

        return $"json_extract({alias}.jsonData, '$.{propertyName}')";
    }

    private string ConvertPropertyCase(string propertyName)
    {
        return JsonNamingPolicy.CamelCase.ConvertName(propertyName);
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