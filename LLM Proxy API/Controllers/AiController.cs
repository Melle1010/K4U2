using Microsoft.AspNetCore.Mvc;
using OllamaSharp;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IOllamaApiClient _ollama;
    private readonly IHttpClientFactory _httpClientFactory;

    public AiController(IOllamaApiClient ollama, IHttpClientFactory httpClientFactory)
    {
        _ollama = ollama;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskAi([FromBody] string prompt)
    {
        _ollama.SelectedModel = "gemma3:4b";

        string fullResponse = "";

        await foreach (var stream in _ollama.GenerateAsync(prompt))
        {
            fullResponse += stream.Response;
        }

        return Ok(fullResponse);
       
    }

    [HttpPost("ask-and-save")]
    public async Task<IActionResult> AskAndSave([FromBody] string prompt)
    {
        _ollama.SelectedModel = "gemma3:4b";
        string fullResponse = "";
        var client = _httpClientFactory.CreateClient("ContentApiClient");

        await client.PostAsJsonAsync("api/messages/create-message", new { Text = prompt});

        await foreach (var stream in _ollama.GenerateAsync(prompt))
        {
            fullResponse += stream.Response;
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