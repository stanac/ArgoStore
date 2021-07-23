using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArgoStore
{
    internal class EntityMetadata
    {
        public Type EntityType { get; }
        public PropertyInfo PrimaryKeyProperty { get; }
        
        public EntityMetadata(Type entityType, string keyPropertyName)
        {
            if (string.IsNullOrWhiteSpace(keyPropertyName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(keyPropertyName));

            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            PrimaryKeyProperty = entityType.GetProperties().FirstOrDefault(x => x.CanRead && x.CanWrite && x.Name == keyPropertyName);

            if (PrimaryKeyProperty == null)
            {
                throw new ArgumentException($"Entity type `{entityType.Name}` does not have a public property named `{keyPropertyName}` with public getter and setter.");
            }

            EnsurePrimaryKeyTypeIsSupported();
        }

        public EntityMetadata(Type entityType)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            PrimaryKeyProperty = GetKeyProperty(entityType);

            EnsurePrimaryKeyTypeIsSupported();
        }

        private PropertyInfo GetKeyProperty(Type entityType)
        {
            PropertyInfo[] props = entityType.GetProperties();

            List<string> expectedKeyPropertyNames = new ()
            {
                "StringId",
                "Key",
                entityType.Name + "StringId",
                entityType.Name + "Key"
            };

            List<PropertyInfo> prop = props.Where(x => x.CanRead && x.CanWrite && expectedKeyPropertyNames.Contains(x.Name)).ToList();

            if (prop.Count == 1)
            {
                return prop[0];
            }

            string expectedNames = "`" + string.Join("`, `", expectedKeyPropertyNames) + "`";

            throw new InvalidOperationException(
                "Cannot find public property with public getter and setter to use as primary key " +
                $"for `{entityType.Name}`, looked for {expectedNames}.");
        }

        private void EnsurePrimaryKeyTypeIsSupported()
        {
            Type[] supportedTypes =
            {
                typeof(int), typeof(long),
                typeof(string), typeof(Guid)
            };

            if (!supportedTypes.Contains(PrimaryKeyProperty.PropertyType))
            {
                throw new InvalidOperationException($"Property of type {PrimaryKeyProperty.PropertyType.Name} cannot be used as primary key. " +
                                                    "Following types are supported: int, long, string, Guid");
            }
        }
    }
}
