using System.Reflection;
using System.Runtime.CompilerServices;

namespace ArgoStore.Helpers;

internal static class Extensions
{
    public static bool IsCaseSensitive(this StringComparison sc)
    {
        return !(
            sc is StringComparison.OrdinalIgnoreCase
            or StringComparison.CurrentCultureIgnoreCase
            or StringComparison.InvariantCultureIgnoreCase
            );
    }

    public static bool IsAnonymousType(this Type type)
    {
        return type.Name.Contains("AnonymousType")
               && type.Namespace == null
               && type.IsSealed
               && type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
    }

    public static bool IsNullableType(this Type type)
    {
        return type.IsGenericType
               && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

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