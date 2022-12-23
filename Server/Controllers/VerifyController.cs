using Craftorio.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Craftorio.Server.Controllers
{
    [Route("api/verify")]
    [ApiController]
    public class VerifyController : ControllerBase
    {
        [HttpPost]
        public ActionResult Post(string username)
        {
            if (true)
            {
                SessionController sessionController = new SessionController();
                if (sessionController.IsLogged(username)){
                    return Content("OK");
                }
                else
                {
                    return Content("NOK");
                }
            }
            else
            {
                return Content("NOK");
            }
        }
    }
}
