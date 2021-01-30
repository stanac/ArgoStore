using System;
using System.Linq;

namespace JsonDbLite
{
    public interface IQueryDocumentSession: IDisposable
    {
        IQueryable<T> Query<T>() where T : class, new();
    }
}
