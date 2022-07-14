using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected OkObjectResult Ok(dynamic? dataResponse = default, string? messageResponse = default)
        {
            return base.Ok(new { data = dataResponse, message = messageResponse });
        }

        protected BadRequestObjectResult BadRequest(string? messageResponse = default)
        {
            return base.BadRequest(new { message = messageResponse });
        }
    }
}
