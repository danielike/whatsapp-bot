using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;
using WhatsappBot.Command;
using WhatsappBot.Common;
using WhatsappBot.DomManipulator;
using WhatsappBot.ExternalApis.Evolution;
using WhatsappBot.ExternalApis.Flaresolverr;
using WhatsappBot.MessageWorker;
using WhatsappBot.Options;
using WhatsappBot.RandomContentGenerator;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, _, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder
    .Services
    .Configure<ConfigurationOptions>(builder.Configuration.GetSection(nameof(ConfigurationOptions)))
    .AddMvc()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    });

var configurationOptions = builder.Configuration.GetSection(nameof(ConfigurationOptions)).Get<ConfigurationOptions>();

builder.Services.AddHttpClient(
    nameof(FlaresolverrApi),
    client =>
    {
        client.BaseAddress = new Uri($"{configurationOptions!.FlaresolverrUrl}");
        client.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
    });

builder.Services.AddHttpClient(
    nameof(EvolutionApiService),
    client =>
    {
        client.BaseAddress = new Uri($"{configurationOptions!.EvolutionApiUrl}");
        client.DefaultRequestHeaders.Add("Accept" , MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("apikey", $"{configurationOptions!.EvolutionApiKey}");
    });

builder.Services.AddHttpClient(
    HttpClientNames.EvolutionApiTranscriber,
    client =>
    {
        client.BaseAddress = new Uri($"{configurationOptions!.EvolutionApiTranscriberUrl}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configurationOptions!.EvolutionApiTranscriberKey);
    });

builder.Services.AddScoped<IEvolutionDelayCalculator, EvolutionDelayCalculator>();
builder.Services.AddScoped<IEvolutionApiService, EvolutionApiService>();
builder.Services.AddScoped<ICommandService, CommandService>();
builder.Services.AddSingleton<IFlaresolverrApi, FlaresolverrApi>();
builder.Services.AddSingleton<IDomManipulator, DomManipulator>();
builder.Services.AddSingleton<IRandomContentGenerator, RandomContentGenerator>();
builder.Services.AddSingleton<IMessageQueue, MessageQueue>();
builder.Services.AddHostedService<MessageWorker>();

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

app.MapControllers();

app.Run();