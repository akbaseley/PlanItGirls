using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlanItGirls.Models;
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


        public ActionResult NumOfDays (Trip trip )
        {
            PlanItDBEntities ORM = new PlanItDBEntities();

            TimeSpan days = trip.EndDate.Subtract(trip.StartDate).Duration();

            ViewBag.DayDiff = days;
             
            return View("../Database/TripBudgetCalculator");

        }
        
    }
}