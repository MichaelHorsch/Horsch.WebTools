using Horsch.WebTools.Library.Extensions;
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
        List<string> Methods = null;

        public AnonBuilderResult Build(string json)
        {
            Methods = new List<string>();
            var builder = new StringBuilder("var modelData = ");

            var token = JToken.Parse(json);

            builder = builder.Append(CompileToken(token, 0).ToString());
            builder = builder.AppendLine(";");

            AnonBuilderResult result = new AnonBuilderResult()
            {
                BuilderString = builder.ToString(),
                SupportMethods = Methods,
            };

            return result;
        }

        protected string CompileToken(JToken json, int indentIndex, string propertyName = null)
        {
            var builder = new StringBuilder();

            switch(json.Type)
            {
                case JTokenType.Array:
                    builder = builder.Append(AddArray((JArray)json, propertyName));
                    break;
                case JTokenType.Object:
                    builder = builder.Append(AddObject((JObject)json, indentIndex));
                    break;
                case JTokenType.Property:
                    builder = builder.Append(AddProperty((JProperty)json, indentIndex));
                    break;
                default:
                    builder = builder.Append(AddValue(json));
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
                builder = builder.Append(CompileToken(obj, 0));
                builder = builder.AppendLine(",");
            }

            builder = builder.AppendLine("}");
            return builder.ToString();
        }

        protected string AddArrayExternal(JArray array, string propertyName)
        {
            var builder = new StringBuilder();
            var methodName = string.Format("Get{0}List", propertyName.ToUpperFirstLetter());
            var variableName = string.Format("{0}List", propertyName);
            var indentIndex = 0;

            builder = builder.AppendLine(string.Format("protected IEnumerable<object> {0}(IEnumerable<{1}> dataSourceList) {{", methodName, propertyName));
            indentIndex++;

            builder = builder.AppendLine(string.Format("{0}var {1} = new List<object>();", Indents(indentIndex), variableName));
            builder = builder.AppendLine(string.Format("{0}foreach(var item in dataSourceList) {{", Indents(indentIndex)));

            var first = array.FirstOrDefault();
            if (first != null)
            {
                builder = builder.AppendLine(string.Format("{0}// dear reader: we are only accounting for the first object type found in the json.  If this is a multitype array, you need to handle that manually.", Indents(indentIndex + 1)));
                builder = builder.AppendLine(string.Format("{0}{1}.Add({2});", Indents(indentIndex + 1), variableName, CompileToken(first, indentIndex + 1)));
            }

            builder = builder.AppendLine(string.Format("{0}}}", Indents(indentIndex)));
            builder = builder.AppendLine(string.Format("{0}return {1};", Indents(indentIndex), variableName));
            indentIndex--;
            builder = builder.AppendLine("}");

            Methods.Add(builder.ToString());

            return string.Format("{0}({1})", methodName, variableName);
        }

        protected string AddObject(JObject obj, int indentIndex)
        {
            var builder = new StringBuilder();
            builder = builder.AppendLine("new {");

            foreach (var child in obj.Children())
            {
                builder = builder.Append(CompileToken(child, indentIndex + 1));
            }

            builder = builder.Append(string.Format("{0}}}", Indents(indentIndex)));
            return builder.ToString();
        }

        protected string AddProperty(JProperty property, int indentIndex)
        {
            var builder = new StringBuilder();

            builder = builder.AppendLine(string.Format("{0}{1} = {2},", Indents(indentIndex), property.Name, CompileToken(property.Value, indentIndex, property.Name)));

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

        protected string Indents(int index)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < index; i++)
            {
                builder.Append("    ");
            }

            return builder.ToString();
        }
    }
}
