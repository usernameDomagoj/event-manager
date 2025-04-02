using EventManager.Enums;
using Microsoft.AspNetCore.Authorization;

namespace EventManager.Services.Auth
{
    public class AuthorizeRoles : AuthorizeAttribute
    {

        public AuthorizeRoles(params UserRole[] roles)
        {
            Roles = string.Join(", ", roles);
        }
    }
}
