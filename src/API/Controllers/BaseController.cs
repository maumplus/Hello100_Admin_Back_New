using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        //protected IMediator Mediator =>
        //    HttpContext.RequestServices.GetRequiredService<IMediator>();

        protected string AId => User.FindFirst("sub")?.Value ?? "";
        protected string AccId => User.FindFirst("account_id")?.Value ?? "";
        protected string HospNo => User.FindFirst("hospital_number")?.Value ?? "";
        protected string HospKey => User.FindFirst("hospital_key")?.Value ?? "";
        protected string Role => User.FindFirst(ClaimTypes.Role)?.Value ?? "";
    }
}
