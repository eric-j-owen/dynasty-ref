using Microsoft.AspNetCore.Mvc;

namespace ClientApi.Controllers;

[ApiController]
[Route("[controller]")]
public class Hello(ILogger<Hello> logger) : ControllerBase
{

    private readonly ILogger<Hello> _logger = logger;

    [HttpGet(Name = "GetHello")]
    public string Get()
    {
        _logger.LogInformation("hello from console");
        return "hello world";
    }


}
