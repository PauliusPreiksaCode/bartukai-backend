using Microsoft.AspNetCore.Mvc;

namespace portal_backend.Controllers;


public class BaseController : ControllerBase
{
    [HttpGet]
    [Route("hello-world")]
    public async Task<IActionResult> GetAllRoomsList()
    {
        return Ok("Hello world!");
    }
}