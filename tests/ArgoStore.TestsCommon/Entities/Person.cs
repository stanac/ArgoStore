using System;
using System.Collections.Generic;
using System.Linq;

namespace ArgoStore.TestsCommon.Entities
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? BirthYear { get; set; }
        public int Points { get; set; }
        public int NumberOfPorts { get; set; }
        public string EmailAddress { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; }
        public DateTimeOffset RegistrationTime { get; set; }
        public DateTime? CakeDay { get; set; }
        public string NickName { get; set; }

        public Person Copy()
        {
            Person p = (Person)MemberwiseClone();
            p.Roles = Roles?.AsEnumerable().ToList();
            return p;
        }
    }
}
