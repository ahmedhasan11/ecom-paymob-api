using Ecom.Domain.Common;
using Ecom.Domain.Exceptions;
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

		public byte[] RowVersion { get; set; } = default!; // For concurrency control
		private Product() { } // For EF Core
		public Product(Decimal price , string name, int? InitialStock =null)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new DomainValidationException("Name cannot be empty.");
			}
			if (InitialStock < 0)
			{
				throw new DomainValidationException("Stock cannot be negative");
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
				throw new DomainValidationException("Quantity must be greater than zero.");
			}
			if (IsDeleted==true)
			{
				throw new BusinessException("Cannot modify a deleted product.");
			}
			StockQuantity += quantity;
		}
		public void DecreaseStock(int quantity)
		{
			if (quantity <= 0)
			{
				throw new DomainValidationException("Quantity must be greater than zero.");
			}
			if (IsDeleted==true)
			{
				throw new BusinessException("Cannot modify a deleted product.");
			}
			if (quantity > StockQuantity)
			{
				throw new BusinessException("Insufficient stock.");
			}
			StockQuantity -= quantity;
		}
		public void MakeAvailable() 
		{
			if (IsDeleted == true)
			{
				throw new BusinessException("Cannot modify a deleted product.");
			}
			IsAvailable = true;
		}
		public void MakeUnavailable()
		{
			if (IsDeleted == true)
			{
				throw new BusinessException("Cannot modify a deleted product.");
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
			IsAvailable = true;
		}
	}
}
