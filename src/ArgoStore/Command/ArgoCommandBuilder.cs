using System.Text;
using System.Text.Json;
using ArgoStore.Config;
using ArgoStore.Helpers;
using ArgoStore.Statements;
using ArgoStore.Statements.Select;
using ArgoStore.Statements.Where;
using ArgoStore.StatementTranslators.Select;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace ArgoStore.Command;

internal class ArgoCommandBuilder
{
    private readonly Type _docType;
    private readonly ArgoCommandParameterCollection _params = new();
    private bool _containsLikeOperator;

    public DocumentMetadata Metadata { get; private set; }
    public List<WhereStatement> WhereStatements = new();
    public SelectStatementBase SelectStatement { get; private set; }
    public Type ResultingType { get; private set; }
    public bool IsResultingTypeJson { get; private set; } = true;

    public bool IsSelectCount => SelectStatement is SelectCountStatement;
    public bool IsSelectFirstOrSingle => SelectStatement is FirstSingleMaybeDefaultStatement;

    public string ItemName { get; set; }

    public ArgoCommandBuilder(QueryModel model)
        : this(model.MainFromClause.ItemType)
    {
    }

    public ArgoCommandBuilder(Type docType)
    {
        _docType = docType;
        ResultingType = docType;
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

        return new ArgoCommand(sql, _params, cmdType, ResultingType, IsResultingTypeJson, _containsLikeOperator);
    }

    public void AddWhereClause(WhereClause whereClause)
    {
        WhereStatements.Add(new WhereStatement(whereClause));
    }

    public void SetSelectStatement(SelectStatementBase selectStatement)
    {
        SelectStatement = selectStatement;
    }

    public void SetSelectStatement(SelectClause selectStatement)
    {
        SelectStatement = SelectToStatementTranslatorStrategies.Translate(selectStatement.Selector);

        if (SelectStatement is SelectPropertyStatement)
        {
            ResultingType = selectStatement.Selector.Type;
            IsResultingTypeJson = false;
        }
        else if (SelectStatement is SelectAnonymousType sat)
        {
            ResultingType = sat.AnonymousType;
        }
    }

    private void AppendSelect(StringBuilder sb)
    {
        if (SelectStatement is SelectCountStatement)
        {
            sb.Append("SELECT COUNT (1)");
        }
        else if (SelectStatement is SelectPropertyStatement sps)
        {
            sb.Append("SELECT json_insert('{}', '$.value', ")
                .Append(GetPropertyExtraction(sps.Name))
                .AppendLine(")");
        }
        else if (SelectStatement is SelectAnonymousType sat)
        {
            sb.Append("SELECT ");

            foreach (SelectPropertyStatement s in sat.SelectElements)
            {
                sb.Append("json_set(");
            }

            for (int i = 0; i < sat.SelectElements.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append("'{}'");
                }

                sb.Append(", ");

                string propName = ConvertPropertyCase(sat.SelectElements[i].ResultName);
                sb.Append("'$.").Append(propName).Append("', ");
                sb.Append(GetPropertyExtraction(sat.SelectElements[i].Name));
                sb.Append(")");
            }
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
                string paramName = _params.AddNewParameter(param.Value);
                sb.Append(" @").Append(paramName).Append(" ");
                break;

            case WhereStringTransformStatement wsts:
                AppendWhereStringTransform(sb, wsts);
                break;

            case WherePropertyStatement prop:
                sb.Append(GetPropertyExtraction(prop.PropertyName)).Append(" ");
                break;

            case WhereStringContainsMethodCallStatement scm:
                AppendWhereStringContains(sb, scm);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(statement));
        }

        sb.Append(" ");
    }

    private void AppendWhereStringTransform(StringBuilder sb, WhereStringTransformStatement wsts)
    {
        if (wsts.Transform == StringTransformTypes.ToLower)
        {
            sb.Append(" lower(");
            AppendWhereStatement(sb, wsts.Statement);
            sb.AppendLine(") ");
        }
        else if (wsts.Transform == StringTransformTypes.ToUpper)
        {
            sb.Append(" upper(");
            AppendWhereStatement(sb, wsts.Statement);
            sb.AppendLine(") ");
        }
        else if (wsts.Transform == StringTransformTypes.Trim)
        {
            sb.Append(" ltrim(rtrim(");
            AppendWhereStatement(sb, wsts.Statement);
            sb.AppendLine(")) ");
        }
        else if (wsts.Transform == StringTransformTypes.TrimEnd)
        {
            sb.Append(" rtrim(");
            AppendWhereStatement(sb, wsts.Statement);
            sb.AppendLine(") ");
        }
        else if (wsts.Transform == StringTransformTypes.TrimStart)
        {
            sb.Append(" ltrim(");
            AppendWhereStatement(sb, wsts.Statement);
            sb.AppendLine(") ");
        }
        else
        {
            throw new NotSupportedException($"String transform type: {wsts.Transform}");
        }
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

    private void AppendWhereStringContains(StringBuilder sb, WhereStringContainsMethodCallStatement s)
    {
        _containsLikeOperator = true;

        WhereStatementBase left = s.ObjectStatement;
        WhereStatementBase right = s.SubjectStatement;

        if (s.IgnoreCase)
        {
            left = new WhereStringTransformStatement(left, StringTransformTypes.ToLower);
            right = new WhereStringTransformStatement(right, StringTransformTypes.ToLower);
        }

        AppendWhereStatement(sb, left);

        sb.Append(" LIKE ");

        if (s.Method is StringMethods.Contains or StringMethods.EndsWith)
        {
            sb.Append(" '%' || ");
        }

        AppendWhereStatement(sb, right);

        if (s.Method is StringMethods.Contains or StringMethods.StartsWith)
        {
            sb.Append(" || '%' ");
        }
    }

    private string GetPropertyExtraction(string propertyName, string alias = null)
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
        return JsonNamingPolicy.CamelCase.ConvertName(propertyName).Replace("'", "''");
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