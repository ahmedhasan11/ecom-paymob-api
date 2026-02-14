using Ecom.Application.Common.Settings;
using Ecom.Application.DTOs.Authentication;
using Ecom.Application.Interfaces;
using Ecom.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Authentication_Services
{
	public class JwtService : IJwtService
	{
		private readonly JwtSettings _jwtSettings;
		public JwtService(IOptions<JwtSettings> options)
		{
			_jwtSettings=options.Value;
		}
		public async Task<JwtResultDto> GenerateTokenAsync(JwtUserDataDto dto)
		{
			var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
			var secretkey = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
			var Issuer = _jwtSettings.Issuer;
			var Audience = _jwtSettings.Audience;

			List<Claim> tokenclaims =  new List<Claim>()
				{
				new Claim(JwtRegisteredClaimNames.Sub, dto.UserId.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				//new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
				new Claim(ClaimTypes.Name, dto.FullName),
				new Claim(ClaimTypes.Email, dto.Email)			
				};
			foreach (var role in dto.Roles)
			{
				tokenclaims.Add(new Claim(ClaimTypes.Role,role));
			}

			SymmetricSecurityKey key = new SymmetricSecurityKey(secretkey);
			SigningCredentials signingcredentials = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);
			JwtSecurityToken token = new JwtSecurityToken( issuer:Issuer, audience:Audience,
				claims:tokenclaims, expires:expiration, signingCredentials:signingcredentials);

			JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
			string JwtToken = handler.WriteToken(token);

			return new JwtResultDto() { Token= JwtToken, ExpiresAt= expiration};

			
		}
	}
}
