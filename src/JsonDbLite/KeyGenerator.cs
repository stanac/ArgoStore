using System;

namespace JsonDbLite
{
    public abstract class KeyGenerator
    {
        public abstract string GenerateNewKey();
    }

    public class GuidKeyGenerator: KeyGenerator
    {
        public override string GenerateNewKey() => Guid.NewGuid().ToString("N");
    }

    public class CombGuidKeyGenerator : KeyGenerator
    {
        public override string GenerateNewKey() => RT.Comb.Provider.Sql.Create().ToString("N");
    }
}
