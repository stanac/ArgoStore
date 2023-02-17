using Remotion.Linq;
using System.Linq.Expressions;
using ArgoStore.Command;
using ArgoStore.Config;

namespace ArgoStore.Implementations;

internal class ArgoStoreQueryProvider : IQueryProvider
{
    private readonly ArgoSession _session;
    internal static bool MeasureExecutionTime { get; set; }
    internal static ArgoActivity? LastActivity { get; private set; }

    public ArgoStoreQueryProvider(ArgoSession session)
    {
        _session = session;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotSupportedException("CreateQuery non generic not supported.");
    }

    public IQueryable<T> CreateQuery<T>(Expression expression)
    {
        return new ArgoStoreQueryable<T>(this, expression);
    }

    public object Execute(Expression expression)
    {
        throw new NotSupportedException("Execute non generic not supported.");
    }

    public TResult Execute<TResult>(Expression expression)
    {
        ArgoActivity? activity = MeasureExecutionTime
            ? new ArgoActivity("QueryProvider.Execute")
            : null;

        TResult result = Execute<TResult>(expression, activity);

        activity?.Stop();
        LastActivity = activity;

        return result;
    }

    internal TResult Execute<TResult>(Expression expression, ArgoActivity? activity)
    {
        ArgoQueryModelVisitor v = VisitAndBuild(expression, activity);
        ArgoCommand cmd = v.CommandBuilder.Build(_session.TenantId, activity);

        ArgoCommandExecutor exec = _session.CreateExecutor();
        TResult? result = (TResult?)exec.Execute(cmd, activity);

        return result!;
    }

    private ArgoQueryModelVisitor VisitAndBuild(Expression expression, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("VisitAndBuild");

        QueryModel query = new ArgoStoreQueryParser().GetParsedQuery(expression);
        DocumentMetadata meta = _session.DocumentTypesMetaMap[query.MainFromClause.ItemType];
        
        ArgoQueryModelVisitor v = new(meta, activity);
        v.VisitQueryModel(query);

        ca?.Stop();

        return v;
    }
}