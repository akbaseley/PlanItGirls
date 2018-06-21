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
    public class APIController : Controller
    {

        public ActionResult GetPricePointHotels(string pricePoint, string latitude, string longitude)
        {
            HttpWebRequest WR = WebRequest.CreateHttp($"https://api.yelp.com/v3/businesses/search?latitude={latitude}&longitude={longitude}&price={pricePoint}&term=hotel");
            WR.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            WR.Headers.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["YelpAPIKey"]}");
            HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();
            StreamReader data = new StreamReader(Response.GetResponseStream());
            string JsonData = data.ReadToEnd();
            JObject YelpData = JObject.Parse(JsonData);
            ViewBag.Fact = YelpData;
            return View("SearchResultsPage");
        }

        public ActionResult GetPricePointRestaurants(string pricePoint)
        {
            HttpWebRequest WR = WebRequest.CreateHttp("https://api.yelp.com/v3/businesses/search?latitude=42.331429&longitude=-83.045753&price=1&term=restaurants");
            WR.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            WR.Headers.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["YelpAPIKey"]}");
            HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();
            StreamReader data = new StreamReader(Response.GetResponseStream());
            string JsonData = data.ReadToEnd();
            JObject YelpData = JObject.Parse(JsonData);
            ViewBag.Fact = YelpData;
            return View("SearchResultsPage");
        }

        public ActionResult GetYelpEventsByLocationandTime(string location, string start_date, string end_date)
        {
            HttpWebRequest WR = WebRequest.CreateHttp("https://api.yelp.com/v3/events?location=Detroit+MI&start_date=1529286986&end_date=1529625600");
            WR.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            WR.Headers.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["YelpAPIKey"]}");
            HttpWebResponse Response = (HttpWebResponse)WR.GetResponse();
            StreamReader data = new StreamReader(Response.GetResponseStream());
            string JsonData = data.ReadToEnd();
            JObject YelpData = JObject.Parse(JsonData);
            ViewBag.Fact = YelpData;
            return View("SearchResultsPage");
        }

       
    }
}