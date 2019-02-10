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
using System.Web;
using Microsoft.AspNet.Identity;


namespace PetQuest2.Controllers
{
    public class PetsController : ApiController
    {
        private IPetQuestContext db = new PetDBContext();
        private PetDBContext db2 = new PetDBContext();
        public PetsController() { }

        public PetsController(IPetQuestContext context)
        {
            db = context;
        }
        // GET: api/Pets
        [Authorize]
        public IQueryable<Pet> GetPets()
        {        
            return db.Pets;
        }
 

        // GET: api/Pets/5
        [Authorize]
        [ResponseType(typeof(Pet))]
        public IHttpActionResult GetPet(int id)
        {
            Pet pet = db.Pets.Find(id);
            if (pet == null)
            {
                return NotFound();
            }

            return Ok(pet);
        }

        // PUT: api/Pets/5
        [Authorize]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPet(int id, Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pet.ID)
            {
                return BadRequest();
            }

            
            if (User.Identity.GetUserId() != null)
            {
                var rpet = db2.Pets.Find(pet.ID);

                if (User.IsInRole("Administrator") == false && User.Identity.GetUserId().Equals(rpet.OwnerID) == false)
                {
                    return BadRequest("test" + User.Identity.GetUserId() + " " + rpet.OwnerID + User.IsInRole("Administrator"));
                }


                pet.PublishedDate = rpet.PublishedDate;
                pet.OwnerID = rpet.OwnerID;
                pet.FoundID = rpet.FoundID;
            }
            //rpet.Dispose(); 
           // db.Entry(pet).State = EntityState.Modified;
            db.MarkAsModified(pet);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(id))
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

        // POST: api/Pets
        [Authorize]
        [ResponseType(typeof(Pet))]
        public IHttpActionResult PostPet(Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            
            }

            // Set the date of published pet to now.
            pet.PublishedDate = DateTime.Now;
            pet.OwnerID = User.Identity.GetUserId();
            db.Pets.Add(pet);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = pet.ID }, pet);
        }

        // DELETE: api/Pets/5
        [Authorize]
        [ResponseType(typeof(Pet))]
        public IHttpActionResult DeletePet(int id)
        {
            Pet pet = db.Pets.Find(id);
            if (pet == null)
            {
                return NotFound();
            }

            // This is here to allow for Testing.
            if (User.Identity.GetUserId() != null) 
            {
                if (User.Identity.GetUserId().Equals(pet.OwnerID) != true || !User.IsInRole("Administrator") != true)
                {
                    return BadRequest();
                }
            }

            db.Pets.Remove(pet);
            db.SaveChanges();

            return Ok(pet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PetExists(int id)
        {
            return db.Pets.Count(e => e.ID == id) > 0;
        }
    }
}