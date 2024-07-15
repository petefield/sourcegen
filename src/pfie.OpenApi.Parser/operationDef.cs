namespace pfie.OpenApi.Parser;


public record OperationDef(string Name, string Method, string Path, ContentDef? RequestBodyContent, ResponseDef[] Responses, parameterDef[] PathParameters);

