using System;

namespace ArgoStore.IntegrationTests.Entities
{
    public class PersonIntPk
    {
        public PersonIntPk()
        {
        }

        public PersonIntPk(PersonIntPk toCopy)
        {
            if (toCopy == null) throw new ArgumentNullException(nameof(toCopy));

            Id = toCopy.Id;
            Name = toCopy.Name;
            BirthYear = toCopy.BirthYear;
            EmailAddress = toCopy.EmailAddress;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? BirthYear { get; set; }
        public string EmailAddress { get; set; }

        public PersonIntPk Copy() => new PersonIntPk(this);
    }
}