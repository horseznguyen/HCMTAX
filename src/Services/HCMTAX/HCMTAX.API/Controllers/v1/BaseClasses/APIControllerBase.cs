using Microsoft.AspNetCore.Mvc;
using Services.Common;

namespace HCMTAX.API.Controllers.v1.BaseClasses
{
    [ApiVersion("1")]
    [Route(Settings.APIDefaultRoute)]
    [ApiController]
    //[Authorize]
    public class APIControllerBase : ControllerBase
    {
    }
}