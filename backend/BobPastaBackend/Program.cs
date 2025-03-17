using BobPastaBackend;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

//TODO: set to use only frontend url
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration["MongoDB:DatabaseName"];
    return client.GetDatabase(databaseName);
});

builder.Services.AddSingleton<BobPastaService>();

var app = builder.Build();

app.UseCors("AllowAll");

app.MapGet("/bobpasta", async (BobPastaService bobpastaService) =>
{
    return Results.Ok(await bobpastaService.GetBobpastosAsync());
});

app.MapPost("/bobpasta", async (BobPasto newBobPasto, BobPastaService bobpastaService) =>
{
    return Results.Created("", await bobpastaService.CreateBobPastoAsync(newBobPasto));
});

app.Run();

//1 - text 2 - file
public record BobPasto([property: JsonIgnore] ObjectId _id, string Name, string Content, int Type = 1);

[JsonSerializable(typeof(List<BobPasto>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
