using Ecom.Application.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface IJwtService
	{
		Task<JwtResultDto> GenerateTokenAsync(JwtUserDataDto dto);
	}
}
