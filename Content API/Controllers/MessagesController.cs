using Content_API.Data;
using Content_API.DTOs;
using Content_API.Exceptions;
using Content_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Content_API.Controllers{ 

    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(AppDbContext dbContext, IHttpClientFactory httpClientFactory, ILogger<MessagesController> logger)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost("send-a-prompt-to-ai-model")]
        public async Task<IActionResult> SendPromptToAi([FromBody] string prompt, [FromServices] IConfiguration configuration)
        {
            var client = _httpClientFactory.CreateClient("LLM_Proxy_Client");
            client.BaseAddress = new Uri("http://localhost:5118/");
            client.DefaultRequestHeaders.Add("X-API-KEY", configuration["ApiKey"]);

            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ValidationException("Prompt cannot be empty.");
            }

            HttpResponseMessage response;

            try
            {
                _logger.LogInformation("Sending prompt to LLM Proxy API at {Url}", client.BaseAddress);
                response = await client.PostAsJsonAsync("api/ai/ask-and-save", prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling LLM Proxy API.");
                throw new Exception("Failed to call LLM Proxy API.", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("LLM Proxy API returned non-success status code {StatusCode}. Body: {Body}", response.StatusCode, errorBody);
                throw new Exception($"LLM Proxy API responded with error code {(int)response.StatusCode}.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var aiResponse = $"AI response to: {prompt}\n- - - - - - - -\n{content}";
            return Ok(new {reply = aiResponse});
        }

        [HttpPost("create-message")]
        public async Task<IActionResult> CreateMessage(CreateMessageRequest request)
        {
            Message message = new Message();
            message.Text = request.Text;

            _dbContext.Add(message);
            await _dbContext.SaveChangesAsync();

            return Ok(message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMessage(Guid id, UpdateMessageRequest request)
        {
            var messageToUpdate = await _dbContext.Messages.FindAsync(id);

            if (messageToUpdate == null)
            {
                return NotFound($"Message with ID {id} was not found.");
            }

            messageToUpdate.Text = request.Text;

            await _dbContext.SaveChangesAsync();

            return Ok(messageToUpdate);
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var messageToDelete = await _dbContext.Messages.FindAsync(id);

            if (messageToDelete == null)
            {
                return NotFound($"Message with ID {id} was not found.");
            }

            _dbContext.Messages.Remove(messageToDelete);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Hämtar alla meddelanden med möjlighet att filtrera på datum och sortering.
        /// </summary>
        /// <param name="startDate">Startdatum för filtrering.</param>
        /// <param name="sort">Sorteringsordning (asc/desc).</param>
        /// <returns>En lista över meddelanden.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllMessages(
    [FromQuery] DateTime? startDate,
    [FromQuery] string? sort = "asc")
        {
            IQueryable<Message> query = _dbContext.Messages;

            if (startDate.HasValue)
            {
                query = query.Where(m => m.CreatedAt >= startDate.Value);
            }

            if (sort.ToLower() == "desc")
            {
                query = query.OrderByDescending(m => m.CreatedAt);
            }
            else
            {
                query = query.OrderBy(m => m.CreatedAt);
            }

            var result = await query.ToListAsync();

            return Ok(result);
        }
    }
}
