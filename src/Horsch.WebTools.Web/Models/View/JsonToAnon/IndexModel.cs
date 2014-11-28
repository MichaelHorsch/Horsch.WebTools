using Horsch.WebTools.Library.JsonToAnon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Horsch.WebTools.Web.Models.View.JsonToAnon
{
    public class IndexModel
    {
        public string Json { get; set; }
        public AnonBuilderResult Anon { get; set; }
        public bool IsInvalid { get; set; }
    }
}