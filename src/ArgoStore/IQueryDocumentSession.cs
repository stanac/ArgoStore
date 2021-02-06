using System;
using System.Linq;

namespace ArgoStore
{
    public interface IQueryDocumentSession: IDisposable
    {
        IQueryable<T> Query<T>() where T : class, new();
    }
}
