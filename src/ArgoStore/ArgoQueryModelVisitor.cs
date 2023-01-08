﻿using System.Collections.ObjectModel;
using ArgoStore.Statements.Select;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
// ReSharper disable RedundantOverriddenMember

namespace ArgoStore;

internal class ArgoQueryModelVisitor : QueryModelVisitorBase
{
    public ArgoCommandBuilder CommandBuilder { get; private set; }
    
    public override void VisitQueryModel(QueryModel queryModel)
    {
        CommandBuilder = new ArgoCommandBuilder(queryModel);
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
        
        base.VisitResultOperator(resultOperator, queryModel, index);
    }

    protected override void VisitResultOperators(ObservableCollection<ResultOperatorBase> resultOperators, QueryModel queryModel)
    {
        base.VisitResultOperators(resultOperators, queryModel);
    }

    public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
    {
        CommandBuilder.SetSelectStatement(selectClause);

        base.VisitSelectClause(selectClause, queryModel);
    }
}