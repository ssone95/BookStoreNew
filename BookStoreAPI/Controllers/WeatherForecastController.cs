using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers;

[Authorize("Implicit")]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet("[action]")]
    public IActionResult Get()
    {
        return new JsonResult(new { Temperature = 32.5, Type = "Sunny" });
    }
}
