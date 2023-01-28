using System.Collections.ObjectModel;
using ArgoStore.Command;
using ArgoStore.Config;
using ArgoStore.Statements;
using ArgoStore.Statements.Select;
using ArgoStore.Statements.SkipTake;
using ArgoStore.StatementTranslators.From;
using ArgoStore.StatementTranslators.Order;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
// ReSharper disable RedundantOverriddenMember

namespace ArgoStore.Implementations;

internal class ArgoQueryModelVisitor : QueryModelVisitorBase
{
    public ArgoCommandBuilder CommandBuilder { get; private set; }

    public ArgoQueryModelVisitor(DocumentMetadata metadata)
    {
        CommandBuilder = new ArgoCommandBuilder(new FromJsonData(metadata), new FromAlias());
    }

    public ArgoQueryModelVisitor(ArgoCommandBuilder commandBuilder)
    {
        CommandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
    }

    public override void VisitQueryModel(QueryModel queryModel)
    {
        base.VisitQueryModel(queryModel);
    }

    public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
    {
        CommandBuilder.AddWhereClause(whereClause);
        base.VisitWhereClause(whereClause, queryModel, index);
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
        throw new NotSupportedException("Join not supported");
    }

    public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
    {
        throw new NotSupportedException("Join not supported");
    }

    public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
    {
        throw new NotSupportedException("Join not supported");
    }

    public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
    {
        CommandBuilder.ItemName = fromClause.ItemName;
        base.VisitMainFromClause(fromClause, queryModel);
    }

    public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
    {
        base.VisitOrderByClause(orderByClause, queryModel, index);
    }

    public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
    {
        CommandBuilder.OrderByStatements.AddRange(OrderTranslator.Translate(ordering));
        base.VisitOrdering(ordering, queryModel, orderByClause, index);
    }

    protected override void VisitOrderings(ObservableCollection<Ordering> orderings, QueryModel queryModel, OrderByClause orderByClause)
    {
        base.VisitOrderings(orderings, queryModel, orderByClause);
    }

    public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
    {
        if (resultOperator is CountResultOperator)
        {
            CommandBuilder.SetSelectStatement(new SelectCountStatement());
        }
        else if (resultOperator is LongCountResultOperator)
        {
            CommandBuilder.SetSelectStatement(new SelectCountStatement(true));
        }
        else if (resultOperator is FirstResultOperator fro)
        {
            CommandBuilder.SetSelectStatement(new FirstSingleMaybeDefaultStatement(true, fro.ReturnDefaultWhenEmpty));
        }
        else if (resultOperator is SingleResultOperator sro)
        {
            CommandBuilder.SetSelectStatement(new FirstSingleMaybeDefaultStatement(false, sro.ReturnDefaultWhenEmpty));
        }
        else if (resultOperator is LastResultOperator)
        {
            throw new NotSupportedException("Linq methods Last and LastOrDefault are not supported.");
        }
        else if (resultOperator is GroupResultOperator)
        {
            throw new NotSupportedException("Grouping is not supported in current version.");
        }
        else if (resultOperator is DistinctResultOperator)
        {
            CommandBuilder.SetIsDistinct(true);
        }
        else if (resultOperator is AnyResultOperator)
        {
            CommandBuilder.SetSelectStatement(new SelectAnyStatement());
        }
        else if (resultOperator is SkipResultOperator skro)
        {
            CommandBuilder.Skip = SkipTakeTranslator.GetSkipOrTakeValue(skro.Count, true);
        }
        else if (resultOperator is TakeResultOperator tro)
        {
            CommandBuilder.Take = SkipTakeTranslator.GetSkipOrTakeValue(tro.Count, false);
        }

        base.VisitResultOperator(resultOperator, queryModel, index);
    }

    protected override void VisitResultOperators(ObservableCollection<ResultOperatorBase> resultOperators, QueryModel queryModel)
    {
        base.VisitResultOperators(resultOperators, queryModel);
    }

    public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
    {
        CommandBuilder.SetSelectClause(selectClause);

        base.VisitSelectClause(selectClause, queryModel);
    }
}