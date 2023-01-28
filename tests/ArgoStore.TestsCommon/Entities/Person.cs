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
        public int OddNumberOfPorts { get; set; }
        public string EmailAddress { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<int> PortList { get; set; }
        public List<string> Roles { get; set; }
        public string[] RolesArray { get; set; }
        public IList<string> RolesIList { get; set; }
        public IReadOnlyList<string> RolesIReadOnlyList { get; set; }
        public IEnumerable<string> RolesIEnumerable { get; set; }
        public DateTimeOffset RegistrationTime { get; set; }
        public DateTime? CakeDay { get; set; }
        public PersonTypes Type { get; set; }
        public string NickName { get; set; }
        public PersonContact PrimaryContact { get; set; }
        public List<PersonContact> Contacts { get; set; }

        public Person Copy()
        {
            Person p = (Person)MemberwiseClone();
            p.Roles = Roles?.AsEnumerable().ToList();
            return p;
        }

        public Person SetCollections()
        {
            if (Roles != null)
            {
                RolesArray = Roles.ToArray();
                RolesIList = Roles.AsEnumerable().ToList();
                RolesIReadOnlyList = Roles.AsEnumerable().ToList();
                RolesIEnumerable = Roles.AsEnumerable().ToList();
            }

            return this;
        }

        public class PersonContact
        {
            public int ContactType { get; set; }
            public List<ContactInfo> ContactInfos { get; set; }
        }

        public class ContactInfo
        {
            public bool Active { get; set; }
            public string[] Details { get; set; }
        }
    }
}
