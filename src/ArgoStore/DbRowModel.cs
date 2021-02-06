using System;

namespace ArgoStore
{
    internal class DbRowModel
    {
        public TenantId TenantId { get; set; }
        public string Key { get; set; }
        public string Json { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset LasUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
