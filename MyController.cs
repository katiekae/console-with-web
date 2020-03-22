using Microsoft.AspNetCore.Mvc;

namespace ConsoleWithWeb
{
    public class MyController : Controller
    {
        private readonly BridgeService _bridgeService;

        public MyController(BridgeService bridgeService)
        {
            _bridgeService = bridgeService;
        }

        [Route("/latest_value")]
        [HttpGet]
        public IActionResult GetLatestValue()
        {
            var latest = _bridgeService.LatestValue;
            if (latest != null)
            {
                return Content(latest.ToString());
            }
            else
            {
                return NotFound();
            }
        }
    }
}
