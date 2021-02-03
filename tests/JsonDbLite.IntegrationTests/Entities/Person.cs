using System;
using System.Collections.Generic;

namespace JsonDbLite.IntegrationTests.Entities
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? BirthYear { get; set; }
        public string EmailAddress { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; }
        public DateTimeOffset RegistrationTime { get; set; }
        public DateTime? CackeDay { get; set; }
    }
}
