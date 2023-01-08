﻿using Remotion.Linq;
using System.Linq.Expressions;
using ArgoStore.Command;

namespace ArgoStore.Implementations;

internal class ArgoStoreQueryProvider : IQueryProvider
{
    private readonly ArgoSession _session;

    public ArgoStoreQueryProvider(ArgoSession session)
    {
        _session = session;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotSupportedException("62627b8bf3c6");
    }

    public IQueryable<T> CreateQuery<T>(Expression expression)
    {
        return new ArgoStoreQueryable<T>(_session, this, expression);
    }

    public object Execute(Expression expression)
    {
        throw new NotSupportedException("df53fa2a3fea");
    }

    public TResult Execute<TResult>(Expression expression)
    {
        ArgoQueryModelVisitor v = VisitAndBuild(expression);
        ArgoCommand cmd = v.CommandBuilder.Build(_session.DocumentTypes, _session.TenantId);

        ArgoCommandExecutor exec = _session.CreateExecutor();
        TResult result = (TResult)exec.Execute(cmd);

        return result;
    }

    internal ArgoQueryModelVisitor VisitAndBuild(Expression expression)
    {
        QueryModel query = new ArgoStoreQueryParser().GetParsedQuery(expression);

        ArgoQueryModelVisitor v = new();
        v.VisitQueryModel(query);

        return v;
    }
}