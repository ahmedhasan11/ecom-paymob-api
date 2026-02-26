using Ecom.Domain.Common;
using Ecom.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Entities
{
	public class Product:AuditableEntity
	{
		public Guid Id { get; private set; } = Guid.NewGuid();
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public Money Price { get; set; } = null!;

		public string? ImageUrl { get; set; }

		public Guid? CategoryId { get; set; }
		public Category? Category { get; set; }

		public bool IsAvailable { get; private set; }

		public bool IsDeleted { get; private set; } 

		public int StockQuantity { get; private set; }
		public bool IsInStock => StockQuantity > 0;
		private Product() { } // For EF Core
		public Product(Decimal price , string name, int? InitialStock =null)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Name cannot be empty.", nameof(name));
			}
			if (InitialStock < 0)
			{
				throw new ArgumentException("Stock cannot be negative");
			}
			Name =name.Trim();
			Price = Money.From(price); // valdiation of price is done already inside Money VO

			if (InitialStock==null)
			{
				StockQuantity= 0;
			}
			else
			{
				StockQuantity = InitialStock.Value;
			}

			IsAvailable = true;
			IsDeleted = false;
		}

		public void IncreaseStock(int quantity)
		{
			if (quantity<=0)
			{
				throw new ArgumentException("Quantity must be greater than zero.");
			}
			if (IsDeleted==true)
			{
				throw new InvalidOperationException("Cannot modify a deleted product.");
			}
			StockQuantity += quantity;
		}

		public void DecreaseStock(int quantity)
		{
			if (quantity <= 0)
			{
				throw new ArgumentException("Quantity must be greater than zero.");
			}
			if (IsDeleted==true)
			{
				throw new InvalidOperationException("Cannot modify a deleted product.");
			}
			if (quantity > StockQuantity)
			{
				throw new InvalidOperationException("Insufficient stock.");
			}
			StockQuantity -= quantity;
		}

		public void MakeAvailable() 
		{
			if (IsDeleted == true)
			{
				throw new InvalidOperationException("Cannot modify a deleted product.");
			}
			IsAvailable = true;
		}

		public void MakeUnavailable()
		{
			if (IsDeleted == true)
			{
				throw new InvalidOperationException("Cannot modify a deleted product.");
			}
			IsAvailable =false;
		}

		public void SoftDelete()
		{
			IsDeleted=true;
			IsAvailable=false;
		}

		public void Restore()
		{
			IsDeleted = false;
		}
	}
}
