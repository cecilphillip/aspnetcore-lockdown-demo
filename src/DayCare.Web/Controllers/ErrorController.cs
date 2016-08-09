namespace DayCare.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("[controller]")]
    public class ErrorController : Controller
    {
        [HttpGet("{statusCode}")]
        public IActionResult Index(int statusCode) => View("Error", statusCode);
    }
}
