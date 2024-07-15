namespace pfie.OpenApi.Parser;


public record ResponseDef(int Status, string Description, ContentDef? Content);

