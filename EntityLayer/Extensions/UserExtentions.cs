using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace EntityLayer.Extensions
{
    
    public static class UserExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return value != null ? int.Parse(value) : 0;
        }
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }
    }
}