using System.Linq.Expressions;

namespace ArgoStore;

public interface IArgoDocumentSession : IArgoQueryDocumentSession
{
    void Insert<T>(params T[] documents) where T : class, new();
    void DeleteById<T>(params object[] documentId);
    void Delete(params object[] document);
    void DeleteWhere<T>(Expression<Func<T, bool>> predicate) where T : class, new();
    void SaveChanges();
    void DiscardChanges();
}