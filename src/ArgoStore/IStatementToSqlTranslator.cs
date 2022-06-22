using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArgoStore.Helpers;
using ArgoStore.Statements;
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

        public object Value { get; private set; }
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

        public void AddWildcard(bool atTheBeginning, bool atTheEnd)
        {
            if (Value is string s)
            {
                if (atTheBeginning && atTheEnd)
                {
                    s = $"%{s}%";
                }
                else if (atTheBeginning)
                {
                    s = $"%{s}";
                }
                else if (atTheEnd)
                {
                    s = $"{s}%";
                }

                Value = s;
            }
            else
            {
                throw new InvalidOperationException("Cannot call AddWildcard to non string parameter");
            }
        }

        public void ToUpper()
        {
            if (Value is string s)
            {
                Value = s.ToUpper();
            }
            else
            {
                throw new InvalidOperationException("Cannot call ToUpper to non string parameter");
            }
        }

        public void ToLower()
        {
            if (Value is string s)
            {
                Value = s.ToLower();
            }
            else
            {
                throw new InvalidOperationException("Cannot call ToLower to non string parameter");
            }
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

        public void AddWildcard(string parameterName, bool atTheBeginning, bool atTheEnd)
        {
            ArgoSqlParameter parameter = _items.FirstOrDefault(x => x.Name == parameterName);

            if (parameter == null)
            {
                throw new ArgumentException("No parameter found with given name", nameof(parameterName));
            }

            if (parameter.Value != null)
            {
                parameter.AddWildcard(atTheBeginning, atTheEnd);
            }
        }

        public void ToUpper(string parameterName)
        {
            ArgoSqlParameter parameter = _items.FirstOrDefault(x => x.Name == parameterName);

            if (parameter == null)
            {
                throw new ArgumentException("No parameter found with given name", nameof(parameterName));
            }

            if (parameter.Value != null)
            {
                parameter.ToUpper();
            }
        }

        public void ToLower(string parameterName)
        {
            ArgoSqlParameter parameter = _items.FirstOrDefault(x => x.Name == parameterName);

            if (parameter == null)
            {
                throw new ArgumentException("No parameter found with given name", nameof(parameterName));
            }

            if (parameter.Value != null)
            {
                parameter.ToLower();
            }
        }
    }
}