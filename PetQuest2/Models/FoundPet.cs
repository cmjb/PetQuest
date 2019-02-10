using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PetQuest2.Models
{
    public class FoundPet
    {
        public int ID { get; set; }
        public string FounderID { get; set; }
        public string LocationFound { get; set; }
        public int PetID { get; set; }
        public DateTime FoundDate { get; set; }
    }
    public class FoundPetDBContext : DbContext
    {
        public FoundPetDBContext() 
        {

        }

        public System.Data.Entity.DbSet<PetQuest2.Models.FoundPet> FoundPets { get; set; }
        
    }
}