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

        public ViewResult TripBudgetCalculator(string TripID, string pricePoint)
        {
            PlanItDBEntities ORM = new PlanItDBEntities();
            if (TempData["currentTrip"] is null)
            {
                TempData["currentTrip"] = ORM.Trips.Find(TripID);
            }

            Trip currentTrip = (Trip)TempData["currentTrip"];

            ViewBag.currentTrip = currentTrip;

            JObject GoogleData = TripDistance(currentTrip);

            double DistanceinKM = (double.Parse(GoogleData["routes"][0]["legs"][0]["distance"]["value"].ToString())) / 1000;
            double Distance = Math.Round(DistanceinKM * 0.621371, 0);
            double drivePrice = 0.608;
            double oneWay = drivePrice * Distance;
            double roundTrip = oneWay * 2;

            ViewBag.DistanceBetweenCities = Distance;
            ViewBag.drivePrice = drivePrice;
            ViewBag.oneWay = Math.Round(oneWay, 2);
            ViewBag.roundTrip = Math.Round(roundTrip, 2);
            ViewBag.newBudget = Math.Round(currentTrip.Price - oneWay, 2);

            if (pricePoint is null)
            {
                ViewBag.Hotels = null;
                ViewBag.Fact = "Select Price Point to get Hotel Options";

            }
            else
            {
                ViewBag.Hotels = HotelsbyPricePoint(currentTrip.TripID, pricePoint);
            }

            TempData["currentTrip"] = TempData["currentTrip"];
            return View();

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
            PlanItDBEntities ORM = new PlanItDBEntities();
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
    }
}

