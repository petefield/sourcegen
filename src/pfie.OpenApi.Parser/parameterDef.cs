namespace pfie.OpenApi.Parser;

public record parameterDef (
        string Name,
     bool Required,
     string Type,
     string Format
);

