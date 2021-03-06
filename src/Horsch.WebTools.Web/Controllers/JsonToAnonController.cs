﻿using Horsch.WebTools.Library.JsonToAnon;
using Horsch.WebTools.Web.Models.View.JsonToAnon;
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
            AnonBuilderResult anon;
            bool isInvalid = false;

            try
            {
                anon = builder.Build(json);
            }
            catch (Exception ex)
            {
                isInvalid = true;
                anon = null;
            }

            var model = new IndexModel() { Anon = anon, Json = json, IsInvalid = isInvalid };

            return View(model);
        }
    }
}