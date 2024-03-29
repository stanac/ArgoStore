﻿using System.Reflection;
using System.Text;

namespace ArgoStore.Config;

internal class DocumentIndexMetadata
{
    public DocumentIndexMetadata(bool unique, IReadOnlyList<string> propertyNames, Type entityType)
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

    public string GetIndexName()
    {
        StringBuilder sb = new StringBuilder();

        if (Unique) sb.Append("UX_");
        else sb.Append("IX_");

        sb.Append(EntityType.Name);

        foreach (string p in PropertyNames)
        {
            sb.Append("_").Append(p);
        }

        return sb.ToString();
    }

    public override string ToString() => $"{(Unique ? "Unique" : "NonUnique")} index on {EntityType.Name} ('{string.Join("', '", PropertyNames)}')";
}