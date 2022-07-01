using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArgoStore.Configurations
{
    internal class EntityMetadata
    {
        private static readonly Type[] _supportedPkTypes =
        {
            typeof(int), typeof(long),
            typeof(uint), typeof(ulong),
            typeof(string), typeof(Guid)
        };
        
        public Type EntityType { get; }
        public PropertyInfo PrimaryKeyProperty { get; }
        public IReadOnlyList<EntityIndexMetadata> Indexes { get; } = new List<EntityIndexMetadata>();

        public EntityMetadata(Type entityType)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            PrimaryKeyProperty = GetKeyProperty(entityType);

            EnsurePrimaryKeyTypeIsSupported();
        }

        public EntityMetadata(Type entityType, string keyPropertyName)
            : this(entityType, keyPropertyName, new List<EntityIndexMetadata>())
        {
        }

        public EntityMetadata(Type entityType, string keyPropertyName, IReadOnlyList<EntityIndexMetadata> indexes)
        {
            if (string.IsNullOrWhiteSpace(keyPropertyName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(keyPropertyName));

            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            PrimaryKeyProperty = entityType.GetProperties().FirstOrDefault(x => x.CanRead && x.CanWrite && x.Name == keyPropertyName);
            Indexes = indexes ?? throw new ArgumentNullException(nameof(indexes));

            if (PrimaryKeyProperty == null)
            {
                throw new ArgumentException($"Entity type `{entityType.Name}` does not have a public property named `{keyPropertyName}` with public getter and setter.");
            }

            EnsurePrimaryKeyTypeIsSupported();
            EnsureIndexesAreValid();
        }

        internal static PropertyInfo GetKeyProperty(Type entityType)
        {
            PropertyInfo[] props = entityType.GetProperties();

            List<string> expectedKeyPropertyNames = new ()
            {
                "Id",
                "Key",
                entityType.Name + "Id",
                entityType.Name + "Key"
            };

            List<PropertyInfo> prop = props.Where(x => x.CanRead && x.CanWrite && expectedKeyPropertyNames.Contains(x.Name)).ToList();

            if (prop.Count == 1)
            {
                return prop[0];
            }

            string expectedNames = "`" + string.Join("`, `", expectedKeyPropertyNames) + "`";

            if (prop.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cannot find public property with public getter and setter to use as primary key " +
                    $"for `{entityType.Name}`, looked for {expectedNames}.");
            }

            throw new InvalidOperationException(
                "Found multiple public properties with public getter and setter to use as primary key " +
                $"for `{entityType.Name}`, looked for {expectedNames}.");
        }
        
        private void EnsurePrimaryKeyTypeIsSupported()
        {
            if (!_supportedPkTypes.Contains(PrimaryKeyProperty.PropertyType))
            {
                throw new InvalidOperationException($"Property of type {PrimaryKeyProperty.PropertyType.Name} cannot be used as primary key. " +
                                                    "Following types are supported: int, long, string, Guid");
            }
        }
        
        private void EnsureIndexesAreValid()
        {
            foreach (EntityIndexMetadata index in Indexes)
            {
                int indexesWithSameColumns = Indexes.Count(x => index.HasSameProperties(x));

                if (indexesWithSameColumns > 1)
                {
                    throw new InvalidOperationException("Multiple indexes found with same columns, i.e. " + index);
                }
            }
        }
    }
}
