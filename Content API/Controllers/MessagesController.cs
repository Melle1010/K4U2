using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Content_API.Controllers{ 

    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController
    {
        private readonly DbContext _dbContext;
        public MessagesController(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
