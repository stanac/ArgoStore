using System;
using System.Collections.Generic;
using System.Linq;

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
                return collectionType.GetInterfaces().First(TypeIsEnumerableOfT).GetGenericArguments().First();
            }

            throw new ArgumentException($"Type {nameof(collectionType)} is not a generic collection type or typed array");
        }

        public static Type CreateIEnumerableOfType(Type elementType)
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));

            return typeof(IEnumerable<>).MakeGenericType(elementType);
        }

        private static bool TypeIsEnumerableOfT(Type t) =>
            t.IsGenericType && t.IsInterface && t.GetGenericTypeDefinition() == typeof(IEnumerable<>);

        private static bool TypeImplementsIEnumerableOfT(Type t) =>
            t.IsGenericType && t.GetInterfaces().Any(IsInterfaceTypeGenericIEnumerable);

        private static bool IsInterfaceTypeGenericIEnumerable(Type t) =>
            t.IsInterface && t.IsGenericType && t.GenericTypeArguments.Length == 1 && t.GetGenericTypeDefinition() == typeof(IEnumerable<>);
    }
}
