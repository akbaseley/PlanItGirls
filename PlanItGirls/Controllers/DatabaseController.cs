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


        public ActionResult DeleteTrip(string TripID)
        {

            PlanItDBEntities ORM = new PlanItDBEntities();

            Trip Found = ORM.Trips.Find(TripID);

            if (Found != null)
            {
                ORM.Trips.Remove(Found);
                ORM.SaveChanges();
                return RedirectToAction("TripList");
            }

            else
            {
                ViewBag.Message = "Trip Not Found";
                return View("Error");
            }


        }
        public ActionResult EditTripDetails(string TripID)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            //string userID = User.Identity.GetUserId();
            Trip Found = ORM.Trips.Find(TripID);
            //ViewBag.userTrips = ORM.AspNetUsers.Find(userID).Trips.ToList();

            if (Found != null)
            {
                return View ("../Home/UpdateUserTrip", Found);
            }
            else
            {
                ViewBag.ErrorMessage = "Tasks Not Found";
                return View("Error");
            }
        }

        public ActionResult SaveUpdateUserTrip (Trip EditTripDetails)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
         //   string userID = User.Identity.GetUserId();
         //ORM.AspNetUsers.Find(userID).Trips.ToList();
            Trip OldTripRecord = ORM.Trips.Find(EditTripDetails.TripID);

            if (OldTripRecord != null && ModelState.IsValid)
            {
                OldTripRecord.TripID = EditTripDetails.TripID;
                OldTripRecord.StartDate = EditTripDetails.StartDate;
                OldTripRecord.EndDate = EditTripDetails.EndDate;
                OldTripRecord.StartCity = EditTripDetails.StartCity;
                OldTripRecord.EndCity = EditTripDetails.EndCity;
                OldTripRecord.Price = EditTripDetails.Price;

                ORM.Entry(OldTripRecord).State = System.Data.Entity.EntityState.Modified;
                
                ORM.SaveChanges();
                return RedirectToAction("TripList");
            }
            else
            {
                ViewBag.ErrorMessage = "Task not Found";
                return View("you suck!!!");
            }

        }



    }
}