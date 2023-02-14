using System.Text;
using ArgoStore.Helpers;
using ArgoStore.Statements;
using ArgoStore.Statements.Order;
using ArgoStore.Statements.Select;
using ArgoStore.Statements.Where;
using ArgoStore.StatementTranslators.From;
using ArgoStore.StatementTranslators.Select;
using ArgoStore.StatementTranslators.Where;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace ArgoStore.Command;

internal class ArgoCommandBuilder
{
    private readonly ArgoCommandParameterCollection _params;
    private bool _containsLikeOperator;

    public FromStatementBase FromStatement { get; }
    public FromAlias Alias { get; }
    public List<WhereStatement> WhereStatements { get; } = new();
    public List<OrderByStatement> OrderByStatements { get; } = new();
    public SelectStatementBase? SelectStatement { get; private set; }
    public Type ResultingType { get; private set; }
    public bool IsResultingTypeJson { get; private set; } = true;
    public bool IsDistinct { get; private set; }
    public int? Take { get; set; }
    public int? Skip { get; set; }
    public bool IsSelectCount => SelectStatement is SelectCountStatement;
    public bool IsSelectFirstOrSingle => SelectStatement is FirstSingleMaybeDefaultStatement;
    public bool IsSelectAny => SelectStatement is SelectAnyStatement;
    
    public string? ItemName { get; set; }
    
    public ArgoCommandBuilder(FromStatementBase fromStatement, FromAlias alias)
    {
        FromStatement = fromStatement ?? throw new ArgumentNullException(nameof(fromStatement));
        Alias = alias;
        _params = new ArgoCommandParameterCollection(alias);
        
        if (fromStatement is FromJsonData fjd)
        {
            ResultingType = fjd.DocumentMetadata.DocumentType;
        }
        else
        {
            ResultingType = typeof(bool);
        }
    }
    
    public void SetIsDistinct(bool value)
    {
        IsDistinct = value;
    }
    
    public ArgoCommand Build(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));
        
        StringBuilder sb = StringBuilderBag.Default.Get();

        AppendSelect(sb);
        AppendFrom(sb);
        AppendWhere(sb, tenantId);
        AppendOrderBy(sb);
        AppendLimit(sb);
        AppendSkip(sb);

        string sql = sb.ToString();

        StringBuilderBag.Default.Return(sb);

        // QueryModel m = _model;
        
        return new ArgoCommand(sql, _params, GetCommandType(), ResultingType, IsResultingTypeJson, _containsLikeOperator);
    }

    private ArgoCommandTypes GetCommandType()
    {
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
        else if (SelectStatement is SelectAnyStatement)
        {
            cmdType = ArgoCommandTypes.Any;
        }

        return cmdType;
    }

    public void AddWhereClause(WhereClause whereClause)
    {
        WhereStatements.Add(new WhereStatement(whereClause, Alias));
    }

    public void SetSelectStatement(SelectStatementBase selectStatement)
    {
        SelectStatement = selectStatement;

        if (selectStatement is SelectAnyStatement)
        {
            ResultingType = typeof(bool);
        }
    }

    public void SetSelectClause(SelectClause selectStatement)
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
            sb.Append("SELECT ");

            if (IsDistinct)
            {
                sb.Append("DISTINCT ");
            }

            sb.Append("json_insert('{}', '$.value', ")
                .Append(ExtractProperty(sps.Name))
                .AppendLine(")");
        }
        else if (SelectStatement is SelectAnonymousType sat)
        {
            sb.Append("SELECT ");

            if (IsDistinct)
            {
                sb.Append("DISTINCT ");
            }


            for (int i = 0; i < sat.SelectElements.Count; i++)
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

                string propName = JsonPropertyDataHelper.ConvertPropertyNameCase(sat.SelectElements[i].ResultName!);
                sb.Append("'$.").Append(propName).Append("', ");

                if (sat.SelectElements[i] is SelectPropertyStatement sps1)
                {
                    sb.Append(ExtractProperty(sps1.Name));
                }
                else if (sat.SelectElements[i] is SelectParameterStatement sps2)
                {
                    string paramName = _params.AddNewParameter(sps2.Value);
                    sb.Append("@").Append(paramName);
                }

                sb.AppendLine(")");
            }
        }
        else if (SelectStatement is SelectAnyStatement)
        {
            sb.AppendLine("SELECT COUNT (1)");
        }
        else
        {
            sb.AppendLine("SELECT t1.jsonData");
        }
    }

    private void AppendFrom(StringBuilder sb)
    {
        sb.Append("FROM ");

        if (FromStatement is FromJsonData jd)
        {
            sb.Append(jd.DocumentMetadata.DocumentName);
        }
        else if (FromStatement is FromProperty fp)
        {
            sb.Append("json_each(").Append(ExtractParentProperty(fp.PropertyName)).Append(") ");
        }
        else
        {
            throw new NotSupportedException();
        }

        sb.Append(" ").Append(Alias.CurrentAliasName).AppendLine();
    }

    #region Where

    private void AppendWhere(StringBuilder sb, string? tenantId)
    {
        if (tenantId == null && WhereStatements.Count == 0)
        {
            return;
        }

        sb.Append("WHERE ");

        if (tenantId != null)
        {
            sb.Append("tenantId = @").AppendLine(_params.AddNewParameter(tenantId));
        }

        for (var i = 0; i < WhereStatements.Count; i++)
        {
            if (i > 0 || tenantId != null)
            {
                sb.Append(" AND ");
            }

            sb.Append(" (");

            WhereStatement w = WhereStatements[i];
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
                AppendWhereParameterStatement(sb, param);
                break;

            case WhereStringTransformStatement wsts:
                AppendWhereStringTransform(sb, wsts);
                break;

            case WherePropertyStatement prop:
                AppendWherePropertyStatement(sb, prop);
                break;

            case WhereStringContainsMethodCallStatement scm:
                AppendWhereStringContains(sb, scm);
                break;

            case WhereNotStatement ns:
                sb.Append(" NOT( ");
                AppendWhereStatement(sb, ns.Statement);
                sb.Append(" ) ");
                break;

            case WhereCollectionContainsStatement cc:
                AppendWhereCollectionContains(sb, cc);
                break;

            case WhereSubQueryStatement sqs:
                AppendWhereSubQuery(sb, sqs);
                break;

            case WhereStringLengthStatement wsls:
                AppendWhereStringLength(sb, wsls);
                break;

            case WhereSubQueryFromStatement wsqf:
                AppendWhereSubQueryFrom(sb, wsqf);
                break;

            case WhereSubQueryValueStatement wsqvs:
                AppendWhereSubQueryValue(sb, wsqvs);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(statement));
        }

        sb.Append(" ");
    }

    private void AppendWherePropertyStatement(StringBuilder sb, WherePropertyStatement prop)
    {
        bool isDate = prop.PropertyType.IsTypeADateType();

        if (isDate)
        {
            sb.Append("strftime('%s', ");
        }
        
        sb.Append(ExtractProperty(prop.PropertyName, prop.FromAlias)).Append(" ");
        
        if (isDate)
        {
            sb.Append(") ");
        }
    }

    private void AppendWhereParameterStatement(StringBuilder sb, WhereParameterStatement param)
    {
        bool isDate = param.Type.IsTypeADateType();

        if (isDate)
        {
            sb.Append("strftime('%s', ");

            string paramName = _params.AddNewParameter(param.Value.FormatAsIso8601DateTimeString()!);
            sb.Append(" @").Append(paramName).Append(" ");

            sb.Append(")");
        }
#if !NETSTANDARD
        else if (param.Type == typeof(TimeOnly) || param.Type == typeof(TimeOnly?))
        {
            TimeOnly val = (param.Value as TimeOnly?)!.Value;
            int intVal = (int)val.ToTimeSpan().TotalMilliseconds;

            string paramName = _params.AddNewParameter(intVal);
            sb.Append(" @").Append(paramName).Append(" ");
        }
#endif
        else
        {
            string paramName = _params.AddNewParameter(param.Value);
            sb.Append(" @").Append(paramName).Append(" ");
        }
    }

    private void AppendWhereCollectionContains(StringBuilder sb, WhereCollectionContainsStatement statement)
    {
        /*
            SELECT *, json_extract(jsonData, '$.roles') 
            FROM Person
            WHERE (json_extract(jsonData, '$.roles') IS NOT NULL AND (
	              EXISTS (SELECT 1 FROM json_each(json_extract(jsonData, '$.roles')) WHERE value = 'admin')
		            ))
        */
        
        sb.AppendLine().Append(" /* Collection contains */ ( ");
        AppendWhereStatement(sb, statement.Collection);
        sb.Append(" IS NOT NULL AND (").Append("    EXISTS (SELECT 1 FROM json_each(");
        AppendWhereStatement(sb, statement.Collection);
        sb.Append(") WHERE value = ");
        AppendWhereStatement(sb, statement.Value);
        sb.AppendLine("))) ");
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

    private void AppendWhereStringLength(StringBuilder sb, WhereStringLengthStatement wsls)
    {
        sb.Append("length(");
        AppendWhereStatement(sb, wsls.Value);
        sb.Append(")");
    }

    private void AppendWhereSubQuery(StringBuilder sb, WhereSubQueryStatement sqs)
    {
        if (sqs is WhereSubQueryAnyStatement s1)
        {
            AppendWhereSubQueryAny(sb, s1);
        }
        else if (sqs is WhereSubQueryContainsStatement s2)
        {
            AppendWhereSubQueryContains(sb, s2);
        }
        else if (sqs is WhereSubQueryCountStatement s3)
        {
            AppendWhereSubQueryCount(sb, s3);
        }
        else
        {
            throw new NotSupportedException($"SubQuery {sqs.GetType().FullName} not supported.");
        }
    }

    private void AppendWhereSubQueryAny(StringBuilder sb, WhereSubQueryAnyStatement s)
    {
        if (s.Condition is null)
        {
            sb.Append("json_array_length(");
            AppendWhereStatement(sb, s.From.FromStatement);
            sb.Append(") > 0");
        }
        else
        {
            sb.Append("EXISTS ( SELECT 1 FROM ");
            AppendWhereStatement(sb, s.From);
            sb.Append(" WHERE ");
            AppendWhereStatement(sb, s.Condition);
            sb.Append(" LIMIT 1 )");
        }
    }

    private void AppendWhereSubQueryContains(StringBuilder sb, WhereSubQueryContainsStatement s)
    {
        sb.Append("EXISTS ( SELECT 1 FROM ");
        AppendWhereStatement(sb, s.From);
        sb.Append($" WHERE {s.Alias.CurrentAliasName}.value = ");
        AppendWhereStatement(sb, s.Value);
        sb.Append(") ");
    }

    private void AppendWhereSubQueryCount(StringBuilder sb, WhereSubQueryCountStatement s)
    {
        if (s.Condition is null)
        {
            sb.Append("json_array_length(");
            AppendWhereStatement(sb, s.From.FromStatement);
            sb.Append(")");
        }
        else
        {
            sb.Append("( SELECT COUNT(1) FROM ");
            AppendWhereStatement(sb, s.From);
            sb.Append(" WHERE ");
            AppendWhereStatement(sb, s.Condition);
            sb.Append(" )");
        }
    }

    private void AppendWhereSubQueryFrom(StringBuilder sb, WhereSubQueryFromStatement s)
    {
        if (s.FromStatement is WhereValueStatement v)
        {
            sb.Append("json_each(");
            AppendWhereStatement(sb, v);
            sb.Append(") ").Append(s.Alias.CurrentAliasName);
        }
        else
        {
            throw new NotSupportedException(
                $"Not supported subquery from statement of type: {s.FromStatement.GetType().Name}"
                );
        }
    }

    private void AppendWhereSubQueryValue(StringBuilder sb, WhereSubQueryValueStatement s)
    {
        sb.Append($"{s.CurrentAlias}.value");
    }

    #endregion Where

    public void HandleResultOperator(QueryModel queryModel, ContainsResultOperator cro)
    {
        WhereValueStatement value = (WhereValueStatement)WhereToStatementTranslatorStrategies.Translate(cro.Item, Alias);
        WhereValueStatement source = (WhereValueStatement)WhereToStatementTranslatorStrategies.Translate(queryModel.MainFromClause.FromExpression, Alias);

        WhereStatementBase contains = new WhereCollectionContainsStatement(source, value);

        WhereStatements.Add(new WhereStatement(contains));
    }

    public void HandleCountResultOperator(QueryModel queryModel)
    {
        throw new NotImplementedException();
    }

    private void AppendOrderBy(StringBuilder sb)
    {
        if (OrderByStatements.Any())
        {
            sb.Append("ORDER BY ");

            for (var i = 0; i < OrderByStatements.Count; i++)
            {
                OrderByStatement s = OrderByStatements[i];

                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(ExtractProperty(s.PropertyName))
                    .Append(" ")
                    .Append(s.Direction)
                    .Append(" ");
            }

            sb.AppendLine();
        }
    }

    private void AppendSkip(StringBuilder sb)
    {
        if (Skip.HasValue)
        {
            sb.Append(" OFFSET ").AppendLine(Skip.Value.ToString());
        }
    }

    private void AppendLimit(StringBuilder sb)
    {
        if (IsSelectFirstOrSingle || IsSelectAny)
        {
            sb.Append("LIMIT 1");
        }
        else if (Take.HasValue)
        {
            sb.Append("LIMIT ").Append(Take.Value.ToString());
        }
    }

    private string ExtractProperty(string propertyName)
    {
        return ExtractProperty(propertyName, Alias.CurrentAliasName);
    }

    private string ExtractProperty(string propertyName, string? alias)
    {
        alias = alias ?? Alias.CurrentAliasName;
        return JsonPropertyDataHelper.ExtractProperty(propertyName, alias);
    }

    private string ExtractParentProperty(string propertyName)
    {
        if (Alias.ParentAlias.HasValue)
        {
            return JsonPropertyDataHelper.ExtractProperty(propertyName, Alias.ParentAliasName);
        }

        throw new InvalidOperationException("Parent alias not set");
    }
}