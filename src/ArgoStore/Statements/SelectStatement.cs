namespace ArgoStore.Statements;

internal class SelectStatement : Statement
{
    public WhereStatement WhereStatement { get; private set; }
    public SelectStatement SubQueryStatement { get; private set; }
    public OrderByStatement OrderByStatement { get; private set; }
    public Type TypeFrom { get; }
    public Type TypeTo { get; }
    public IReadOnlyList<SelectStatementElement> SelectElements { get; }
    public int? Top { get; private set; }
    public CalledByMethods CalledByMethod { get; private set; }
    public string Alias { get; private set; }
        
    public SelectStatement(WhereStatement whereStatement, Type typeFrom, Type typeTo, IReadOnlyList<SelectStatementElement> selectElements,
        int? top, CalledByMethods calledByMethod)
        : this (typeFrom, typeTo, selectElements, top, calledByMethod)
    {
        WhereStatement = whereStatement ?? throw new ArgumentNullException(nameof(whereStatement));
    }

    public SelectStatement(SelectStatement subQueryStatement, Type typeFrom, Type typeTo, IReadOnlyList<SelectStatementElement> selectElements,
        int? top, CalledByMethods calledByMethod)
        : this (typeFrom, typeTo, selectElements, top, calledByMethod)
    {
        SubQueryStatement = subQueryStatement ?? throw new ArgumentNullException(nameof(subQueryStatement));
    }

    public SelectStatement(OrderByStatement orderByStatement, Type typeFrom, Type typeTo, IReadOnlyList<SelectStatementElement> selectElements,
        int? top, CalledByMethods calledByMethod)
        : this(typeFrom, typeTo, selectElements, top, calledByMethod)
    {
        OrderByStatement = orderByStatement ?? throw new ArgumentNullException(nameof(orderByStatement));
    }

    public SelectStatement(Type typeFrom, Type typeTo, IReadOnlyList<SelectStatementElement> selectElements,
        int? top, CalledByMethods calledByMethod)
    {
        TypeFrom = typeFrom ?? throw new ArgumentNullException(nameof(typeFrom));
        TypeTo = typeTo ?? throw new ArgumentNullException(nameof(typeTo));
        SelectElements = selectElements ?? throw new ArgumentNullException(nameof(selectElements));
        Top = top;
        CalledByMethod = calledByMethod;

        if (top.HasValue && top < 1) throw new ArgumentException($"{nameof(top)} cannot be less than 1", nameof(top));
    }

    public static SelectStatement Create(Statement from, Type typeFrom, Type typeTo, IReadOnlyList<SelectStatementElement> selectElements,
        int? top, CalledByMethods calledByMethod)
    {
        if (from == null) return new SelectStatement(typeFrom, typeTo, selectElements, top, calledByMethod);

        if (from is WhereStatement w) return new SelectStatement(w, typeFrom, typeTo, selectElements, top, calledByMethod);

        if (from is SelectStatement s) return new SelectStatement(s, typeFrom, typeTo, selectElements, top, calledByMethod);

        if (from is OrderByStatement o) return new SelectStatement(o, typeFrom, typeTo, selectElements, top, calledByMethod);

        throw new ArgumentException($"{from} statement of type {from.GetType().FullName} not supported in SelectStatement.Create", nameof(from));
    }

    public override Statement Negate()
    {
        throw new NotSupportedException();
    }

    public override Statement ReduceIfPossible()
    {
        if (SubQueryStatement != null) SubQueryStatement = SubQueryStatement.ReduceIfPossible() as SelectStatement;
        if (WhereStatement != null) WhereStatement = WhereStatement.ReduceIfPossible() as WhereStatement;
        return this;
    }

    public override string ToDebugString()
    {
        return $"SELECT {string.Join(", ", SelectElements.Select(x => x.ToDebugString()))} {WhereStatement?.ToDebugString()}";
    }

    public void SetAliases(int level)
    {
        Alias = $"\"..t{level}\"";

        if (WhereStatement != null) WhereStatement.SetAliases(level);
        if (SubQueryStatement != null) SubQueryStatement.SetAliases(level + 1);

        if (SelectElements != null)
        {
            for (int i = 0; i < SelectElements.Count; i++)
            {
                SelectElements[i].Alias = $"\"...c{i}\"";
            }
        }
    }

    public void SetSubQueryAliases(SelectStatement parent)
    {
        if (SubQueryStatement != null)
        {
            SubQueryStatement.SetSubQueryAliases(this);
        }

        if (parent != null)
        {
            foreach (SelectStatementElement e in parent.SelectElements)
            {
                e.BindsToSubQueryAlias = SelectElements.First(x => x.OutputProperty == e.InputProperty).Alias;
            }
        }
    }

    public SelectStatement SetOrderBy(OrderByStatement orderByStatement)
    {
        if (OrderByStatement == null)
        {
            OrderByStatement = orderByStatement ?? throw new ArgumentNullException(nameof(orderByStatement));
        }
        else
        {
            OrderByStatement = OrderByStatement.Join(orderByStatement);
        }
        return this;
    }

    public SelectStatement AddWhereCondition(WhereStatement where)
    {
        if (where is null) throw new ArgumentNullException(nameof(where));

        if (WhereStatement == null)
        {
            WhereStatement = where;
        }
        else
        {
            WhereStatement.AddConjunctedCondition(where.Statement);
        }

        return this;
    }

    public SelectStatement SetTop(int top)
    {
        Top = top;
        return this;
    }

    public SelectStatement SetCalledByMethod(CalledByMethods method)
    {
        CalledByMethod = method;
        return this;
    }
}