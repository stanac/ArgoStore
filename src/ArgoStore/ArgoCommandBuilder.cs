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
    
    public DocumentMetadata Metadata { get; private set; }
    public List<WhereStatement> WhereStatements = new();
    public SelectStatementBase SelectStatement { get; private set; }

    public bool IsSelectCount => SelectStatement is SelectCountStatement;
    public bool IsSelectFirstOrSingle => SelectStatement is FirstSingleMaybeDefaultStatement;

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
        Metadata = FindDocMeta(documentTypes);
        StringBuilder sb = StringBuilderBag.Default.Get();

        AppendSelect(sb);
        AppendFrom(sb);
        AppendWhere(sb, tenantId);
        AppendLimit(sb);

        string sql = sb.ToString();

        StringBuilderBag.Default.Return(sb);

        // QueryModel m = _model;

        ArgoCommandTypes cmdType = ArgoCommandTypes.ToList;

        if (SelectStatement is SelectCountStatement c)
        {
            cmdType = c.CountLong
                ? ArgoCommandTypes.LongCount
                : ArgoCommandTypes.Count;
        }
        else if (SelectStatement is FirstSingleMaybeDefaultStatement f)
        {
            if (f.IsFirst)
            {
                cmdType = f.IsDefault
                    ? ArgoCommandTypes.FirstOrDefault
                    : ArgoCommandTypes.First;
            }
            else
            {
                cmdType = f.IsDefault
                    ? ArgoCommandTypes.SingleOrDefault
                    : ArgoCommandTypes.Single;
            }
        }

        // TODO: set command type
        return new ArgoCommand(sql, _params, cmdType, Metadata.DocumentType);
    }

    public void AddWhereClause(WhereClause whereClause)
    {
        WhereStatements.Add(new WhereStatement(whereClause));
    }
    
    public void SetSelectStatement(SelectStatementBase selectStatement)
    {
        SelectStatement = selectStatement;
    }

    private void AppendSelect(StringBuilder sb)
    {
        if (SelectStatement is SelectCountStatement)
        {
            sb.Append("SELECT COUNT (1)");
        }
        else
        {
            sb.AppendLine("SELECT jsonData");
        }
    }

    private void AppendFrom(StringBuilder sb)
    {
        sb.Append("FROM ").Append(Metadata.DocumentName);
        sb.AppendLine();
    }

    private void AppendWhere(StringBuilder sb, string tenantId)
    {
        sb.Append("WHERE tenantId = @").AppendLine(_params.AddNewParameter(tenantId));

        foreach (WhereStatement w in WhereStatements)
        {
            sb.Append("AND (");
            AppendWhereStatement(sb, w.Statement);
            sb.AppendLine(")");
        }
    }

    private void AppendWhereStatement(StringBuilder sb, WhereStatementBase statement)
    {
        sb.Append(" ");
        
        switch (statement)
        {
            case WhereComparisonStatement wcs:
                AppendWhereComparison(sb, wcs);
                break;

            case WhereLogicalAndOrStatement wlaos:
                AppendWhereLogicalAndOr(sb, wlaos);
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

    private void AppendWhereComparison(StringBuilder sb, WhereComparisonStatement s)
    {
        sb.Append(" ");

        if (s.Operator.IsEqualOrNotEqual() && (s.Left is WhereNullValueStatement || s.Right is WhereNullValueStatement))
        {
            string op = s.Operator == ComparisonOperators.Equal
                ? "IS NULL"
                : "IS NOT NULL";

            if (s.Left is WhereNullValueStatement && s.Right is WhereNullValueStatement)
            {
                sb.Append("NULL ").Append(op);
            }
            else if (s.Left is WhereNullValueStatement)
            {
                AppendWhereStatement(sb, s.Right);
                sb.Append(" ").Append(op);
            }
            else
            {
                AppendWhereStatement(sb, s.Left);
                sb.Append(" ").Append(op);
            }
        }
        else
        {
            AppendWhereStatement(sb, s.Left);
            sb.Append(" ").Append(s.Operator.ToSqlOperator()).Append(" ");
            AppendWhereStatement(sb, s.Right);
        }

        sb.Append(" ");
    }

    private void AppendWhereLogicalAndOr(StringBuilder sb, WhereLogicalAndOrStatement s)
    {
        string andOr = s.IsAnd ? "AND" : "OR";

        sb.Append(" (( ");
        AppendWhereStatement(sb, s.Left);
        sb.Append(" ) ").Append(andOr).Append(" ( ");
        AppendWhereStatement(sb, s.Right);
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

    private void AppendLimit(StringBuilder sb)
    {
        if (IsSelectFirstOrSingle)
        {
            sb.Append("LIMIT 1");
        }
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