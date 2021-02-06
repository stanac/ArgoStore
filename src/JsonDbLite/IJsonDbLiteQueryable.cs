using System.Linq;

namespace JsonDbLite
{
    public interface IJsonDbLiteQueryable<T> : IQueryable<T>
    {
        string ToSqlString();
    }
}
