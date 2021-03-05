using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ArgoStore.Helpers
{
    internal static class TypeHelpers
    {
        public static bool IsCollectionType(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            return type.IsArray || TypeIsEnumerableOfT(type) || TypeImplementsIEnumerableOfT(type);
        }

        public static Type GetCollectionElementType(Type collectionType)
        {
            if (collectionType is null) throw new ArgumentNullException(nameof(collectionType));

            if (collectionType.IsArray) return collectionType.GetElementType();

            if (TypeIsEnumerableOfT(collectionType) || TypeImplementsIEnumerableOfT(collectionType))
            {
                return collectionType.GenericTypeArguments.Last();
            }

            throw new ArgumentException($"Type {nameof(collectionType)} is not a generic collection type or typed array");
        }

        public static bool ImeplementsIQueryableGenericInteface(Type t)
        {
            return (t.IsGenericType && t.IsClass && t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryable<>)))
                || (t.IsInterface && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IQueryable<>))
                ;
        }

        public static Type CreateIEnumerableOfType(Type elementType)
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));

            return typeof(IEnumerable<>).MakeGenericType(elementType);
        }

        public static Type GetMemberType(MemberInfo mi)
        {
            if (mi is null) throw new ArgumentNullException(nameof(mi));

            if (mi is PropertyInfo pi)
            {
                return pi.PropertyType;
            }

            if (mi is FieldInfo fi)
            {
                return fi.FieldType;
            }

            throw new NotSupportedException($"Cannot {nameof(GetMemberType)} from {mi.GetType().FullName}");
        }

        private static bool TypeIsEnumerableOfT(Type t) =>
            t.IsGenericType && t.IsInterface && t.GetGenericTypeDefinition() == typeof(IEnumerable<>);

        private static bool TypeImplementsIEnumerableOfT(Type t) =>
            t.IsGenericType && t.GetInterfaces().Any(IsInterfaceTypeGenericIEnumerable);

        private static bool IsInterfaceTypeGenericIEnumerable(Type t) =>
            t.IsInterface && t.IsGenericType && t.GenericTypeArguments.Length == 1 && t.GetGenericTypeDefinition() == typeof(IEnumerable<>);

        public static bool IsAnonymousType(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            return type.GetCustomAttribute<CompilerGeneratedAttribute>(false) != null
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"));
        }
    }
}
