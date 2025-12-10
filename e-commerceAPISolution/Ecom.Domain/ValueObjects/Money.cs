using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.ValueObjects
{
	/// <summary>
	/// Money value object - immutable, validates non-negative amount.
	/// Simple and safe representation for currency amounts.
	/// (We use a single-currency approach for now.)
	/// </summary>
	/// 
	/// 
	/// 
	/// sealed-->ممنوع حد يرث (extends) منه. ده مهم للـ Value Object عشان نمنع تغيّرات غير متوقعة عن طريق الوراثة.
	public sealed class Money:IEquatable<Money>
	{
		public decimal Amount { get; }

		private Money(decimal amount)
		{
			Amount = Decimal.Round(amount, MidpointRounding.AwayFromZero);
			//يضمن سلوك تقريب ثابت (مثلاً 2.345 => 2.35). مهم علشان الحسابات المالية تبقى متوقعة.
		}

		public static Money From(Decimal amount)
		{
			if (amount < 0)
				throw new ArgumentException("Amount cannot be negative.");

			return new Money(amount);
		}

		public Money Add(Money other)
		{
			return new Money(Amount + other.Amount);
		}

		public override bool Equals(object? obj) => Equals(obj as Money); /*دي معمولة علشان:

لو حد قارن Money بـ object
C# يحوّله لـ Money وبعدين ينادي Equals(Money).

فهي مجرد كوبري.*/

		public bool Equals(Money? other)
		{
			if (other is null) return false;

			return Amount == other.Amount;
		}

		public override int GetHashCode() => Amount.GetHashCode();
	}
}
