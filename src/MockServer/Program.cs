using System.Reflection;
using System.Text.Json.Serialization;
using MockServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MockServer.Controllers;
using MockServer.Databases;
using MockServer.Middlewares;
using MockServer.Services;
using IziSwaggerSwashbuckle;

// using MockServer.Services;

namespace MockServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var config = new Config();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<Config>(x => config);
        builder.Services.AddSingleton<OnDemandUpdate>();    

        builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

#if DEBUG
        // builder.Services.AddSingleton<InitSeeder>();
        // builder.Services.AddHostedService<DebugService>();
#endif
        // builder.Services.AddHostedService<OrderProcessingService>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGenWithSpecificOfIziHardGames();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // http://<server-name>[:server-port]/api/v3/
        app.UsePathBase("/api/v3");

        app.UseAuthorization();

        app.MapControllers();
        app.MapGet("", () => Results.Ok("This is mock service"));

        app.UseMiddleware<ValidateHeadersMiddleware>();
        app.UseMiddleware<ValidateIdsMiddleware>();
        app.UseMiddleware<ValidateNotDefault>();
        app.Run();
    }
}