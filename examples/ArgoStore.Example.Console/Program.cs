using System;
using System.IO;

namespace ArgoStore.Example.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            string dbFilePath = GetDbFilePath();

            var store = new DocumentStore(dbFilePath, createEntityTablesOnTheFly: true);

            IDocumentSession session = store.CreateSession();

            throw new NotImplementedException();
        }

        private static string GetDbFilePath()
        {
            string tempDir = Path.GetTempPath();
            return Path.Combine(tempDir, $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.sqlite");
        }
    }
}
