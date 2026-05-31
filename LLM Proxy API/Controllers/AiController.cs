using System.Net;
using Microsoft.AspNetCore.Mvc;
using OllamaSharp;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IOllamaApiClient _ollama;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AiController> _logger;
    private readonly string promptPreset = "You are a helpful assistant that provides concise and accurate answers to user questions. Always respond in a clear and informative manner, ensuring that your answers are relevant to the user's query. If you don't know the answer, say you don't know instead of making something up. Be polite and professional in your responses.";

    public AiController(IOllamaApiClient ollama, IHttpClientFactory httpClientFactory, ILogger<AiController> logger)
    {
        _ollama = ollama;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost("ask-and-save")]
    public async Task<IActionResult> AskAndSave([FromBody] string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return BadRequest("Prompt cannot be empty.");
        }

        _ollama.SelectedModel = "gemma3:4b";
        var fullResponse = string.Empty;
        var client = _httpClientFactory.CreateClient("ContentApiClient");

        try
        {
            var promptSaveResponse = await client.PostAsJsonAsync("api/messages/create-message", new { Text = prompt });

            if (!promptSaveResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to save prompt to Content API. StatusCode: {StatusCode}", promptSaveResponse.StatusCode);
                return StatusCode((int)HttpStatusCode.BadGateway, new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadGateway,
                    Title = "Content API error",
                    Detail = "Failed to save prompt to Content API.",
                    Instance = HttpContext.Request.Path
                });
            }

            await foreach (var stream in _ollama.GenerateAsync(promptPreset+prompt))
            {
                fullResponse += stream.Response;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while generating AI response for /api/ai/ask-and-save.");

            var problem = new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadGateway,
                Title = "AI service error",
                Detail = "The AI service failed to generate a response.",
                Instance = HttpContext.Request.Path
            };

            return StatusCode(problem.Status.Value, problem);
        }

        

        var contentRequest = new { Text = fullResponse };

        var response = await client.PostAsJsonAsync("api/messages/create-message", contentRequest);

        if (response.IsSuccessStatusCode)
        {
            
            return Ok(new
            {
                Status = "Saved to Content API",
                Data = contentRequest.Text
            });
        }

        return StatusCode((int)response.StatusCode, "AI generated a response, but Content API rejected it.");
    }
}