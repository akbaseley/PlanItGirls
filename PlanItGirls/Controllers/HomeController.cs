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

        [Authorize]                     
        public ActionResult TripCreation()
        {
            return View();
        }

        [Authorize]
        public ActionResult TripBudgetCalculator(string VehicleSelection, string TripID, string hotelPricePoint, string HotelSelection, string NumberOfNights, string restaurantPricePoint, string RestaurantSelection, string NumberOfMeals)
        {

            #region This Trip
            PlanItDBEntities ORM = new PlanItDBEntities();

            if (TempData["currentTrip"] is null)
            {
                TempData["currentTrip"] = ORM.Trips.Find(TripID);
            }
            Trip currentTrip = (Trip)TempData["currentTrip"];
            ViewBag.currentTrip = currentTrip;
            #endregion

            #region Travel Costs

            if (TempData["VehicleSelection"] is null && VehicleSelection is null)
            {
                ViewBag.SelectVehicle = "Select which type of vehicle you plan to drive";
            }
            else
            {
                if (TempData["VehicleSelection"] is null)
                {
                    TempData["VehicleSelection"] = VehicleSelection;
                }
                else if (VehicleSelection is null)
                {
                    VehicleSelection = (string)TempData["VehicleSelection"];
                }
                else if ((string)TempData["VehicleSelection"] != VehicleSelection)
                {
                    TempData["VehicleSelection"] = VehicleSelection;
                }

                double Distance = TripDistance(currentTrip);

                ViewBag.GasPrice = (double.Parse(VehicleSelection));
                ViewBag.DistanceBetweenCities = Distance;
                ViewBag.carBudget = CarBudget(currentTrip, VehicleSelection);
            }

            #endregion

            #region hotelPricePoint
            if (TempData["hotelPricePoint"] is null && hotelPricePoint is null)
            {
                ViewBag.Hotels = null;
                ViewBag.Fact = "Select Price Point to get Hotel Options";
            }
            else
            {
                if (TempData["hotelPricePoint"] is null)
                {
                    TempData["hotelPricePoint"] = hotelPricePoint;
                }
                else if (hotelPricePoint is null)
                {
                    hotelPricePoint = (string)TempData["hotelPricePoint"];
                }
                else if ((string)TempData["hotelPricePoint"] != hotelPricePoint)
                {
                    TempData["hotelPricePoint"] = hotelPricePoint;
                }
                ViewBag.Hotels = HotelsbyPricePoint(currentTrip.TripID, hotelPricePoint);
                ViewBag.hotelPricePoint = double.Parse(hotelPricePoint);
            }
            #endregion
            #region HotelSelection
            if (TempData["HotelSelection"] is null && HotelSelection is null)
            {
                ViewBag.HotelSelection = null;
            }
            else
            {
                if (TempData["HotelSelection"] is null)
                {
                    TempData["HotelSelection"] = HotelSelection;
                }
                else if (HotelSelection is null)
                {
                    HotelSelection = (string)TempData["HotelSelection"];
                }
                else if ((string)TempData["HotelSelection"] != HotelSelection)
                {
                    TempData["HotelSelection"] = HotelSelection;
                }
                ViewBag.HotelSelection = JObject.Parse(HotelSelection);
                ViewBag.DayDiff = NumOfDays(currentTrip);
                TempData["HotelSelection"] = HotelSelection;
            }
            #endregion
            #region NumberOfNights
            if (TempData["NumberOfNights"] is null && NumberOfNights is null)
            {
                ViewBag.NumberOfNights = null;
            }
            else
            {
                if (TempData["NumberOfNights"] is null)
                {
                    TempData["NumberOfNights"] = NumberOfNights;
                }
                else if (NumberOfNights is null)
                {
                    NumberOfNights = (string)TempData["NumberOfNights"];
                }
                else if ((string)TempData["NumberOfNights"] != NumberOfNights)
                {
                    TempData["NumberOfNights"] = NumberOfNights;
                }
                ViewBag.NumberOfNights = double.Parse(NumberOfNights);
                double hotelBudget = double.Parse(hotelPricePoint) * double.Parse(NumberOfNights);

            }
            #endregion

            #region restaurantPricePoint
            if (TempData["restaurantPricePoint"] is null && restaurantPricePoint is null)
            {
                ViewBag.Restaurants = null;
                ViewBag.MealFact = "Select Price Point to get Restaurant Options";
            }
            else
            {
                if (TempData["restaurantPricePoint"] is null)
                {
                    TempData["restaurantPricePoint"] = restaurantPricePoint;
                }
                else if (restaurantPricePoint is null)
                {
                    restaurantPricePoint = (string)TempData["restaurantPricePoint"];
                }
                else if ((string)TempData["restaurantPricePoint"] != restaurantPricePoint)
                {
                    TempData["restaurantPricePoint"] = restaurantPricePoint;
                }
                ViewBag.restaurantPricePoint = double.Parse(restaurantPricePoint);
                ViewBag.Restaurants = RestaurantsbyPricePoint(currentTrip.TripID, restaurantPricePoint);
            }
            #endregion
            #region RestaurantSelection
            if (TempData["RestaurantSelection"] is null && RestaurantSelection is null)
            {
                ViewBag.RestaurantSelection = null;
            }
            else
            {
                if (TempData["RestaurantSelection"] is null)
                {
                    TempData["RestaurantSelection"] = RestaurantSelection;
                }
                else if (RestaurantSelection is null)
                {
                    RestaurantSelection = (string)TempData["RestaurantSelection"];
                }
                else if ((string)TempData["RestaurantSelection"] != RestaurantSelection)
                {
                    TempData["RestaurantSelection"] = RestaurantSelection;
                    RestaurantSelection = (string)TempData["RestaurantSelection"];
                }
                ViewBag.RestaurantSelection = JObject.Parse(RestaurantSelection);
                ViewBag.DayDiff = NumOfDays(currentTrip);
                TempData["RestaurantSelection"] = RestaurantSelection;
            }
            #endregion
            #region NumberOfMeals
            if (TempData["NumberOfMeals"] is null && NumberOfMeals is null)
            {
                ViewBag.NumberOfMeals = null;
            }
            else
            {
                if (TempData["NumberOfMeals"] is null)
                {
                    TempData["NumberOfMeals"] = NumberOfMeals;
                }
                else if (NumberOfMeals is null)
                {
                    NumberOfMeals = (string)TempData["NumberOfMeals"];
                }
                else if ((string)TempData["NumberOfMeals"] != NumberOfMeals)
                {
                    TempData["NumberOfMeals"] = NumberOfMeals;
                }

                ViewBag.NumberOfMeals = double.Parse(NumberOfMeals);
            }
            #endregion

            #region Budget Calculations

            ViewBag.currentTrip = currentTrip; //Trip currentTrip
            ViewBag.HotelBudget = HotelBudget(currentTrip); //calculates the total amount for all selected hotels
            ViewBag.RestaurantBudget = RestaurantBudget(currentTrip); //calculates the total amount for all selected restaurants
            ViewBag.totalDeductedBudget = TotalDeductedBudget(currentTrip); //combined hotel budgets & restaurant budgets
            ViewBag.remainingBudget = RemainingBudget(currentTrip); //total budget minus hotels, restaurants, and cars 
                                                                    //ViewBag.carBudget is found above under Travel Costs - total cost of driving

            ViewBag.reservedMeals = ReservedMeals(currentTrip);
            ViewBag.reservedNights = ReservedNights(currentTrip);
            ViewBag.numberOfDays = NumOfDays(currentTrip);
            #endregion

            #region TempData
            TempData["VehicleSelection"] = TempData["VehicleSelection"];
            TempData["currentTrip"] = TempData["currentTrip"];

            TempData["hotelPricePoint"] = TempData["hotelPricePoint"];
            TempData["HotelSelection"] = TempData["HotelSelection"];
            TempData["NumberOfNights"] = TempData["NumberOfNights"];

            TempData["restaurantPricePoint"] = TempData["restaurantPricePoint"];
            TempData["RestaurantSelection"] = TempData["RestaurantSelection"];
            TempData["NumberOfMeals"] = TempData["NumberOfMeals"];
            #endregion

            return View();
        }

        [Authorize]
        public ActionResult TripSummary(string TripID)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            Trip currentTrip;

            if (TripID is null)
            {
                currentTrip = (Trip)TempData["currentTrip"];
            }
            else
            {
                currentTrip = ORM.Trips.Find(TripID);
            }

            ViewBag.currentTrip = currentTrip;
            ViewBag.HotelBudget = HotelBudget(currentTrip);
            ViewBag.RestaurantBudget = RestaurantBudget(currentTrip);
            ViewBag.CarBudget = CarBudget(currentTrip);
            ViewBag.totalDeductedBudget = TotalDeductedBudget(currentTrip);
            ViewBag.remainingBudget = RemainingBudget(currentTrip);

            ViewBag.reservedMeals = ReservedMeals(currentTrip);
            ViewBag.reservedNights = ReservedNights(currentTrip);
            ViewBag.numberOfDays = NumOfDays(currentTrip);

            JObject EventOptions = Events(currentTrip.TripID);
            ViewBag.EventOptions = EventOptions["events"];

            TempData["currentTrip"] = currentTrip;
            TempData["currentTrip"] = TempData["currentTrip"];

            TempData["VehicleSelection"] = null;
            TempData["carBudget"] = null;

            TempData["hotelPricePoint"] = null;
            TempData["HotelSelection"] = null;
            TempData["NumberOfNights"] = null;

            TempData["restaurantPricePoint"] = null;
            TempData["RestaurantSelection"] = null;
            TempData["NumberOfMeals"] = null;

            return View();
        }

        public ActionResult ContinueBudgetPlanning()
        {
            TempData["currentTrip"] = TempData["currentTrip"];
            return RedirectToAction("TripBudgetCalculator");
        }

        public double HotelBudget(Trip thisTrip)
        {
            double hotelBudget = 0;

            foreach (var hotel in thisTrip.Lodges)
            {
                hotelBudget = hotelBudget + hotel.Price;
            }

            return hotelBudget;
        }

        public double RestaurantBudget(Trip thisTrip)
        {
            double restaurantBudget = 0;

            foreach (var restaurant in thisTrip.Foods)
            {
                restaurantBudget = ((restaurantBudget + restaurant.Price) * restaurant.NumberOfMeals);
            }

            return restaurantBudget;
        }

        public double TotalDeductedBudget(Trip thisTrip)
        {
            double restaurantBudget = RestaurantBudget(thisTrip);
            double hotelBudget = HotelBudget(thisTrip);
            double totalDeductedBudget = restaurantBudget + hotelBudget;

            return totalDeductedBudget;
        }

        public double CarBudget(Trip thisTrip, string thisCar)
        {
            double Distance = TripDistance(thisTrip);
            double carBudget = ((double.Parse(thisCar) * Distance)*2);

            return carBudget;
        }

        public double CarBudget(Trip thisTrip)
        {
            double carBudget;
            if (thisTrip.Car != null)
            {
                double Distance = TripDistance(thisTrip);
                carBudget = (((double)thisTrip.Car * Distance) * 2);
            }
            else
            {
                carBudget = 0;
            }
               return carBudget;
        }

        public double RemainingBudget(Trip thisTrip)
        {
            double carBudget;
            if (thisTrip.Car is null)
            {
                carBudget = 0;
            }
            else
            {
                carBudget = CarBudget(thisTrip);
            }

            double remainingBudget = thisTrip.Price - (TotalDeductedBudget(thisTrip) + carBudget);

            return remainingBudget;
        }

        public static int NumOfDays(Trip thisTrip)
        {
            TimeSpan days = thisTrip.EndDate.Subtract(thisTrip.StartDate).Duration();
            int DayDiff = (int)days.TotalDays;
            return DayDiff;
        }

        public double TripDistance(Trip currentTrip)
        {
            HttpWebRequest WR = WebRequest.CreateHttp($"https://maps.googleapis.com/maps/api/directions/json?origin=" + currentTrip.StartCity + "+" + currentTrip.StartState + "&destination=" + currentTrip.EndCity + "," + currentTrip.EndState + "&key=" + ConfigurationManager.AppSettings["GoogleAPIKey"]);
            WR.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();
            StreamReader data = new StreamReader(Response.GetResponseStream());
            string JsonData = data.ReadToEnd();
            JObject GoogleData = JObject.Parse(JsonData);

            double DistanceinKM = (double.Parse(GoogleData["routes"][0]["legs"][0]["distance"]["value"].ToString())) / 1000;
            double Distance = Math.Round(DistanceinKM * 0.621371, 0);

            return Distance;
        }

        public JObject HotelsbyPricePoint(string TripID, string pricePoint)
        {

            string Cost;
            if (pricePoint == "80")
            {
                Cost = "1";
            }
            else if (pricePoint == "150")
            {
                Cost = "2";
            }
            else if (pricePoint == "225")
            {
                Cost = "3";
            }
            else
            {
                Cost = "4";
            }

            PlanItDBEntities ORM = new PlanItDBEntities();
            Trip currentTrip = ORM.Trips.Find(TripID);
            HttpWebRequest WR = WebRequest.CreateHttp($"https://api.yelp.com/v3/businesses/search?location={currentTrip.EndCity},+{currentTrip.EndState}&price={Cost}&term=hotel");
            WR.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            WR.Headers.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["YelpAPIKey"]}");
            HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();
            StreamReader data = new StreamReader(Response.GetResponseStream());
            string JsonData = data.ReadToEnd();
            JObject YelpData = JObject.Parse(JsonData);
            return YelpData;
        }

        public JObject RestaurantsbyPricePoint(string TripID, string pricePoint)
        {

            string Cost;
            if (pricePoint == "10")
            {
                Cost = "1";
            }
            else if (pricePoint == "35")
            {
                Cost = "2";
            }
            else if (pricePoint == "60")
            {
                Cost = "3";
            }
            else
            {
                Cost = "4";
            }
            PlanItDBEntities ORM = new PlanItDBEntities();
            Trip currentTrip = ORM.Trips.Find(TripID);
            HttpWebRequest WR = WebRequest.CreateHttp($"https://api.yelp.com/v3/businesses/search?location={currentTrip.EndCity},+{currentTrip.EndState}&price={Cost}&term=restaurants");
            WR.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            WR.Headers.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["YelpAPIKey"]}");
            HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();
            StreamReader data = new StreamReader(Response.GetResponseStream());
            string JsonData = data.ReadToEnd();
            JObject YelpData = JObject.Parse(JsonData);
            return YelpData;
        }

        public JObject Events(string TripID)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            Trip currentTrip = ORM.Trips.Find(TripID);
            string StartDate = ConvertToUnixTime(currentTrip.StartDate);
            string EndDate = ConvertToUnixTime(currentTrip.EndDate);
            HttpWebRequest WR = WebRequest.CreateHttp($"https://api.yelp.com/v3/events?location={currentTrip.EndCity},+{currentTrip.EndState}&start_date={StartDate}&end_date={EndDate}&term=events");
            WR.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            WR.Headers.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["YelpAPIKey"]}");
            HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();
            StreamReader data = new StreamReader(Response.GetResponseStream());
            string JsonData = data.ReadToEnd();
            JObject YelpData = JObject.Parse(JsonData);
            return YelpData;
        }

        public static string ConvertToUnixTime(DateTime currentTime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return ((currentTime - sTime).TotalSeconds).ToString();
        }

        public int ReservedNights (Trip thisTrip)
        {
            int reservedNights = 0;
            foreach (Lodge hotel in thisTrip.Lodges)
            {
                reservedNights = reservedNights = hotel.NumberOfNights;
            }

            return reservedNights;
        }

        public int ReservedMeals (Trip thisTrip)
        {
            int reservedMeals = 0;
            foreach (Food restaurant in thisTrip.Foods)
            {
                reservedMeals = reservedMeals + restaurant.NumberOfMeals;
            }
            return reservedMeals;
        }
    }
}