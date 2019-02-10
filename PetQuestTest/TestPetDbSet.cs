using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetQuest2.Models;

namespace PetQuest2.Tests
{
    class TestPetDbSet : TestDbSet<Pet>
    {
        public override Pet Find(params object[] keyValues)
        {
            return this.SingleOrDefault(pet => pet.ID == (int)keyValues.Single());
        }
    }
}
