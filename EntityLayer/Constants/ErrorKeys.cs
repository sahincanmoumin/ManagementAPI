using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Constants
{
    public static class ErrorKeys
    {
        public const string AnimalNotFound = "AnimalNotFound";
        public const string AnimalBuyingFailed = "AnimalBuyingFailed";
        public const string ProductNotFound = "ProductNotFound";
        public const string UserNotFound = "UserNotFound";
        public const string UsernameAlreadyExists = "UsernameAlreadyExists";
        public const string InvalidCredentials = "InvalidCredentials";
        public const string RoleNotFound = "RoleNotFound";
        public const string UserAlreadyHasRole = "UserAlreadyHasRole";
        public const string UserDoesNotHaveRole = "UserDoesNotHaveRole";
        public const string InsufficientBalance = "InsufficientBalance";
        public const string ProductAlreadySold = "ProductAlreadySold";
        public const string FarmNotFound = "FarmNotFound";
        public const string UnauthorizedAction = "UnauthorizedAction";
    }
}