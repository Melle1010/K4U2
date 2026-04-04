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
        public MessagesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(CreateMessageRequest request)
        {
            Message message = new Message();
            message.Text = request.Text;

            _dbContext.Add(message);
            await _dbContext.SaveChangesAsync();

            return Ok(message);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMessages()
        {
            List<Message> messages = await _dbContext.Set<Message>().ToListAsync();
            return Ok(messages);
        }
    }
}
