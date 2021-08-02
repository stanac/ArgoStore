using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore
{
    internal interface IStatementToSqlTranslator
    {
        ArgoSqlCommand CreateCommand(TopStatement statement);
    }

    internal class ArgoSqlCommand
    {
        public string CommandText { get; set; }
        public ArgoSqlParameterCollection Parameters { get; } = new ArgoSqlParameterCollection();
        public string AddParameterAndGetParameterName(object value) => Parameters.AddParameterAndGetName(value);

        public void SetRandomParametersPrefix()
        {
            string prefix = RandomString.Next();

            foreach (ArgoSqlParameter p in Parameters)
            {
                string oldParamName = p.Name;
                p.NamePrefix = prefix;
                CommandText = CommandText.Replace(oldParamName, p.Name);
            }
        }

        public void LockPrefix()
        {
            foreach (ArgoSqlParameter p in Parameters)
            {
                p.LockPrefix();
            }
        }

        public bool ArePrefixLocked() => Parameters.Count > 0 && Parameters.Any(x => x.PrefixLocked);

        public SqliteCommand CreateCommand(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));

            if (string.IsNullOrWhiteSpace(CommandText))
            {
                throw new InvalidOperationException("CommandText property not set");
            }

            SqliteCommand cmd = new SqliteCommand(CommandText);
            
            foreach (ArgoSqlParameter p in Parameters)
            {
                cmd.Parameters.AddWithValue(p.Name, p.Value);
            }

            cmd.Parameters.AddWithValue("$__tenant_id__", tenantId);

            return cmd;
        }
    }

    internal class ArgoSqlParameter
    {
        private string _namePrefix;

        public object Value { get; }
        public int OrderNumber { get; }
        public string Name => $"$p{NamePrefix}___p___{OrderNumber}";
        public bool PrefixLocked { get; private set; }
        public string NamePrefix
        {
            get => _namePrefix;
            set
            {
                if (PrefixLocked) throw new InvalidOperationException("Prefix is locked");

                _namePrefix = value;
            }
        }
        
        public ArgoSqlParameter(int orderNumber, object value)
        {
            OrderNumber = orderNumber;
            Value = value;
        }

        public void LockPrefix()
        {
            PrefixLocked = true;
        }
    }

    internal class ArgoSqlParameterCollection : ICollection<ArgoSqlParameter>
    {
        private readonly List<ArgoSqlParameter> _items = new List<ArgoSqlParameter>();

        public IEnumerator<ArgoSqlParameter> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(ArgoSqlParameter item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _items.Add(item);
        }

        public string AddParameterAndGetName(object value)
        {
            int next = GetNextOrderNumber();
            ArgoSqlParameter item = new ArgoSqlParameter(next, value);
            Add(item);
            return item.Name;
        }

        public int GetNextOrderNumber()
        {
            if (Count == 0)
            {
                return 0;
            }

            return this.OrderByDescending(x => x.OrderNumber).Last().OrderNumber + 1;
        }

        public void Clear() => _items.Clear();

        public bool Contains(ArgoSqlParameter item) => _items.Contains(item);

        public void CopyTo(ArgoSqlParameter[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public bool Remove(ArgoSqlParameter item) => _items.Remove(item);

        public int Count => _items.Count;
        public bool IsReadOnly => false;
    }
}