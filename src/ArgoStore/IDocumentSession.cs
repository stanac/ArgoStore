using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ArgoStore
{
    public interface IDocumentSession : IQueryDocumentSession
    {
        /// <summary>
        /// Inserts entities, changes applied on <see cref="SaveChanges"/> or <see cref="SaveChangesAsync"/>
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entities">Entities to insert</param>
        void Insert<T>(params T[] entities);

        /// <summary>
        /// Updates entities, changes applied on <see cref="SaveChanges"/> or <see cref="SaveChangesAsync"/>
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entities">Entities to update</param>
        void Update<T>(params T[] entities);

        /// <summary>
        /// Deletes entities, changes applied on <see cref="SaveChanges"/> or <see cref="SaveChangesAsync"/>
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entities">Entities to delete</param>
        void Delete<T>(params T[] entities);

        /// <summary>
        /// Upserts entities (insert or update), changes applied on <see cref="SaveChanges"/> or <see cref="SaveChangesAsync"/>
        /// If entity exists it will be updated, otherwise inserted
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entities">Entities to save</param>
        void Store<T>(params T[] entities);

        /// <summary>
        /// Deletes entities that match predicate, changes applied on <see cref="SaveChanges"/> or <see cref="SaveChangesAsync"/>
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="predicate">Predicate, condition for deleting</param>
        void DeleteWhere<T>(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Saves all changes in this session
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Saves all changes in this session asynchronously
        /// </summary>
        /// <returns>Task to await</returns>
        Task SaveChangesAsync();
    }
}
