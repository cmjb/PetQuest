using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetQuest2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        
        }

        public ActionResult About()
        {
            ViewBag.Message = "PetQuest";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "PetQuest";

            return View();
        }
    }
}
