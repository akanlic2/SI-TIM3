using Microsoft.AspNetCore.Mvc;
using ConferenceManagement.Application.Services;
using System.Threading.Tasks;

namespace ConferenceManagement.Api.Modules
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardModule : ControllerBase
    {
        private readonly IUserService _userService;

        public DashboardModule(IUserService userService)
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
