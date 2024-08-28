using pfie.OpenApi.Parser;
using System.Text;

namespace pfie.http.sourcegen
{
    public class CSharpMockBuilder
    {
        public  static string Build(ServiceDefinition service)
        {
            var sb = new StringBuilder();

            foreach (var operation in service.Operations)
            {
                sb.AppendLine($"public class {operation.Name} : RequestBuilder {{");

                sb.AppendLine($"\tpublic {operation.Name} WithMethod(HttpMethod method) => WithMethod(method, this);");
                sb.AppendLine();

                sb.AppendLine($"\tpublic {operation.Name} WithPath(string path) => WithPath(path, this);");
                sb.AppendLine();

                if (!string.IsNullOrEmpty( ResponseType(operation)))
                    sb.AppendLine($"\tpublic ResponseBuilder<{ResponseType(operation)}> Returns => new();");

                if (operation.PathParameters.Any())
                {
                    sb.AppendLine($"\tpublic class PathParameters  {{");

                    foreach (var par in operation.PathParameters)
                    {
                        sb.AppendLine($"\t\tpublic {MapType(par)} {par.Name} {{ get; set; }}");
                    }

                    sb.AppendLine($"\t}}");

                    sb.AppendLine();

                    sb.AppendLine($"\tPathParameters _pathParameters = new PathParameters();");

                    sb.AppendLine();

                    sb.AppendLine($"\tpublic {operation.Name} WithPathParams(Action<PathParameters> action) => SetPathParameters(action, _pathParameters, this);\r\n");

                }


                sb.AppendLine($"}}");
                sb.AppendLine();


               
            }
           
            foreach (var schema in service.Schemas)
            {
                sb.AppendLine($"public class {schema.Name} {{");

                foreach (var prop in schema.Properties)
                {

                    sb.AppendLine($"\tpublic {MapType(prop)} {prop.Name} {{ get; set; }}");
                }

                sb.AppendLine("}");


            }
            return sb.ToString();
        }

        public static string MapType(PropertyDef prop)
        {
            var type = prop.Type;

            if (Types.TryGetValue(prop.Type, out var t))
                type = t;

            return type;
        }

        public static string MapType(parameterDef p)
        {
            var type = p.Type;

            if (Types.TryGetValue(p.Type, out var t))
                type = t;

            return type;
        }

        public static Dictionary<string, string> Types = new Dictionary<string, string>() {
            {"integer", "int" }
        };

        private static string ResponseType(OperationDef operation)
        {
            var responseContent = operation.Responses.First().Content;
            return responseContent == null
                ? string.Empty
                : responseContent.Type == "array"
                    ? $"List<{responseContent.Schema}>"
                    : responseContent.Schema;
        }
    }
}
