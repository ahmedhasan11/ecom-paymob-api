using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Entities
{
	public class RefreshToken
	{
		public Guid Id { get; private set; }
		public Guid UserId { get; private set; }
		public string HashedToken { get; private set; }

		public DateTime CreatedAt { get; private set; }
		public DateTime ExpiresAt { get; private set; }

		public bool IsRevoked { get; private set; }
		public DateTime? RevokedAt { get; private set; }

		public bool IsExpired => DateTime.UtcNow >= ExpiresAt;      // Computed Property (NOT stored in DB)
		private RefreshToken() { }

		public RefreshToken(Guid userid , string hashedtoken , DateTime expiresAt)
		{
			Id= Guid.NewGuid();
			UserId= userid;
			HashedToken= hashedtoken;
			CreatedAt = DateTime.UtcNow;
			ExpiresAt= expiresAt;
			IsRevoked = false;
		}
		public bool CanBeUsed()
		{
			if (IsRevoked==false && IsExpired==false)
			{
				return true;
			}
			return false;
		}

		public void Revoke()
		{
			if (IsRevoked)
				return;
			IsRevoked = true;
			RevokedAt = DateTime.UtcNow;
		}
		
	}
}
