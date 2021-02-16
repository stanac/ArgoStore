using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgoStore
{
    internal interface IDbAccess
    {
        IEnumerable<string> QueryJsonField(string sql);
        IEnumerable<object[]> QueryFields(string sql, Type[] expectedResultTypes);
        IEnumerable<object> QueryField(string sql);
        Task<IEnumerable<string>> QueryJsonFieldAsync(string sql);
    }
}
