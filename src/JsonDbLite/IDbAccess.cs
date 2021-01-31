using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonDbLite
{
    internal interface IDbAccess
    {
        IReadOnlyList<string> QueryJsonField(string sql);
        Task<IReadOnlyList<string>> QueryJsonFieldAsync(string sql);
    }
}
