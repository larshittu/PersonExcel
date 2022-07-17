using System.Collections.Generic;

namespace PersonalExce.Helper.Data.Dto
{
    public class PersonDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public int Age { get; set; }

        public string Sex { get; set; }

        public string Mobile { get; set; }

        public bool Active { get; set; }

        public List<PersonDto> Persons { get; set; }

    }
}
