using System.Linq.Expressions;

namespace ArgoStore;

/// <summary>
/// Configuration for specific Entity
/// </summary>
/// <typeparam name="TDocument">Entity type</typeparam>
internal interface IDocumentConfiguration<TDocument> where TDocument : class, new()
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
    /// <returns>Same instance of IDocumentConfiguration</returns>
    IDocumentConfiguration<TDocument> PrimaryKey<TProperty>(Expression<Func<TDocument, TProperty>> selector);

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
    /// <returns>Same instance of IDocumentConfiguration</returns>
    IDocumentConfiguration<TDocument> UniqueIndex<TProperty>(Expression<Func<TDocument, TProperty>> selector);

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
    /// <returns>Same instance of IDocumentConfiguration</returns>
    IDocumentConfiguration<TDocument> NonUniqueIndex<TProperty>(Expression<Func<TDocument, TProperty>> selector);

    /// <summary>
    /// Sets name of table to use
    /// </summary>
    /// <param name="tableName">Name of table to use for document type</param>
    /// <returns>Same instance of IDocumentConfiguration</returns>
    IDocumentConfiguration<TDocument> TableName(string tableName);
}