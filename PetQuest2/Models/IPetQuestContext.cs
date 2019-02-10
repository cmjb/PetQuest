using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PetQuest2.Models
{
    public interface IPetQuestContext : IDisposable
    {
        DbSet<Pet> Pets { get; }
        int SaveChanges();
        void MarkAsModified(Pet item);
    }
}
