using System;
using System.Linq.Expressions;

namespace ArgoStore.Configurations
{
    /// <summary>
    /// Configuration for specific Entity
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface IEntityConfiguration<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// Sets primary key
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// config.Entity<Person>().PrimaryKey(p => p.EmailAddress);
        /// ]]>
        /// </example>
        /// <typeparam name="TProperty">Entity type</typeparam>
        /// <param name="selector">Lambda expression to select a single property as primary key</param>
        /// <returns>Same instance of IEntityConfiguration</returns>
        IEntityConfiguration<TEntity> PrimaryKey<TProperty>(Expression<Func<TEntity, TProperty>> selector);

        /// <summary>
        /// Sets unique index
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// config.Entity<Person>().UniqueIndex(p => p.EmailAddress);
        /// config.Entity<Person>().UniqueIndex(p => new { p.EmailAddress, p.Age });
        /// ]]>
        /// </example>
        /// <typeparam name="TProperty">Entity type</typeparam>
        /// <param name="selector">Lambda expression to select a single property as index or new anonymous object with multiple properties</param>
        /// <returns>Same instance of IEntityConfiguration</returns>
        IEntityConfiguration<TEntity> UniqueIndex<TProperty>(Expression<Func<TEntity, TProperty>> selector);

        /// <summary>
        /// Sets non unique index
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// config.Entity<Person>().NonUniqueIndex(p => p.Age);
        /// config.Entity<Person>().NonUniqueIndex(p => new { p.EmailAddress, p.Age });
        /// ]]>
        /// </example>
        /// <typeparam name="TProperty">Entity type</typeparam>
        /// <param name="selector">Lambda expression to select a single property as index or new anonymous object with multiple properties</param>
        /// <returns>Same instance of IEntityConfiguration</returns>
        IEntityConfiguration<TEntity> NonUniqueIndex<TProperty>(Expression<Func<TEntity, TProperty>> selector);
    }
}