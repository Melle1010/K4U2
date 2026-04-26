using Content_API.Data;
using Content_API.DTOs;
using Content_API.Exceptions;
using Content_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Content_API.Controllers{ 

    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        public MessagesController(AppDbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("send-a-prompt-to-ai-model")]
        public async Task<IActionResult> SendPromptToAi([FromBody] string prompt, [FromServices] IConfiguration configuration)
        {

            var client = _httpClientFactory.CreateClient("LLM_Proxy_Client");
            client.BaseAddress = new Uri("http://localhost:5118/");
            client.DefaultRequestHeaders.Add("X-API-KEY", configuration["ApiKey"]);

            if (string.IsNullOrEmpty(prompt)) throw new ValidationException("Prompt cannot be empty");

            var response = await client.PostAsJsonAsync("api/ai/ask-and-save", prompt);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("LLM Proxy API svarade med felkod.");
            }

            string aiResponse = $"AI response to: {prompt}\n- - - - - - - -\n{response}";
            return Ok(aiResponse);
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
