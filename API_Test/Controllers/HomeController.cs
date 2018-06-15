using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace API_Test.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            HttpWebRequest WR = WebRequest.CreateHttp("https://www.zillow.com/webservice/GetUpdatedPropertyDetails.htm?zws-id=X1-ZWz18inimh1wqz_5kvb6&zpid=2093018427");
            WR.UserAgent = ".NET Framework Test Client";

            HttpWebResponse Response;

            try
            {
                Response = (HttpWebResponse)WR.GetResponse();
            }
            catch (WebException e)
            {
                ViewBag.Error = "Exception";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }

            if (Response.StatusCode != HttpStatusCode.OK)
            {
                ViewBag.Error = Response.StatusCode;
                ViewBag.ErrorDescription = Response.StatusDescription;
                return View();
            }

            StreamReader reader = new StreamReader(Response.GetResponseStream());
            string ZillowData = reader.ReadToEnd();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ZillowData);
                string jsonText = JsonConvert.SerializeXmlNode(doc);
                JObject JsonData = JObject.Parse(jsonText);
                ViewBag.Address = JsonData["UpdatedPropertyDetails:updatedPropertyDetails"]["response"]["address"]["street"];
                ViewBag.Zipcode = JsonData["UpdatedPropertyDetails:updatedPropertyDetails"]["response"]["address"]["zipcode"];
                ViewBag.ImageQuant = (int)JsonData["UpdatedPropertyDetails:updatedPropertyDetails"]["response"]["images"]["count"];
                ViewBag.Facts = JsonData["UpdatedPropertyDetails:updatedPropertyDetails"]["response"]["homeDescription"];
                ViewBag.Image = JsonData["UpdatedPropertyDetails:updatedPropertyDetails"]["response"]["images"]["image"]["url"];
                
            }
            catch (Exception e)
            {
                ViewBag.Error = "JSON Issue";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }

            return View();
        }
    }
}