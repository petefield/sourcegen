using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;

namespace pfie.http;

[GenerateOneOf]
public partial class ResultOf<T> : OneOfBase<T, Failure>
{
    public bool Succeeded => base.IsT0;

    public bool Failed => base.IsT1;

    public new T Value => this.AsT0;

    public Failure Failure => this.AsT1;

}


[GenerateOneOf]
public partial class Result : OneOfBase<Success, Failure>
{
    public bool Succeeded => base.IsT0;

    public bool Failed => base.IsT1;

    public Failure Failure => this.AsT1;
}


[GenerateOneOf]
public partial class Failure : OneOfBase<BadGateway, BadRequest, ServiceUnavailable, NotFound, Conflict, InternalServerError> {

    public static Failure From(Exception ex) => new(new InternalServerError( ex.Message, ex));
    public static Failure From(NotFound _) => new(_);
    public static Failure From(Conflict _) => new(_);
    public static Failure From(BadGateway _) => new(_);
    public static Failure From(BadRequest _) => new(_);
    public static Failure From(ServiceUnavailable _) => new(_);

}


public record BadGateway(string message, Exception? ex = null);
public record BadRequest(string message, ValidationProblemDetails? pd = null);

public record ServiceUnavailable(string message, ServiceUnavailable? ex = null);
public record InternalServerError(string message, Exception? ex = null);
public record NotFound(string? message = null);
public record Conflict(string? message = null);
