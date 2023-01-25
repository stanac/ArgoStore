using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Command;
using ArgoStore.Helpers;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

// ReSharper disable RedundantOverriddenMember

namespace ArgoStore.Implementations;

internal class ArgoWhereSubQueryModelVisitor : QueryModelVisitorBase
{
    public ArgoWhereSubQueryCommandBuilder CommandBuilder { get; }
    
    public ArgoWhereSubQueryModelVisitor(ArgoWhereSubQueryCommandBuilder commandBuilder)
    {
        CommandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
    }

    public void Visit(SubQueryExpression ex)
    {
        VisitQueryModel(ex.QueryModel);
    }

    public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
    {
        base.VisitAdditionalFromClause(fromClause, queryModel, index);
    }

    protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses, QueryModel queryModel)
    {
        base.VisitBodyClauses(bodyClauses, queryModel);
    }

    public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
    {
        base.VisitGroupJoinClause(groupJoinClause, queryModel, index);
    }

    public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
    {
        base.VisitJoinClause(joinClause, queryModel, index);
    }

    public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
    {
        base.VisitJoinClause(joinClause, queryModel, groupJoinClause);
    }

    public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
    {
        if (fromClause.FromExpression is MemberExpression me && me.Member is PropertyInfo pi)
        {
            CommandBuilder.FromProperty = pi;
        }
        else
        {
            throw new NotSupportedException(
                $"Not supported expression `{fromClause.FromExpression.Describe()}` in subquery"
                );
        }

        base.VisitMainFromClause(fromClause, queryModel);
    }

    public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
    {
        throw new NotSupportedException("Ordering not supported in subquery");
        // base.VisitOrderByClause(orderByClause, queryModel, index);
    }

    public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
    {
        throw new NotSupportedException("Ordering not supported in subquery");
        // base.VisitOrdering(ordering, queryModel, orderByClause, index);
    }

    protected override void VisitOrderings(ObservableCollection<Ordering> orderings, QueryModel queryModel, OrderByClause orderByClause)
    {
        throw new NotSupportedException("Ordering not supported in subquery");
        // base.VisitOrderings(orderings, queryModel, orderByClause);
    }

    public override void VisitQueryModel(QueryModel queryModel)
    {
        base.VisitQueryModel(queryModel);
    }

    public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
    {
        if (resultOperator is AnyResultOperator)
        {
            CommandBuilder.IsAny = true;
        }
        else if (resultOperator is CountResultOperator)
        {
            CommandBuilder.IsCount = true;
        }
        else
        {
            throw new NotSupportedException($"Not supported operator in subquery: {resultOperator.GetType().Name}");
        }
        
        base.VisitResultOperator(resultOperator, queryModel, index);
    }

    protected override void VisitResultOperators(ObservableCollection<ResultOperatorBase> resultOperators, QueryModel queryModel)
    {
        base.VisitResultOperators(resultOperators, queryModel);
    }

    public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
    {
        base.VisitSelectClause(selectClause, queryModel);
    }

    public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
    {
        base.VisitWhereClause(whereClause, queryModel, index);
    }
}