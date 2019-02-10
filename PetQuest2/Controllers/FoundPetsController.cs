using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PetQuest2.Models;
using Microsoft.AspNet.Identity;
using System.Security.Principal;

namespace PetQuest2.Controllers
{
    public class FoundPetsController : ApiController
    {
        private FoundPetDBContext db = new FoundPetDBContext();
        private PetDBContext db2 = new PetDBContext();
        

        // GET: api/FoundPets
        public IQueryable<FoundPet> GetFoundPets1()
        {
            return db.FoundPets;
        }

        // GET: api/FoundPets/5
        [ResponseType(typeof(FoundPet))]
        public IHttpActionResult GetFoundPet(int id)
        {
            FoundPet foundPet = db.FoundPets.Find(id);
            if (foundPet == null)
            {
                return NotFound();
            }

            return Ok(foundPet);
        }

        // PUT: api/FoundPets/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutFoundPet(int id, FoundPet foundPet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != foundPet.ID)
            {
                return BadRequest();
            }

            db.Entry(foundPet).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoundPetExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/FoundPets
        [ResponseType(typeof(FoundPet))]
        public IHttpActionResult PostFoundPet(FoundPet foundPet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Pet pet = db2.Pets.Find(foundPet.PetID);
            Pet pet = db2.Pets.Single(p => p.ID == foundPet.PetID);
            if (pet == null)
            {
                return NotFound();
            }
            
            
            foundPet.FounderID = User.Identity.GetUserId();
            foundPet.FoundDate = DateTime.Now;
            db.FoundPets.Add(foundPet);
            
            db.SaveChanges();
            pet.FoundID = foundPet.ID;
            db2.SaveChanges();
            return CreatedAtRoute("DefaultApi", new { id = foundPet.ID }, foundPet);
        }

        // DELETE: api/FoundPets/5
        [ResponseType(typeof(FoundPet))]
        public IHttpActionResult DeleteFoundPet(int id)
        {
            FoundPet foundPet = db.FoundPets.Find(id);
            if (foundPet == null)
            {
                return NotFound();
            }

            db.FoundPets.Remove(foundPet);
            db.SaveChanges();

            return Ok(foundPet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FoundPetExists(int id)
        {
            return db.FoundPets.Count(e => e.ID == id) > 0;
        }
    }
}