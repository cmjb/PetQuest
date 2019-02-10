using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetQuest2.Controllers;
using PetQuest2.Models;
using System.Web.Http.Results;
using System.Net;

namespace PetQuest2.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var controller = new PetsController(new PetQuest2.Tests.TestPetQuestContext());

            var pet = GetDemoPet();

            var result = controller.PostPet(pet) as CreatedAtRouteNegotiatedContentResult<Pet>;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteName, "DefaultApi");
            Assert.AreEqual(result.RouteValues["id"], result.Content.ID);
            Assert.AreEqual(result.Content.Name, pet.Name);
        }

        [TestMethod]
        public void PostPet_ShouldReturnSamePet()
        {
            var controller = new PetsController(new TestPetQuestContext());

            var item = GetDemoPet();

            var result = controller.PostPet(item) as CreatedAtRouteNegotiatedContentResult<Pet>;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteName, "DefaultApi");
            Assert.AreEqual(result.RouteValues["id"], result.Content.ID);
            Assert.AreEqual(result.Content.Name, item.Name);
        }

        [TestMethod]
        public void PutPet_ShouldReturnStatusCode()
        {
            var controller = new PetsController(new TestPetQuestContext());

            var item = GetDemoPet();

            var result = controller.PutPet(item.ID, item) as StatusCodeResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [TestMethod]
        public void PutPet_ShouldFail_WhenDifferentID()
        {
            var controller = new PetsController(new TestPetQuestContext());

            var badresult = controller.PutPet(999, GetDemoPet());
            Assert.IsInstanceOfType(badresult, typeof(BadRequestResult));
        }

        [TestMethod]
        public void GetPet_ShouldReturnPetWithSameID()
        {
            var demo = GetDemoPet();
            var context = new TestPetQuestContext();
            context.Pets.Add(demo);

            var controller = new PetsController(context);
            var result = controller.GetPet(3) as OkNegotiatedContentResult<Pet>;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Content.ID);
        }

        [TestMethod]
        public void GetPets_ShouldReturnAllPets()
        {
            var context = new TestPetQuestContext();
            context.Pets.Add(new Pet { Name = "Demo1", Location = "Demo" });
            context.Pets.Add(new Pet { Name = "Demo2", Location = "Demo" });
            context.Pets.Add(new Pet { Name = "Demo3", Location = "Demo" });

            var controller = new PetsController(context);
            var result = controller.GetPets() as TestPetDbSet;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Local.Count);
        }

        [TestMethod]
        public void DeleteProduct_ShouldReturnOK()
        {
            var context = new TestPetQuestContext();
            var item = GetDemoPet();
            context.Pets.Add(item);

            var controller = new PetsController(context);
            var result = controller.DeletePet(3) as OkNegotiatedContentResult<Pet>;

            Assert.IsNotNull(result);
            Assert.AreEqual(item.ID, result.Content.ID);
        }

        Pet GetDemoPet()
        {
            return new Pet() { ID = 3, Name = "Demo4", Location = "Demo" };
        }
    }
}
