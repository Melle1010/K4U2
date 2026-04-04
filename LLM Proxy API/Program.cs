using OllamaSharp;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var ollamaUrl = new Uri("https://ollama.com");
var apiKey = "19757ca89b814494a2cae5d8d5d73396.5CUICh0wG7gnVM7G98cMmOxj";

builder.Services.AddScoped<IOllamaApiClient>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = ollamaUrl };
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

    return new OllamaApiClient(httpClient);
});

builder.Services.AddHttpClient("ContentApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5024/");
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();



var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
