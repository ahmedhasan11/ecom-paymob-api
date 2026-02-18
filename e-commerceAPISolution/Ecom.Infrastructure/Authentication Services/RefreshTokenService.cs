using Ecom.Application.DTOs.Authentication.RefreshToken;
using Ecom.Application.Interfaces;
using Ecom.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Authentication_Services
{
	public class RefreshTokenService : IRefreshTokenService
	{

		public RefreshTokenResultDto GenerateRefreshTokenAsync(DateTime expiresAt)
		{
			byte[] bytes = new byte[64];
			using var randomnumber = RandomNumberGenerator.Create();
			randomnumber.GetBytes(bytes);
			string RAWtoken = Convert.ToBase64String(bytes); //RAW TOKEN AS A STRING


			string HashedToken = HashToken(RAWtoken);

			//SECURITY INFO : iff someond got thr hashed token , he cant convert it back to Raw 
			//HASH is one way ,, Not-Reversable

			return new RefreshTokenResultDto() { RawToken=RAWtoken, HashedToken=HashedToken, ExpiresAt=expiresAt };
		}

		public string HashToken(string rawToken)
		{
			byte[] rawBytes = Encoding.UTF8.GetBytes(rawToken);
			using var sha = SHA256.Create();
			byte[] HashedBytes= sha.ComputeHash(rawBytes);
			string HashedToken = Convert.ToBase64String(HashedBytes);

			return HashedToken;
		}
	}
}
