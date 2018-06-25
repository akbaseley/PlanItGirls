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
        public ActionResult TripCreation()
        {
            return View();
        }

        
        public ActionResult TripBudgetCalculator(string VehicleSelection, string TripID, string hotelPricePoint, string HotelSelection, string NumberOfNights, string restaurantPricePoint, string RestaurantSelection, string NumberOfMeals)
        {
            double travelBudget = 0;
            planitdbEntities ORM = new planitdbEntities();
            if (TempData["currentTrip"] is null)
            {
                TempData["currentTrip"] = ORM.Trips.Find(TripID);
            }
            Trip currentTrip = (Trip)TempData["currentTrip"];
            ViewBag.currentTrip = currentTrip;

            #region Travel Costs
            //ViewBag.drivePrice = drivePrice;

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
                JObject GoogleData = TripDistance(currentTrip);
                double DistanceinKM = (double.Parse(GoogleData["routes"][0]["legs"][0]["distance"]["value"].ToString())) / 1000;
                double Distance = Math.Round(DistanceinKM * 0.621371, 0);
                double travelCost = Distance / CalculateGasBudget(VehicleSelection);
                //double drivePrice = 0.608;  
                double oneWayCost = double.Parse(VehicleSelection) * Distance;
                travelBudget = Math.Round(currentTrip.Price - oneWayCost, 2);
                ViewBag.GasPrice = CalculateGasBudget(VehicleSelection);
                ViewBag.DistanceBetweenCities = Distance;
                ViewBag.oneWayCost = Math.Round((oneWayCost * CalculateGasBudget(VehicleSelection)), 2);
                ViewBag.travelBudget = travelBudget;
                TempData["travelBudget"] = travelBudget;
            }
            #endregion
            #region Hotels
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
                ViewBag.PricePerDay = findHotelBudget(hotelPricePoint);
            }
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
            if (TempData["NumberOfNights"] is null && NumberOfNights is null)
            {
                ViewBag.NumberOfNights = null;
            }
            else
            {
                if (TempData["NumberOfNights"] is null)
                {
                    TempData["NumberOfNigths"] = NumberOfNights;
                }
                else
                {
                    NumberOfNights = (string)TempData["NumberOfNights"];
                }
                ViewBag.NumberOfNights = int.Parse(NumberOfNights);
                double TotalHotelBudget = findHotelBudget(hotelPricePoint) * double.Parse(NumberOfNights);
                ViewBag.TotalHotelBudget = TotalHotelBudget;
                double AdjustedTotalBudget = travelBudget - TotalHotelBudget;
                ViewBag.AdjustedTotalBudget = AdjustedTotalBudget;
                TempData["AdjustedTotalBudget"] = AdjustedTotalBudget;
            }
            #endregion
            #region Restaurants
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
                ViewBag.Restaurants = RestaurantsbyPricePoint(currentTrip.TripID, restaurantPricePoint);
                ViewBag.PricePerMeal = findRestaurantBudget(restaurantPricePoint);
            }
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
                else
                {
                    NumberOfMeals = (string)TempData["NumberOfMeals"];
                }
                ViewBag.NumberOfMeals = int.Parse(NumberOfMeals);
                double TotalRestaurantBudget = findRestaurantBudget(hotelPricePoint) * double.Parse(NumberOfMeals);
                ViewBag.TotalRestaurantBudget = TotalRestaurantBudget;
                double AdjustedRestaurantTotalBudget = travelBudget - TotalRestaurantBudget;
                ViewBag.AdjustedRestaurantTotalBudget = AdjustedRestaurantTotalBudget;
                TempData["AdjustedRestaurantTotalBudget"] = AdjustedRestaurantTotalBudget;
            }
            #endregion
            #region TempData
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
            #endregion
            return View();
        }
        public static double CalculateGasBudget(string VehicleSelection)
        {
            double GasPrice = 0;
            if (VehicleSelection == "1")
            {
                GasPrice = 0.46;
            }
            else if (VehicleSelection == "2")
            {
                GasPrice = 0.59;
            }
            else if (VehicleSelection == "3")
            {
                GasPrice = 0.65;
            }
            else if (VehicleSelection == "4")
            {
                GasPrice = 0.72;
            }
            return GasPrice;
        }
        public static double findRestaurantBudget(string pricePoint)
        {
            double Price = 0;
            if (pricePoint == "1")
            {
                Price = 10;
            }
            else if (pricePoint == "2")
            {
                Price = 35;
            }
            else if (pricePoint == "3")
            {
                Price = 60;
            }
            else if (pricePoint == "4")
            {
                Price = 100;
            }
            return Price;
        }
        public static double findHotelBudget(string pricePoint)
        {
            double hotelPrice = 0;
            if (pricePoint == "1")
            {
                hotelPrice = 80;
            }
            else if (pricePoint == "2")
            {
                hotelPrice = 150;
            }
            else if (pricePoint == "3")
            {
                hotelPrice = 225;
            }
            else if (pricePoint == "4")
            {
                hotelPrice = 350;
            }
            return hotelPrice;
        }
        public static int NumOfDays(Trip thisTrip)
        {
            TimeSpan days = thisTrip.EndDate.Subtract(thisTrip.StartDate).Duration();
            int DayDiff = (int)days.TotalDays;
            return DayDiff;
        }
        public JObject TripDistance(Trip currentTrip)
        {
            HttpWebRequest WR = WebRequest.CreateHttp($"https://maps.googleapis.com/maps/api/directions/json?origin=" + currentTrip.StartCity + "+" + currentTrip.StartState + "&destination=" + currentTrip.EndCity + "," + currentTrip.EndState + "&key=" + ConfigurationManager.AppSettings["GoogleAPIKey"]);
            WR.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();
            StreamReader data = new StreamReader(Response.GetResponseStream());
            string JsonData = data.ReadToEnd();
            JObject GoogleData = JObject.Parse(JsonData);
            return GoogleData;
        }
        public JObject HotelsbyPricePoint(string TripID, string pricePoint)
        {
            planitdbEntities ORM = new planitdbEntities();
            Trip currentTrip = ORM.Trips.Find(TripID);
            HttpWebRequest WR = WebRequest.CreateHttp($"https://api.yelp.com/v3/businesses/search?location={currentTrip.EndCity},+{currentTrip.EndState}&price={pricePoint}&term=hotel");
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
            planitdbEntities ORM = new planitdbEntities();
            Trip currentTrip = ORM.Trips.Find(TripID);
            HttpWebRequest WR = WebRequest.CreateHttp($"https://api.yelp.com/v3/businesses/search?location={currentTrip.EndCity},+{currentTrip.EndState}&price={pricePoint}&term=restaurants");
            WR.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            WR.Headers.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["YelpAPIKey"]}");
            HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();
            StreamReader data = new StreamReader(Response.GetResponseStream());
            string JsonData = data.ReadToEnd();
            JObject YelpData = JObject.Parse(JsonData);
            return YelpData;
        }
        public JObject EventsbyPricePoint(string TripID)
        {
            planitdbEntities ORM = new planitdbEntities();
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
    }
}