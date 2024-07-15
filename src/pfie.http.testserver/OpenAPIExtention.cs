using Microsoft.OpenApi.Any;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace pfie.http.testserver
{
    public static class OpenAPIExtention
    {
        public static TBuilder WithOpenApiExample<TBuilder>(this TBuilder builder, HttpStatusCode status, object example) where TBuilder : IEndpointConventionBuilder
        {

            builder.WithOpenApi(c =>
          {
              var statusCodeKey = ((int)status).ToString();
              c.Responses[statusCodeKey].Content.First().Value.Example = new OpenApiString(JsonSerializer.Serialize(example));
              return c;
          });

            return builder;
        }
    }
}
