using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetQuest2.Models;
using System.Data.Entity;

namespace PetQuest2.Tests
{
    class TestPetQuestContext : PetQuest2.Models.IPetQuestContext
    {
        public TestPetQuestContext()
        {
            this.Pets = new TestPetDbSet();
        }

        public DbSet<Pet> Pets { get; set; }

        public int SaveChanges()
        {
            return 0;
        }

        public void MarkAsModified(Pet item) { }
        public void Dispose() { }


        public DbSet<FoundPet> FoundPets
        {
            get { throw new NotImplementedException(); }
        }
    }
}
