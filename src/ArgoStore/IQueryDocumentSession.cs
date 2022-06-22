using System;
using System.Linq;

namespace ArgoStore
{
    /// <summary>
    /// Read-only document session
    /// </summary>
    public interface IQueryDocumentSession: IDisposable
    {
        IQueryable<T> Query<T>() where T : class, new();
    }
}
