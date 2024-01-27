using System.Reflection;
using FluentValidation;
using WilderMinds.MinimalApis.FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(Assembly.GetEntryAssembly());

var app = builder.Build();

app.MapPost("/test", (SomeModel model) => Results.Created("/test/1", model))
  .Validate<SomeModel>();

app.Run();


