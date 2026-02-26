using BoxHub.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoxHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly BoxHubDbContext _context;

        public TestController(BoxHubDbContext context)
        {
            _context = context;
        }

        [HttpGet("db")]
        public async Task<IActionResult> TestDb()
        {
            var canConnect = await _context.Database.CanConnectAsync();
            return Ok(new { connected = canConnect });
        }

        [HttpGet("debug-conn")]
        public IActionResult DebugConnection()
        {
            var conn = _context.Database.GetConnectionString();
            return Ok(conn);
        }
    }
}
