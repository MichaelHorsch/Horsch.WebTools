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
        StringBuilder outsideBuilder = null;

        public string Build(JToken json)
        {
            outsideBuilder = new StringBuilder();
            var builder = new StringBuilder("var modelData = ");

            builder = builder.AppendLine(CompileToken(json).ToString());

            builder = builder.AppendLine(";");
            outsideBuilder = outsideBuilder.AppendLine(builder.ToString());
            return outsideBuilder.ToString();
        }

        protected string CompileToken(JToken json, string propertyName = null)
        {
            var builder = new StringBuilder();

            switch(json.Type)
            {
                case JTokenType.Array:
                    builder = builder.AppendLine(AddArray((JArray)json, propertyName));
                    break;
                case JTokenType.Object:
                    builder = builder.AppendLine(AddObject((JObject)json));
                    break;
                case JTokenType.Property:
                    builder = builder.AppendLine(AddProperty((JProperty)json));
                    break;
                default:
                    builder = builder.AppendLine(AddValue(json));
                    break;
            }

            return builder.ToString();
        }

        protected string AddArray(JArray array, string propertyName = null)
        {
            string result;
            if (!string.IsNullOrEmpty(propertyName))
            {
                result = AddArrayExternal(array, propertyName);
            }
            else
            {
                result = AddArrayInternal(array);
            }

            return result;
        }

        protected string AddArrayInternal(JArray array)
        {
            var builder = new StringBuilder();
            builder = builder.AppendLine("new List<object>() {");

            foreach (var obj in array)
            {
                builder = builder.Append(CompileToken(obj));
                builder = builder.AppendLine(",");
            }

            builder = builder.AppendLine("}");
            return builder.ToString();
        }

        protected string AddArrayExternal(JArray array, string propertyName)
        {
            var builder = new StringBuilder();
            var variableName = string.Format("{0}List", propertyName);
            builder = builder.AppendLine(string.Format("var {0} = new List<object>();", variableName));

            builder = builder.AppendLine(string.Format("foreach(var item in {0}) {{", propertyName));

            builder = builder.AppendLine("// dear reader: we are only accounting for the first object type found in the json.  If this is a multitype array, you need to handle that manually.");

            var first = array.FirstOrDefault();
            if (first != null)
            {
                builder = builder.AppendLine(string.Format("{0}.Add({1});", variableName, CompileToken(first)));
            }

            builder = builder.AppendLine("}");

            outsideBuilder = outsideBuilder.AppendLine(builder.ToString());

            return variableName;
        }

        protected string AddObject(JObject obj)
        {
            var builder = new StringBuilder();
            builder = builder.AppendLine("new {");

            foreach (var child in obj.Children())
            {
                builder = builder.Append(CompileToken(child));
            }

            builder = builder.AppendLine("}");
            return builder.ToString();
        }

        protected string AddProperty(JProperty property)
        {
            var builder = new StringBuilder();

            builder = builder.AppendLine(string.Format("{0} = {1},", property.Name, CompileToken(property.Value, property.Name)));

            return builder.ToString();
        }

        protected string AddValue(JToken value)
        {
            var builder = new StringBuilder();

            switch (value.Type)
            {
                case JTokenType.String:
                    builder = builder.AppendFormat("\"{0}\"", value.ToString());
                    break;
                case JTokenType.Boolean:
                    builder = builder.Append(value.ToString().ToLower());
                    break;
                default:
                    builder = builder.Append(value.ToString());
                    break;
            }

            return builder.ToString();
        }

        //protected void blah()
        //{
        //    var buttList = new List<object>(); 
        //    foreach(var item in butt) 
        //    { // dear reader: we are only accounting for the first object type found in the json. If this is a multitype array, you need to handle that manually. 
        //        buttList.Add(new { dick = "head" , } ); 
        //    } 
        //    var stfuList = new List<object>(); 
        //    foreach(var item in stfu) 
        //    { // dear reader: we are only accounting for the first object type found in the json. If this is a multitype array, you need to handle that manually. 
        //        stfuList.Add(new { geek = "dork" , butt = buttList , } ); 
        //    } 
        //    var modelData = new { lol = "rofl" , lmao = new { pizza = true , party = "time!" , } , stfu = stfuList , } ; 
        //}
    }
}
