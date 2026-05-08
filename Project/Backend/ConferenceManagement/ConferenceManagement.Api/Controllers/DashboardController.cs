using Microsoft.AspNetCore.Mvc;
using ConferenceManagement.Application.Interfaces;
using System.Threading.Tasks;

namespace ConferenceManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IUserService _userService;

        public DashboardController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("user-count")]
        public async Task<IActionResult> GetUserCount()
        {
            var count = await _userService.GetUserCountAsync();
            return Ok(new { userCount = count });
        }
    }
}
