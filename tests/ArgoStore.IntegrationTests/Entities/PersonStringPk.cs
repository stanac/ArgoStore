using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgoStore.IntegrationTests.Entities
{
    public class PersonStringPk
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? BirthYear { get; set; }
        public string EmailAddress { get; set; }
    }
}
