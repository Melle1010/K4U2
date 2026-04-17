using LLM_Proxy_API.Middlewares;
using OllamaSharp;
using Scalar.AspNetCore;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var ollamaUrl = new Uri("https://ollama.com");
var apiKey = builder.Configuration["OllamaApiKey"] ?? throw new Exception("Ollama API key is not configured.");

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

builder.Services.AddOpenApi();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();



app.MapControllers();

app.Run();
