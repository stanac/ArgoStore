using System;

namespace ArgoStore.Helpers
{
    internal static class PrimaryKeySetter
    {
        // todo: replace reflection to improve performance

        public static void SetPrimaryKey(EntityMetadata entityMeta, object entity)
        {
            if (entityMeta == null) throw new ArgumentNullException(nameof(entityMeta));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            object value = entityMeta.PrimaryKeyProperty.GetValue(entity);

            if (IsValueDefault(value, entityMeta.PrimaryKeyProperty.PropertyType))
            {
                if (entityMeta.PrimaryKeyProperty.PropertyType == typeof(string))
                {
                    object keyValue = Guid.NewGuid().ToString();
                    entityMeta.PrimaryKeyProperty.SetValue(entity, keyValue);
                }
                else if (entityMeta.PrimaryKeyProperty.PropertyType == typeof(Guid))
                {
                    object keyValue = Guid.NewGuid();
                    entityMeta.PrimaryKeyProperty.SetValue(entity, keyValue);
                }
            }
        }

        public static void EnsurePrimaryKeyAutoIncIsNotSet(EntityMetadata entityMeta, object entity)
        {
            if (entityMeta == null) throw new ArgumentNullException(nameof(entityMeta));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            if (entityMeta.PrimaryKeyProperty.PropertyType == typeof(int))
            {
                int value = (int) entityMeta.PrimaryKeyProperty.GetValue(entity);

                if (value != 0)
                {
                    throw new InvalidOperationException($"Int primary key in `{entityMeta.EntityType.Name}` is set before insert. " +
                                                        "AutoInc keys (int and long) cannot have value other than 0 before insert");
                }
            }
            else  if (entityMeta.PrimaryKeyProperty.PropertyType == typeof(long))
            {
                long value = (long)entityMeta.PrimaryKeyProperty.GetValue(entity);

                if (value != 0)
                {
                    throw new InvalidOperationException($"Int primary key in `{entityMeta.EntityType.Name}` is set before insert. " +
                                                        "AutoInc keys (int and long) cannot have value other than 0 before insert");
                }
            }
        }

        private static bool IsValueDefault(object value, Type keyPropertyPropertyType)
        {
            if (keyPropertyPropertyType == typeof(int))
            {
                return (int) value == 0;
            }

            if (keyPropertyPropertyType == typeof(long))
            {
                return (long)value == 0L;
            }

            if (keyPropertyPropertyType == typeof(string))
            {
                return string.IsNullOrWhiteSpace(value as string);
            }

            if (keyPropertyPropertyType == typeof(Guid))
            {
                return (Guid) value == Guid.Empty;
            }

            throw new InvalidOperationException($"Type {keyPropertyPropertyType.Name} is not supported as primary key");
        }
    }
}
