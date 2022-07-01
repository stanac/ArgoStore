using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArgoStore.Configurations
{
    internal class EntityIndexMetadata
    {
        public EntityIndexMetadata(bool unique, IReadOnlyList<string> propertyNames, Type entityType)
        {
            Unique = unique;
            PropertyNames = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));

            if (propertyNames.Count == 0)
            {
                throw new ArgumentException("Collection cannot be empty", nameof(propertyNames));
            }

            EnsurePropertyNamesAreUnique();
            EnsurePropertiesExist();
        }

        public bool Unique { get; }
        public IReadOnlyList<string> PropertyNames { get; }
        public Type EntityType { get; }

        public bool HasSameProperties(EntityIndexMetadata other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            string props1 = string.Join("|||?|||", PropertyNames.Select(x => x.ToLower()).OrderBy(x => x));
            string props2 = string.Join("|||?|||", other.PropertyNames.Select(x => x.ToLower()).OrderBy(x => x));

            return props1 == props2;
        }

        public void EnsurePropertyNamesAreUnique()
        {
            if (PropertyNames.Count != PropertyNames.Distinct().Count())
            {
                throw new InvalidOperationException($"{this} has duplicate columns");
            }
        }

        public void EnsurePropertiesExist()
        {
            PropertyInfo[] props = EntityType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanRead && x.CanWrite)
                .ToArray();

            foreach (string prop in PropertyNames)
            {
                if (props.All(x => x.Name != prop))
                {
                    throw new InvalidOperationException($"Property `{prop}` not found on type `{EntityType.FullName}`");
                }
            }
        }

        public override string ToString() => $"{(Unique ? "Unique" : "NonUnique")} index on {EntityType.Name} ('{string.Join("', '", PropertyNames)}')";
    }
}
