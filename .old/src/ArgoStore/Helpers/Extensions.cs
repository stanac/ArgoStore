﻿using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ArgoStore.Helpers;

internal static class Extensions
{
    // TODO: validate list
    private static readonly Type[] _supportedPrimitiveTypes =
    {
        typeof(int), typeof(uint), typeof(long), typeof(ulong),
        typeof (byte), typeof(short), typeof(ushort),
        typeof(float), typeof(double), typeof(decimal),
        typeof(char), typeof(string), typeof(Guid), 
        typeof(DateTime), typeof(DateTimeOffset)
#if !NETSTANDARD
        ,typeof(DateOnly)
#endif
    };

    private static readonly Type[] _dateTypes =
    {
        typeof(DateTime), typeof(DateTimeOffset)
#if !NETSTANDARD
        , typeof(DateOnly)
#endif
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
        return type.Name.Contains("AnonymousType")
               && type.Namespace == null
               && type.IsSealed
               && type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
    }

    public static bool IsNullableType(this Type type, out Type? innerType)
    {
        if (type.IsGenericType
            && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            innerType = type.GetGenericArguments()[0];
            return true;
        }

        innerType = null;
        return false;
    }

    public static bool HasPublicGetter(this PropertyInfo pi)
    {
        if (pi == null) throw new ArgumentNullException(nameof(pi));

        if (!pi.CanRead)
        {
            return false;
        }

        MethodInfo? getter = pi.GetMethod;

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

        MethodInfo? setter = pi.SetMethod;

        if (setter == null) return false;

        return setter.IsPublic;
    }

    public static bool HasPublicGetAndSet(this PropertyInfo pi) => pi.HasPublicGetter() && pi.HasPublicSetter();

    public static bool IsSupportedPrimitiveType(this Type t)
    {
        return _supportedPrimitiveTypes.Contains(t);
    }

    public static bool IsTypeCollectionOfSupportedPrimitiveType(this Type type)
    {
        return type.IsTypeCollection()
               && type.GetCollectionTypeArgument().IsSupportedPrimitiveType();
    }

    public static Type GetCollectionTypeArgument(this Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType()!;
        }

        return type.GetGenericArguments()[0];
    }

    public static bool IsTypeCollection(this Type type)
    {
        if (type.IsArray && type.GetArrayRank() == 1) return true;

        if (type.IsClass && type.IsGenericType && type.GenericTypeArguments.Length == 1)
        {
            Type gen = type.GetGenericTypeDefinition()!;
            return gen.GetInterfaces().Contains(typeof(IEnumerable));
        }

        return false;
    }

    public static bool IsTypeADateType(this Type type)
    {
        if (type.IsNullableType(out Type? t))
        {
            return t!.IsTypeADateType();
        }

        return _dateTypes.Contains(type);
    }

    public static string? FormatAsIso8601DateTimeString(this object? o)
    {
        if (o is null) return null;

        DateTimeOffset dto;

        if (o is DateTime dt)
        {
            dto = dt;
        }
        else if (o is DateTimeOffset d)
        {
            dto = d;
        }
#if !NETSTANDARD
        else if (o is DateOnly dOnly)
        {
            return dOnly.ToString("yyyy-MM-dd");
        }
#endif
        else
        {
            throw new ArgumentOutOfRangeException(nameof(o), $"Unexpected type {o.GetType().Name}");
        }

        return dto.ToString("O");
    }
}