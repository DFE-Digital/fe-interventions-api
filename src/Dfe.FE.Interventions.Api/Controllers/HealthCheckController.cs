using Microsoft.AspNetCore.Mvc;

namespace Dfe.FE.Interventions.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [Route("")]
        public IActionResult GetRouteHeartbeat()
        {
            return Ok();
        }
    }
}