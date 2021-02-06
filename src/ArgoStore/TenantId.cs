using System;

namespace ArgoStore
{
    /// <summary>
    /// Tenant id, can be either "default" or non empty Guid
    /// </summary>
    public class TenantId
    {
        /// <summary>
        /// Default value for TenantId
        /// </summary>
        public const string DefaultValue = "default";

        private readonly Guid? _id;

        private TenantId() { }

        /// <summary>
        /// Constructor for non default TenantId
        /// </summary>
        /// <param name="id">Non empty Guid</param>
        public TenantId(Guid id)
        {
            if (id == default) throw new ArgumentException($"{nameof(id)} isn't set, it cannot be empty Guid");

            _id = id;
        }

        /// <summary>
        /// Returns True is TenantId has default value, otherwise false
        /// </summary>
        public bool IsDefault => !_id.HasValue;

        /// <summary>
        /// Created default TenantId with value "default"
        /// </summary>
        /// <returns>TenantId with value "default"</returns>
        public static TenantId CreateDefault() => new TenantId();

        /// <summary>
        /// Parses string to TenantId
        /// </summary>
        /// <param name="value">Either "default" or non empty Guid</param>
        /// <returns>Parsed TenantId</returns>
        public static TenantId Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"'{nameof(value)}' cannot be null or whitespace", nameof(value));

            if (value == DefaultValue) return CreateDefault();

            if (Guid.TryParse(value, out Guid g)) return new TenantId(g);

            throw new ArgumentException($"{nameof(value)} must be either \"{default}\" or non empty Guid");
        }

        /// <summary>
        /// Tries to parse string to TenantId
        /// </summary>
        /// <param name="value">Value to parse, must be either "default" or non empty Guid</param>
        /// <param name="id">TenantId if parsing is successful, otherwise null</param>
        /// <returns>True if parsing is successful, otherwise False</returns>
        public static bool TryParse(string value, out TenantId id)
        {
            try
            {
                id = Parse(value);
                return true;
            }
            catch
            {
                id = null;
                return false;
            }
        }

        public override string ToString() => _id.HasValue ? _id.Value.ToString() : DefaultValue;
    }
}
