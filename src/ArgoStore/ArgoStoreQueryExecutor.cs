using Remotion.Linq;

namespace ArgoStore;

internal class ArgoStoreQueryExecutor : IQueryExecutor
{
    public T ExecuteScalar<T>(QueryModel queryModel)
    {
        throw new NotImplementedException();
    }

    public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
    {
        throw new NotImplementedException();
    }
}