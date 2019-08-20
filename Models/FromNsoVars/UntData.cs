using NSOWater.HotMigration.Models;
using System.Collections.Generic;

namespace NsoGetData.Models
{
    //public class UntData
    //{
    //    public string Ea { get; set; }
    //    public string BuildingId { get; set; }
    //    public List<Poppulation> PoppulationData { get; set; }
    //}

    public class UntData
    {
        public string Ea { get; set; }
        public string BuildingId { get; set; }
        public Poppulation PoppulationData { get; set; }
    }
    public class Poppulation
    {
        public NameTitle? NameTitle { get; set; }
        public string OtherTitle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Relationship? Relationship { get; set; }
        public Sex? Sex { get; set; }
        public BirthDate? BirthDate { get; set; }
        public BirthMonth? BirthMonth { get; set; }
        public BirthYear? BirthYear { get; set; }
        public Age? Age { get; set; }
        public string Nationality { get; set; }
        public Registration? Registration { get; set; }
        public string OtherProvince { get; set; }

    }
}
