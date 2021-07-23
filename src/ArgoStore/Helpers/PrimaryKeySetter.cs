using System;

namespace ArgoStore.Helpers
{
    internal static class PrimaryKeySetter
    {
        // todo: replace reflection to improve performance

        public static void SetPrimaryKey(EntityMetadata entityMeta, object entity, out string stringId)
        {
            if (entityMeta == null) throw new ArgumentNullException(nameof(entityMeta));
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            stringId = Guid.NewGuid().ToString();

            object value = entityMeta.PrimaryKeyProperty.GetValue(entity);

            if (IsValueDefault(value, entityMeta.PrimaryKeyProperty.PropertyType))
            {
                if (entityMeta.PrimaryKeyProperty.PropertyType == typeof(string))
                {
                    entityMeta.PrimaryKeyProperty.SetValue(entity, stringId);
                }
                else if (entityMeta.PrimaryKeyProperty.PropertyType == typeof(Guid))
                {
                    object keyValue = Guid.Parse(stringId);
                    entityMeta.PrimaryKeyProperty.SetValue(entity, keyValue);
                }
                else
                {
                    stringId = value.ToString();
                }
            }
            else
            {
                stringId = value.ToString();
            }
        }

        public static bool DoesPrimaryKeyHaveDefaultValue(EntityMetadata entityMeta, object entity)
        {
            if (entityMeta == null) throw new ArgumentNullException(nameof(entityMeta));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            object pkValue = entityMeta.PrimaryKeyProperty.GetValue(entity);

            if (pkValue == null)
            {
                return true;
            }

            if (entityMeta.PrimaryKeyProperty.PropertyType == typeof(Guid))
            {
                return (Guid) pkValue != Guid.Empty;
            }

            if (entityMeta.PrimaryKeyProperty.PropertyType == typeof(int))
            {
                return (int) pkValue == 0;
            }

            if (entityMeta.PrimaryKeyProperty.PropertyType == typeof(long))
            {
                return (long)pkValue == 0L;
            }

            throw new InvalidOperationException($"PK type {entityMeta.PrimaryKeyProperty.PropertyType} not supported");
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
