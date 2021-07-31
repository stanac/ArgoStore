using System;

namespace ArgoStore.Helpers
{
    public static class RandomString
    {
        private static readonly Random _rand = new Random();
        private const string Chars = "qwertyuiopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM";

        public static string Next() => Next(6);

        public static string Next(int length)
        {
            if (length < 1) throw new ArgumentException("Value cannot be < 1", nameof(length));

            char[] c = new char[length];

            for (int i = 0; i < length; i++)
            {
                c[i] = Chars[_rand.Next(Chars.Length)];
            }

            return new string(c);
        }
    }
}
