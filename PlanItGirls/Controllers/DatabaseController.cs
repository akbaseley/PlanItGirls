using PlanItGirls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;

namespace PlanItGirls.Controllers
{
    public class DatabaseController : Controller
    {
        // Use this controller when using CRUD methods to Database
        // Primarily used when Logging trip information and when 

        public ActionResult CreateNewTrip(Trip newTrip)
        {
            planitdbEntities ORM = new planitdbEntities();

            newTrip.UserID = User.Identity.GetUserId();

            ORM.Trips.Add(newTrip);

            ORM.SaveChanges();

            return View("../Home/TripCreation");
        }
        public ActionResult TripList()
        {
            planitdbEntities ORM = new planitdbEntities();

            string userID = User.Identity.GetUserId();

            ViewBag.userTrips = ORM.AspNetUsers.Find(userID).Trips.ToList();

            return View("../Home/TripList");
        }

        public ActionResult DeleteTrip(string TripID)
        {

            planitdbEntities ORM = new planitdbEntities();

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
            planitdbEntities ORM = new planitdbEntities();
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
            planitdbEntities ORM = new planitdbEntities();
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
                return View("Error");
            }

        }

        public ActionResult SaveHotelOption()
        {
            PlanItDBEntities ORM = new PlanItDBEntities();

            Trip currentTrip = (Trip)TempData["currentTrip"];
            string thisHotel = (string)TempData["HotelSelection"];
            JObject currentHotel = (JObject)thisHotel;
            Lodge newHotel = new Lodge();
            newHotel.TripID = currentTrip.TripID;
            newHotel.Price = (decimal)TempData["hotelPricePoint"];
            newHotel.Address = (string)currentHotel["location"]["adddress1"];
            newHotel.City = (string)currentHotel["location"]["city"];
            newHotel.State = (string)currentHotel["location"]["state"];
            newHotel.PostalCode = (string)currentHotel["location"]["zip_code"];
            newHotel.PhoneNumber = (string)currentHotel["display_phone"];
            ORM.Lodges.Add(newHotel);
            ORM.SaveChanges();

            TempData["VehicleSelection"] = TempData["VehicleSelection"];
            TempData["currentTrip"] = TempData["currentTrip"];
            TempData["hotelPricePoint"] = TempData["hotelPricePoint"];
            TempData["HotelSelection"] = TempData["HotelSelection"];
            TempData["NumberOfNights"] = TempData["NumberOfNights"];
            TempData["AdjustedTotalBudget"] = TempData["AdjustedTotalBudget"];
            TempData["restaurantPricePoint"] = TempData["restaurantPricePoint"];
            TempData["RestaurantSelection"] = TempData["RestaurantSelection"];
            TempData["NumberOfMeals"] = TempData["NumberOfMeals"];
            TempData["AdjustedRestaurantTotalBudget"] = TempData["AdjustedRestaurantTotalBudget"];
            return RedirectToAction("../Home/TripBudgetCalculator");
        }

        public ActionResult SaveRestaurantOption(string TripID)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();

            Trip currentTrip = (Trip)TempData["currentTrip"];
            string thisRestaurant = (string)TempData["RestaurantSelection"];
            JObject currentRestaurant = (JObject)thisRestaurant;
            Food newRestaurant = new Food();
            newRestaurant.TripID = currentTrip.TripID;
            newRestaurant.Price = (int)TempData["restaurantPricePoint"];
            newRestaurant.Address = (string)currentRestaurant["location"]["adddress1"];
            newRestaurant.City = (string)currentRestaurant["location"]["city"];
            newRestaurant.State = (string)currentRestaurant["location"]["state"];
            newRestaurant.PostalCode = (string)currentRestaurant["location"]["zip_code"];
            newRestaurant.PhoneNumber = (string)currentRestaurant["display_phone"];
            newRestaurant.URL = (string)currentRestaurant["url"];
            ORM.Foods.Add(newRestaurant);
            ORM.SaveChanges();

            TempData["VehicleSelection"] = TempData["VehicleSelection"];
            TempData["currentTrip"] = TempData["currentTrip"];
            TempData["hotelPricePoint"] = TempData["hotelPricePoint"];
            TempData["HotelSelection"] = TempData["HotelSelection"];
            TempData["NumberOfNights"] = TempData["NumberOfNights"];
            TempData["AdjustedTotalBudget"] = TempData["AdjustedTotalBudget"];
            TempData["restaurantPricePoint"] = TempData["restaurantPricePoint"];
            TempData["RestaurantSelection"] = TempData["RestaurantSelection"];
            TempData["NumberOfMeals"] = TempData["NumberOfMeals"];
            TempData["AdjustedRestaurantTotalBudget"] = TempData["AdjustedRestaurantTotalBudget"];
            return RedirectToAction("../Home/TripBudgetCalculator");
        }



    }
}