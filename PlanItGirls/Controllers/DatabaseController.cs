using PlanItGirls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace PlanItGirls.Controllers
{
    public class DatabaseController : Controller
    {
        // Use this controller when using CRUD methods to Database
        // Primarily used when Logging trip information and when 

        public ActionResult CreateNewTrip(Trip newTrip)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();

            newTrip.UserID = User.Identity.GetUserId();

            ORM.Trips.Add(newTrip);

            ORM.SaveChanges();

            return View("../Home/TripCreation");
        }

        public ActionResult TripList()
        {
            PlanItDBEntities ORM = new PlanItDBEntities();

            string userID = User.Identity.GetUserId();

            ViewBag.userTrips = ORM.AspNetUsers.Find(userID).Trips.ToList();

            return View("../Home/TripList");
        }

        public ActionResult NumOfDays(string TripID)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            
            Trip thisTrip = ORM.Trips.Find(TripID);
            TimeSpan days = thisTrip.EndDate.Subtract(thisTrip.StartDate).Duration();
            ViewBag.DayDiff = days;

            return RedirectToAction("../API/CalculateDistance");
        }
    }
}