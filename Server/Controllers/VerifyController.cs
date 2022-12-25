using Craftorio.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Craftorio.Server.Controllers
{
    [Route("api/verify")]
    [ApiController]
    public class VerifyController : ControllerBase
    {
        private readonly ISessionController _sessionController;
        public VerifyController(ISessionController sessionController) {
            _sessionController = sessionController;
        }
        [HttpPost]
        public ActionResult Post(string username)
        {
            if (true)
            {
                if (_sessionController.IsLogged(username)){
                    return StatusCode(200);
                }
                else
                {
                    //Unauthorized
                    return StatusCode(401);
                }
            }
            else
            {
                return Content("NOK");
            }
        }
    }
}
