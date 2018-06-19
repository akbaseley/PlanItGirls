using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PlanItGirls.Models;

namespace PlanItGirls.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

<<<<<<< HEAD
        public ActionResult StartDate (DateTime start)
        {
            planitdbEntities1
        }
    }
}
=======

        public ActionResult TripCreation(DateTime start)
        {
            planitdbEntities1 ORM = new planitdbEntities1();

            ORM.Trips.Add(start);
            ORM.SaveChanges();
            return View();
           
        }

 
    }





}
>>>>>>> TripCreation View added
