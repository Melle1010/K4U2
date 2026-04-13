using Content_API.Data;
using Content_API.DTOs;
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

            var response = await client.PostAsJsonAsync("api/ai/ask-and-save", prompt);

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
        public async Task<IActionResult> UpdateMessage(int id, UpdateMessageRequest request)
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
