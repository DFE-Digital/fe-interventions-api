using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dfe.FE.Interventions.Api.Controllers
{
    [ApiController]
    [Route("fe-providers")]
    public class FeProviderController : ControllerBase
    {
        private readonly ILogger<FeProviderController> _logger;

        public FeProviderController(ILogger<FeProviderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "hello there!";
        }
    }
}