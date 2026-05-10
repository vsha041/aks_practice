using Microsoft.AspNetCore.Mvc;

namespace People.WebApp.Controllers
{
    [Route("api/info")]
    [ApiController]
    public class InfoApiController : ControllerBase
    {
        [HttpGet]
        public string GetMachineName()
        {
            return $"AKS / CI-CD : The request was processed by {Environment.MachineName}";
        }
    }
}
