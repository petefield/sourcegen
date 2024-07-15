using pfie.OpenApi.Parser;
using System.Text;

namespace pfie.http.sourcegen
{
    public class CSharpClientBuilder
    {
        public  static string Build(ServiceDefinition service)
        {
            var sb = new StringBuilder();
            
           sb.AppendLine($"public partial class {service.Name} :  HttpServiceBase {{");
            sb.AppendLine();


            sb.AppendLine($"\tpublic AnimalFactsApiClient(HttpClient client, ILogger<{service.Name}> logger) : base(client, logger){{}}");

            sb.AppendLine($"\tpublic override string ServiceName =>\"{service.Name}\";");


            foreach (var operation in service.Operations)
            {
                sb.AppendLine();
                sb.AppendLine($"\tinternal async Task<{ResultType(operation)}> {operation.Name}({methodParams(operation)}) {{");
                sb.AppendLine($"\t\tvar response = await {operation.Method}{ResponseType(operation)}($\"{operation.Path}\"{RequestBodyParameters(operation)});");
                sb.AppendLine($"\t\treturn response;");
                sb.AppendLine($"\t}}");
            }
            sb.AppendLine("}");

            sb.AppendLine();

            sb.AppendLine();

            foreach (var schema in service.Schemas)
            {
                sb.AppendLine($"internal class {schema.Name} {{");

                foreach (var prop in schema.Properties)
                {

                    sb.AppendLine($"   public {MapType(prop)} {prop.Name} {{ get; set; }}");
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


        private static string methodParams(OperationDef operation)
        {
            return string.Concat(PathParameters(operation), BodyMethodParameters(operation)).TrimEnd(' ', ',');


        }

        private static string PathParameters(OperationDef operation)
        {
            var s = string.Empty;

            foreach (var p in operation.PathParameters)
            {
                s += $"{MapType(p)} {p.Name}, ";
            }

            return s.TrimEnd(' ', ',');

        }

        private static string BodyMethodParameters(OperationDef operation)
        {
            var requestBodySchema = operation?.RequestBodyContent?.Schema;

            return (requestBodySchema != null)
                ? $"{requestBodySchema} body"
                : string.Empty;
        }

        private static string RequestBodyParameters(OperationDef operation)
        {
            var requestBodySchema = operation?.RequestBodyContent?.Schema;

            return (requestBodySchema != null)
                ? ", body"
                : string.Empty;

        }

        private static string ResponseType(OperationDef operation)
        {
            var responseContent = operation.Responses.First().Content;
            return responseContent == null
                ? string.Empty
                : responseContent.Type == "array"
                    ? $"<{responseContent.Schema}[]>"
                    : $"<{responseContent.Schema}>";
        }

        private static string ResultType(OperationDef operation)
        {
            var responseType = ResponseType(operation);
            return string.IsNullOrWhiteSpace(responseType)
                ? "Result"
                : $"ResultOf{responseType}";
        }

    }
    
}
