using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace pfie.OpenApi.Parser
{
    public class OpenApiParser
    {
        public static async Task<ServiceDefinition> Parse(string serviceName, string url)
        {

            var httpClient = new HttpClient();

            var openApiStream = await httpClient.GetStreamAsync(url);

            var openApiDocument = new OpenApiStreamReader().Read(openApiStream, out var diagnostic);


            var def = new ServiceDefinition(serviceName);

            foreach (var schema in openApiDocument.Components.Schemas)
            {
                var propertyDefs = schema.Value.Properties.Select(x => new PropertyDef(x.Key, x.Value.Type, !x.Value.Nullable)).ToArray();

                var schemadef = new SchemaDef(schema.Key, propertyDefs);

                def.Schemas.Add(schemadef);
            }


            foreach (var path in openApiDocument.Paths)
            {
                foreach (var op in path.Value.Operations)
                {

                    var responseDefs = new List<ResponseDef>();

                    foreach (var r in op.Value.Responses)
                    {
                        var status = int.Parse(r.Key);
                        var description = r.Value.Description;

                        ContentDef? content;


                        if (!r.Value.Content?.Any() ?? false)
                            content = null;

                        else
                        {
                            var s = r.Value.Content.First().Value!;

                            if (s.Schema.Items is null)
                                content = new ContentDef(s.Schema.Type, s.Schema.Reference.Id, ((OpenApiString)s.Example)?.Value?.ToString());
                            else
                                content = new ContentDef(s.Schema.Type, s.Schema.Items.Reference.Id, null);
                  
                        }

                        responseDefs.Add(new ResponseDef(status, description, content));

                    }

                    ContentDef? requestBodyContent;


                    if (op.Value.RequestBody is not null)
                    {
                        var reqeuestBodySchema = op.Value.RequestBody.Content.First().Value.Schema;
                        requestBodyContent = new ContentDef(reqeuestBodySchema.Type, reqeuestBodySchema.Reference.Id, null);
                    }
                    else
                        requestBodyContent = null;

                    parameterDef[] pathParameters = Array.Empty<parameterDef>();

                    if (op.Value.Parameters.Any())
                    {
                        pathParameters = op.Value.Parameters
                            .Where( x => x.In == ParameterLocation.Path)
                            .Select(x => new parameterDef(x.Name, x.Required, x.Schema.Type, x.Schema.Format)).ToArray();
                    }

                    var operationDef = new OperationDef(op.Value.OperationId, op.Key.ToString(), path.Key, requestBodyContent, responseDefs.ToArray(), pathParameters);


                    def.Operations.Add(operationDef);
                }
            }

            return def;
        }
    }
}
