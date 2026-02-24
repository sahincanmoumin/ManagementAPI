using Microsoft.AspNetCore.Mvc;
using EntityLayer.Extensions; 

namespace ApiLayer.Controller
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        
        protected int CurrentUserId => User.GetUserId();
        protected bool IsAdmin => User.IsAdmin();
    }
}