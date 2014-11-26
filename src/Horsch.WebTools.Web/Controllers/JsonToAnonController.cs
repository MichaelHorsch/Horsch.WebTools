using Horsch.WebTools.Library.JsonToAnon;
using Horsch.WebTools.Web.Models.View.JsonToAnon;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Horsch.WebTools.Web.Controllers
{
    public class JsonToAnonController : Controller
    {
        public ActionResult Index()
        {
            var model = new IndexModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string json)
        {
            var builder = new AnonBuilder();
            string anon = string.Empty;
            bool isInvalid = false;

            try
            {
                var jobject = JToken.Parse(json);
                anon = builder.Build(jobject);
            }
            catch (Exception ex)
            {
                isInvalid = true;
            }

            var model = new IndexModel() { Anon = anon, Json = json, IsInvalid = isInvalid };

            return View(model);
        }
    }
}