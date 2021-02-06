using System.Linq;

namespace ArgoStore
{
    public interface IArgoStoreQueryable<T> : IQueryable<T>
    {
        string ToSqlString();
    }
}
