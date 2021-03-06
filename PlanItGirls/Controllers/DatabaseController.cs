﻿using PlanItGirls.Models;
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
        // Primarily used when Logging trip information

        [Authorize]
        public ActionResult TripList()
        {

            PlanItDBEntities ORM = new PlanItDBEntities();

            if (User.Identity.GetUserId() != null)
            {
                string userID = User.Identity.GetUserId();
                ViewBag.userTrips = ORM.AspNetUsers.Find(userID).Trips.ToList();
                return View("../Home/TripList");
            }
            else
            {
                ViewBag.ErrorMessage = "Please log in to see your trips.";
                return View("Error");
            }
        }
        public ActionResult CreateNewTrip(Trip newTrip)
        {

            if (newTrip.StartDate > newTrip.EndDate)
            {

                ViewBag.Error = "End date should be greater than Start date";
                return View("../Home/TripCreation");
            }

            PlanItDBEntities ORM = new PlanItDBEntities();

            newTrip.UserID = User.Identity.GetUserId();

            ORM.Trips.Add(newTrip);

            ORM.SaveChanges();

            return RedirectToAction("TripList");
        }

        public ActionResult DeleteTrip(string TripID)
        {

            PlanItDBEntities ORM = new PlanItDBEntities();

            Trip Found = ORM.Trips.Find(TripID);

            if (Found != null)
            {
                List<Food> foundFoods = ORM.Foods.Where(c=>c.TripID.Contains(TripID)).ToList();
                List<Lodge> foundLodges = ORM.Lodges.Where(c => c.TripID.Contains(TripID)).ToList();

                foreach(var restauarant in foundFoods)
                {
                    ORM.Foods.Remove(restauarant);
                }

                foreach (var hotel in foundLodges)
                {
                    ORM.Lodges.Remove(hotel);
                }

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

            Trip Found;


            if (TripID is null)
            {
                Found = (Trip)TempData["currentTrip"];
            }
            else
            {
                Found = ORM.Trips.Find(TripID);
            }
            //string userID = User.Identity.GetUserId();
            //ViewBag.userTrips = ORM.AspNetUsers.Find(userID).Trips.ToList();

            if (Found != null)
            {
                if (Found.StartDate > Found.EndDate)
                {
                    ViewBag.Error = "End date should be greater than Start date";
                    return View("../Home/UpdateUserTrip", Found);
                }

                TempData["currentTrip"] = Found;
                TempData["currentTrip"] = TempData["currentTrip"];
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
                TempData["currentTrip"] = OldTripRecord;
                TempData["currentTrip"] = TempData["currentTrip"];
                
                return RedirectToAction("EditTripDetails");
            }

        }

        public ActionResult SaveHotelOption()
        {
            PlanItDBEntities ORM = new PlanItDBEntities();

            Trip currentTrip = (Trip)TempData["currentTrip"];
            string thisHotel = (string)TempData["HotelSelection"];
            string NumberOfNights = (string)TempData["NumberOfNights"];
            string hotelPricePoint = (string)TempData["hotelPricePoint"];
            JObject currentHotel = JObject.Parse(thisHotel);
            string hotelName = currentHotel["name"].ToString();
            Lodge newHotel = new Lodge();

            Lodge Found = ORM.Lodges.Find(hotelName);

            if (Found != null)
            {
                TempData["hotelPricePoint"] = null;
                TempData["HotelSelection"] = null;
                TempData["NumberOfNights"] = null;

                ViewBag.DuplicateHotel = "The hotel you chose is already on your list";

                TempData["currentTrip"] = TempData["currentTrip"];
                return View("Error");
            }
            else
            {
                newHotel.Lodging = (string)currentHotel["name"];
                newHotel.Price = int.Parse(hotelPricePoint);
                newHotel.NumberOfNights = int.Parse(NumberOfNights);
                newHotel.Address = (string)currentHotel["location"]["address1"];
                newHotel.City = (string)currentHotel["location"]["city"];
                newHotel.State = (string)currentHotel["location"]["state"];
                newHotel.PostalCode = (string)currentHotel["location"]["zip_code"];
                newHotel.PhoneNumber = (string)currentHotel["display_phone"];
                newHotel.URL = (string)currentHotel["url"];
                newHotel.TripID = currentTrip.TripID;

                ORM.Lodges.Add(newHotel);
                ORM.SaveChanges();

                Trip editedTrip = ORM.Trips.Find(newHotel.TripID);

                TempData["currentTrip"] = editedTrip;
                TempData["currentTrip"] = TempData["currentTrip"];

                return RedirectToAction("../Home/TripSummary");
            }
        }
        public ActionResult DeleteHotel(string Lodging)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            Lodge Found = ORM.Lodges.Find(Lodging);
            Trip currentTrip = ORM.Trips.Find(Found.TripID);

            if (Found != null)
            {
                ORM.Lodges.Remove(Found);
                ORM.SaveChanges();

                TempData["currentTrip"] = currentTrip;
                TempData["currentTrip"] = TempData["currentTrip"];

                return RedirectToAction("../Home/TripSummary");
            }
            else
            {
                TempData["currentTrip"] = TempData["currentTrip"];

                ViewBag.Message = "Hotel Not Found";
                return View("Error");
            }

        }
        public ActionResult EditHotelDetails(string Lodging)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            Lodge Found = ORM.Lodges.Find(Lodging);
            Trip currentTrip = (Trip)TempData["currentTrip"];

            ViewBag.DayDiff = HomeController.NumOfDays(currentTrip);

            TempData["currentTrip"] = TempData["currentTrip"];
            if (Found != null)
            {
                return View("EditHotelDetails", Found);
            }
            else
            {
                ViewBag.ErrorMessage = "Hotel Not Found";
                return View("Error");
            }

        }
        public ActionResult SaveUpdateHotel(Lodge EditHotelDetails)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            Lodge OldHotelRecord = ORM.Lodges.Find(EditHotelDetails.Lodging);


            if (OldHotelRecord != null && ModelState.IsValid)
            {
                OldHotelRecord.Lodging = EditHotelDetails.Lodging;
                OldHotelRecord.Price = EditHotelDetails.Price;
                OldHotelRecord.NumberOfNights = EditHotelDetails.NumberOfNights;
                OldHotelRecord.Address = EditHotelDetails.Address;
                OldHotelRecord.City = EditHotelDetails.City;
                OldHotelRecord.State = EditHotelDetails.State;
                OldHotelRecord.PostalCode = EditHotelDetails.PostalCode;
                OldHotelRecord.PhoneNumber = EditHotelDetails.PhoneNumber;
                OldHotelRecord.URL = EditHotelDetails.URL;
                OldHotelRecord.TripID = EditHotelDetails.TripID;

                ORM.Entry(OldHotelRecord).State = System.Data.Entity.EntityState.Modified;
                ORM.SaveChanges();

                TempData["currentTrip"] = ORM.Trips.Find(EditHotelDetails.TripID);
                TempData["currentTrip"] = TempData["currentTrip"];

                return RedirectToAction("../Home/TripSummary");
        }
            else
            {
                TempData["currentTrip"] = TempData["currentTrip"];

                ViewBag.Message = "Hotel Not Found";
                return View("Error");
    }
}

        public ActionResult SaveRestaurantOption()
        {
            PlanItDBEntities ORM = new PlanItDBEntities();

            Trip currentTrip = (Trip)TempData["currentTrip"];
            string thisRestaurant = (string)TempData["RestaurantSelection"];
            string numberOfMeals = (string)TempData["NumberOfMeals"];
            string restaurantPricePoint = (string)TempData["restaurantPricePoint"];
            JObject currentRestaurant = JObject.Parse(thisRestaurant);
            string restaurantName = currentRestaurant["name"].ToString();
            Food newRestaurant = new Food();

            Food Found = ORM.Foods.Find(restaurantName);

            if (Found != null)
            {
                TempData["restaurantPricePoint"] = null;
                TempData["restaurantSelection"] = null;
                TempData["NumberOfMeals"] = null;

                ViewBag.DuplicateRestaurant = "The restaurant you chose is already on your list";

                TempData["currentTrip"] = TempData["currentTrip"];
                return View("Error");
            }
            else
            {
                newRestaurant.Restaurant = (string)currentRestaurant["name"];
                newRestaurant.Price = int.Parse(restaurantPricePoint);
                newRestaurant.NumberOfMeals = int.Parse(numberOfMeals);
                newRestaurant.Address = (string)currentRestaurant["location"]["address1"];
                newRestaurant.City = (string)currentRestaurant["location"]["city"];
                newRestaurant.State = (string)currentRestaurant["location"]["state"];
                newRestaurant.PostalCode = (string)currentRestaurant["location"]["zip_code"];
                newRestaurant.PhoneNumber = (string)currentRestaurant["display_phone"];
                newRestaurant.URL = (string)currentRestaurant["url"];
                newRestaurant.TripID = currentTrip.TripID;

                ORM.Foods.Add(newRestaurant);
                ORM.SaveChanges();

                Trip thisTrip = ORM.Trips.Find(newRestaurant.TripID);

                TempData["currentTrip"] = thisTrip;

                TempData["currentTrip"] = TempData["currentTrip"];

                return RedirectToAction("../Home/TripSummary");
            }
        }
        public ActionResult DeleteRestaurant(string Restaurant)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            Food Found = ORM.Foods.Find(Restaurant);

            if (Found != null)
            {
                Trip currentTrip = ORM.Trips.Find(Found.TripID);

                ORM.Foods.Remove(Found);
                ORM.SaveChanges();

                TempData["currentTrip"] = currentTrip;
                TempData["currentTrip"] = TempData["currentTrip"];

                return RedirectToAction("../Home/TripSummary");
            }
            else
            {
                TempData["currentTrip"] = TempData["currentTrip"];

                ViewBag.Message = "Restaurant Not Found";
                return View("Error");
            }
        }
        public ActionResult EditRestaurantDetails(string Restaurant)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            Food Found = ORM.Foods.Find(Restaurant);
            Trip currentTrip = (Trip)TempData["currentTrip"];

            ViewBag.DayDiff = HomeController.NumOfDays(currentTrip);

            TempData["currentTrip"] = TempData["currentTrip"];
            if (Found != null)
            {
                return View("EditRestaurantDetails", Found);
            }
            else
            {
                ViewBag.ErrorMessage = "Restaurant Not Found";
                return View("Error");
            }
        }
        public ActionResult SaveUpdateRestaurant(Food EditRestaurantDetails)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            Food OldRestaurantRecord = ORM.Foods.Find(EditRestaurantDetails.Restaurant);

            if (OldRestaurantRecord != null && ModelState.IsValid)
            {
                OldRestaurantRecord.Restaurant = EditRestaurantDetails.Restaurant;
                OldRestaurantRecord.Price = EditRestaurantDetails.Price;
                OldRestaurantRecord.NumberOfMeals = EditRestaurantDetails.NumberOfMeals;
                OldRestaurantRecord.Address = EditRestaurantDetails.Address;
                OldRestaurantRecord.City = EditRestaurantDetails.City;
                OldRestaurantRecord.State = EditRestaurantDetails.State;
                OldRestaurantRecord.PostalCode = EditRestaurantDetails.PostalCode;
                OldRestaurantRecord.PhoneNumber = EditRestaurantDetails.PhoneNumber;
                OldRestaurantRecord.URL = EditRestaurantDetails.URL;
                OldRestaurantRecord.TripID = EditRestaurantDetails.TripID;

                ORM.Entry(OldRestaurantRecord).State = System.Data.Entity.EntityState.Modified;
                ORM.SaveChanges();

                TempData["currentTrip"] = ORM.Trips.Find(OldRestaurantRecord.TripID);
                TempData["currentTrip"] = TempData["currentTrip"];
                return RedirectToAction("../Home/TripSummary");
            }
            else
            {
                TempData["currentTrip"] = TempData["currentTrip"];

                ViewBag.Message = "Restaurant Not Found";
                return View("Error");
            }

        }

        public ActionResult EditCarOption()
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            Trip Found = (Trip)TempData["currentTrip"];

            if (Found !=null)
            {
                TempData["VehicleSelection"] = TempData["VehicleSelection"];
                TempData["currentTrip"] = TempData["currentTrip"];
                return RedirectToAction("SaveCarOption");
            }
            else
            {
                ViewBag.Message = "Select a trip to add a car to.";
                return View("Error");
            }

        }
        public ActionResult SaveCarOption()
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            Trip Found = (Trip)TempData["currentTrip"];

            if (Found != null)
            {
                string Car = TempData["VehicleSelection"].ToString();

                Found.Car = Double.Parse(Car);

                //ORM.Entry(Found).State = System.Data.Entity.EntityState.Modified;

                ORM.SaveChanges();

                TempData["currentTrip"] = Found;

                TempData["currentTrip"] = TempData["currentTrip"];
                TempData["VehicleSelection"] = TempData["VehicleSelection"];

                return RedirectToAction("../Home/TripBudgetCalculator");

            }
            else
            {
                TempData["currentTrip"] = TempData["currentTrip"];
                TempData["VehicleSelection"] = TempData["VehicleSelection"];

                return View("Error");
            }
        }
    }
}