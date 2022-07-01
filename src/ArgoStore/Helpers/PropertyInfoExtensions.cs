using System.Reflection;

namespace ArgoStore.Helpers
{
    internal static class PropertyInfoExtensions
    {
        public static bool HasPublicGetter(this PropertyInfo pi)
        {
            if (pi == null) throw new ArgumentNullException(nameof(pi));

            if (!pi.CanRead)
            {
                return false;
            }

            MethodInfo getter = pi.GetMethod;

            if (getter == null) return false;

            return getter.IsPublic;
        }

        public static bool HasPublicSetter(this PropertyInfo pi)
        {
            if (pi == null) throw new ArgumentNullException(nameof(pi));

            if (!pi.CanRead)
            {
                return false;
            }

            MethodInfo setter = pi.SetMethod;

            if (setter == null) return false;

            return setter.IsPublic;
        }

        public static bool HasPublicGetAndSet(this PropertyInfo pi) => pi.HasPublicGetter() && pi.HasPublicSetter();
    }
}
