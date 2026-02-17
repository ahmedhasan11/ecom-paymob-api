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
		public async Task<RefreshTokenResultDto> GenerateRefreshTokenAsync(DateTime expiresAt)
		{
			byte[] bytes = new byte[64];
			using var randomnumber = RandomNumberGenerator.Create();
			randomnumber.GetBytes(bytes);
			string RAWtoken = Convert.ToBase64String(bytes); //RAW TOKEN AS A STRING
			

			byte[] rawBytes = Encoding.UTF8.GetBytes(RAWtoken); //computer doesnt understand strings , Conversion Operation
			using var sha = SHA256.Create(); //ready of SHA machine
			byte[] hashBytes= sha.ComputeHash(rawBytes); //give SHA the result to make the hash

			var HashedToken = Convert.ToBase64String(hashBytes); //Hashed Token AS A STRING

			//SECURITY INFO : iff someond got thr hashed token , he cant convert it back to Raw 
			//HASH is one way ,, Not-Reversable

			return new RefreshTokenResultDto() { RawToken=RAWtoken, HashedToken=HashedToken, ExpiresAt=expiresAt };
		}
	}
}
