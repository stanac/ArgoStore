using System;
using System.Collections.Generic;

namespace ArgoStore.Statements
{
    internal class TopStatement
    {
        public Type TypeFrom { get; }
        public Type TypeTo { get; }
        public SelectStatement SelectStatement { get; }
        public bool IsAnyQuery { get; private set; }
        public bool IsCountQuery { get; private set; }
        public string TenantId { get; }
        
        public TopStatement(WhereStatement where, CalledByMethods method, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));

            TenantId = tenantId;

            var selectElements = new List<SelectStatementElement>
            {
                SelectStatementElement.CreateWithStar(where.TargetType)
            };

            SelectStatement = new SelectStatement(where, where.TargetType, where.TargetType, selectElements, null, method);
            TypeFrom = where.TargetType;
            TypeTo = where.TargetType;
        }

        public TopStatement(SelectStatement select, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));

            TenantId = tenantId;

            SelectStatement = select ?? throw new ArgumentNullException(nameof(select));
            TypeFrom = select.TypeFrom ?? throw new ArgumentException("TypeFrom not set", nameof(select));
            TypeTo = select.TypeTo ?? throw new ArgumentException("TypeTo not set", nameof(select));
        }

        public TopStatement(Type entityType, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));

            TenantId = tenantId;

            TypeFrom = entityType;
            TypeTo = entityType;
        }

        public static TopStatement Create(Statement statement, string tenantId)
        {
            if (statement is null) throw new ArgumentNullException(nameof(statement));
            if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));

            if (statement is SelectStatement ss)
            {
                return new TopStatement(ss, tenantId);
            }
            
            if (statement is WhereStatement ws)
            {
                return new TopStatement(ws, CalledByMethods.Select, tenantId); // todo: check CalledByMethods.Select in other methods
            }

            if (statement is SelectCountStatement cq)
            {
                if (cq.Where != null)
                {
                    return new TopStatement(cq.Where, CalledByMethods.Count, tenantId)
                    {
                        IsCountQuery = true
                    };
                }
                
                if (cq.SubQuery != null)
                {
                    return new TopStatement(cq.SubQuery, tenantId)
                    {
                        IsCountQuery = true
                    };
                }

                if (cq.FromType != null)
                {
                    return new TopStatement(cq.FromType, tenantId)
                    {
                        IsCountQuery = true
                    };
                }
            }

            if (statement is SelectExistsStatement eq)
            {
                if (eq.Where != null)
                {
                    return new TopStatement(eq.Where, CalledByMethods.Count, tenantId)
                    {
                        IsAnyQuery = true
                    };
                }

                if (eq.SubQuery != null)
                {
                    return new TopStatement(eq.SubQuery, tenantId)
                    {
                        IsAnyQuery = true
                    };
                }

                if (eq.FromType != null)
                {
                    return new TopStatement(eq.FromType, tenantId)
                    {
                        IsAnyQuery = true
                    };
                }
            }

            throw new ArgumentException($"Cannot create {nameof(TopStatement)} from {statement.GetType().FullName}", nameof(statement));
        }

        public void SetAliases()
        {
            if (SelectStatement != null)
            {
                SelectStatement.SetAliases(0);
                SelectStatement.SetSubQueryAliases(null);
            }
        }
    }
}