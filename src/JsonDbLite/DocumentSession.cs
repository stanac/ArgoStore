using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonDbLite
{
    internal class DocumentSession : IDocumentSession
    {
        private static readonly HashSet<Type> _createdEntities = new HashSet<Type>();

        private readonly Configuration _config;

        public DocumentSession(Configuration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _config.EnsureValid();
        }

        public void Dispose()
        {
            // do nothing for now
        }

        public IQueryable<T> Query<T>() where T : class, new()
        {
            return new JsonDbLiteQueryable<T>(new JsonDbLiteQueryProvider(_config));
        }

    }
}
