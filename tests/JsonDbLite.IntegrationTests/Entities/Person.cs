using System;

namespace JsonDbLite.IntegrationTests.Entities
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int BirthYear { get; set; }
    }
}
