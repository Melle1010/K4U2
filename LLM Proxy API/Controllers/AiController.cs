using Microsoft.AspNetCore.Mvc;
using OllamaSharp;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IOllamaApiClient _ollama;

    public AiController(IOllamaApiClient ollama)
    {
        _ollama = ollama;
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
}