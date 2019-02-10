using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PetQuest2.Models
{
    public class Pet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Image { get; set; }
        public int FoundID { get; set; }
        public string OwnerID { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
    }

    public class PetDBContext : DbContext, IPetQuestContext
    {
        public PetDBContext() 
        {

        }
        public DbSet<Pet> Pets { get; set; }
        public void MarkAsModified(Pet pet)
        {
            Entry(pet).State = EntityState.Modified;
        }
    }
}