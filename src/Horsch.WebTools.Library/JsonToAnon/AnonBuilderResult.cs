using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horsch.WebTools.Library.JsonToAnon
{
    public class AnonBuilderResult
    {
        public string BuilderString { get; set; }
        public IEnumerable<string> SupportMethods { get; set; }
    }
}
