using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horsch.WebTools.Library.JsonToAnon
{
    public class AnonBuilder
    {
        public string Build(JToken json)
        {
            var builder = new StringBuilder("var modelData = ");

            builder.AppendLine(CompileToken(json).ToString());

            builder.AppendLine(";");
            return builder.ToString();
        }

        protected string CompileToken(JToken json)
        {
            var builder = new StringBuilder();

            switch(json.Type)
            {
                case JTokenType.Array:
                    builder.AppendLine(AddArray((JArray)json));
                    break;
                case JTokenType.Object:
                    builder.AppendLine(AddObject((JObject)json));
                    break;
                case JTokenType.Property:
                    builder.AppendLine(AddProperty((JProperty)json));
                    break;
                default:
                    builder.AppendLine(AddValue(json));
                    break;
            }

            return builder.ToString();
        }

        protected string AddArray(JArray array)
        {
            var builder = new StringBuilder();
            builder.AppendLine("new object[] {");

            foreach (var obj in array)
            {
                builder.Append(CompileToken(obj));
                builder.AppendLine(",");
            }

            builder.AppendLine("}");
            return builder.ToString();
        }

        protected string AddObject(JObject obj)
        {
            var builder = new StringBuilder();
            builder.AppendLine("new {");

            foreach (var child in obj.Children())
            {
                builder.Append(CompileToken(child));
            }

            builder.AppendLine("}");
            return builder.ToString();
        }

        protected string AddProperty(JProperty property)
        {
            var builder = new StringBuilder();

            builder.AppendLine(string.Format("{0} = {1},", property.Name, CompileToken(property.Value)));

            return builder.ToString();
        }

        protected string AddValue(JToken value)
        {
            var builder = new StringBuilder();
            if (value.Type == JTokenType.String)
            {
                builder.Append("\"");
            }

            builder.Append(value.ToString());

            if (value.Type == JTokenType.String)
            {
                builder.Append("\"");
            }
            return builder.ToString();
        }

        protected void blah()
        {
            //var modelData = new { lol = "rofl" , lmao = new { pizza = True , party = "time!" , } , stfu = new object[] { new { geek = "dork" , } , new { nerd = "doofus" , } , } , } ;
            //var modelData = new { lol = "rofl", lmao = new { pizza = true, party = "time!", }, stfu = new object[] { new { geek = "dork", }, new { nerd = "doofus", }, }, };
        }
    }
}
