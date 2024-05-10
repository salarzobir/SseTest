using Microsoft.AspNetCore.Mvc;
using SseTest.Services;

namespace SseTest.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(UserService userService, ISseService sseService) : ControllerBase
{
    [HttpGet("subscribe")]
    public async Task Subscribe()
    {
        try
        {
            await sseService.RegisterClientAsync("testClientId", Response);
        }
        catch
        {
        }
    }

    [HttpGet]
    public string? Get()
    {
        return userService.Get();
    }

    [HttpPut]
    public IActionResult Put(string newName)
    {
        userService.Put(newName);
        return Ok();
    }

    [HttpPost]
    public IActionResult Post()
    {
        Parallel.ForEach(Enumerable.Range(0, 1000), _ => userService.Put(Guid.NewGuid().ToString()));
        return Ok();
    }
}
