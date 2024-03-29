﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

// ReSharper disable RedundantNullableFlowAttribute

namespace ArgoStore;

/// <summary>
/// Argo Document Store Configuration
/// </summary>
public interface IArgoStoreConfiguration
{
    /// <summary>
    /// Sets connection string for store
    /// </summary>
    /// <param name="connectionString">SQLite connection string</param>
    void ConnectionString([DisallowNull] string connectionString);

    /// <summary>
    /// Sets Factory of LoggerFactory
    /// </summary>
    /// <param name="loggerFactoryFactory">Func&lt;ILoggerFactory&gt;</param>
    void SetLogger(Func<ILoggerFactory> loggerFactoryFactory);

    /// <summary>
    /// Registers document type without any additional options
    /// </summary>
    /// <typeparam name="TDocument">Document type</typeparam>
    /// <returns>Same instance of <see cref="IDocumentConfiguration{TDocument}"/></returns>
    IDocumentConfiguration<TDocument> RegisterDocument<TDocument>() where TDocument : class, new();

    /// <summary>
    /// Registers document type with options for document name, indexes, ...
    /// </summary>
    /// <typeparam name="TDocument">Document type</typeparam>
    /// <returns>Same instance of <see cref="IDocumentConfiguration{TDocument}"/></returns>
    IDocumentConfiguration<TDocument> RegisterDocument<TDocument>([DisallowNull] Action<IDocumentConfiguration<TDocument>> configure) where TDocument : class, new();
}