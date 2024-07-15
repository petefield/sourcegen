using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using pfie.http.testserver;
using pfie.http.testserver.animalfacts;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var facts = new List<string>()
{
    "Cats are small", 
    "Dogs are big", 
    "Horses are fast."
};

app.MapGet("/animal/fact", () => TypedResults.Ok(new AnimalFactResponse(facts[Random.Shared.Next(facts.Count)])))
    .WithName("GetFact")
    .WithOpenApiExample(HttpStatusCode.OK, new AnimalFactResponse("ExampleResponse"));

app.MapGet("/animal/fact/{id}", (int id) => TypedResults.Ok(new AnimalFactResponse(facts[id])))
    .WithName("GetFactById")
    .WithOpenApiExample(HttpStatusCode.OK, new AnimalFactResponse("ExampleResponse"));

app.MapGet("/animal/facts", () => TypedResults.Ok(facts.Select(fact => new AnimalFactResponse(fact))))
    .WithName("GetFacts")
    .WithOpenApi();

app.MapPost("/animal/fact", (AnimalFactRequest request) => { 
    facts.Add(request.Fact);
    return TypedResults.NoContent();
})
    .WithName("CreateFact")
    .WithOpenApi();

app.MapPost("/animal/fail", (AnimalFactRequest request) => {
    facts.Add(request.Fact);
    return TypedResults.BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]> { 
        { "Fact", new[] { "Must be something" } }
    }));
})
    .WithName("fails")
    .WithOpenApi();

app.Run();

