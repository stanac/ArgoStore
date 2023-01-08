﻿using System.Reflection;
using System.Runtime.CompilerServices;

namespace ArgoStore;

internal static class Extensions
{
    private static readonly string[] _anonymousTypePrefixes =
    {
        "<>", "VB$"
    };

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
        return type.IsGenericType
               && type.Name.Contains("AnonymousType")
               && type.Namespace == null
               && type.IsSealed
               && type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
    }
}