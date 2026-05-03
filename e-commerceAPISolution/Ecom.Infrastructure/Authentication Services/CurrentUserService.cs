using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Ecom.Infrastructure.Authentication_Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
				var userIdClaim = _httpContextAccessor.HttpContext?.User?
					.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
					?? _httpContextAccessor.HttpContext?.User?
					.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				if (Guid.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }

                return null;
            }
        }
    }
}