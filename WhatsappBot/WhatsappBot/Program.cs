using System.Text.Json;
using Serilog;
using WhatsappBot;
using WhatsappBot.Command;
using WhatsappBot.DomManipulator;
using WhatsappBot.ExternalApis.Evolution;
using WhatsappBot.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, _, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder
    .Services
    .Configure<ConfigurationOptions>(builder.Configuration.GetSection(nameof(ConfigurationOptions)))
    .AddHostedService<GenerateRandomContent>()
    .AddMvc()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });;

builder.Services.AddHttpClient(
    nameof(EvolutionApiService),
    client =>
    {
        client.BaseAddress = new Uri($"{builder.Configuration["ConfigurationOptions:EvolutionApiKey"]}");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("apikey", $"{builder.Configuration["ConfigurationOptions:EvolutionApiKey"]}");
    });
 
builder.Services.AddScoped<IEvolutionDelayCalculator, EvolutionDelayCalculator>();
builder.Services.AddScoped<IEvolutionApiService, EvolutionApiService>();
builder.Services.AddScoped<ICommandService, CommandService>();
builder.Services.AddSingleton<IDomManipulator, DomManipulator>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");
 
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}