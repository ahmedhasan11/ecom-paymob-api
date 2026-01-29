using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Persistence.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly AppDbContext _dbContext;
		public ProductRepository( AppDbContext context) 
		{
			_dbContext = context;
		}
		public async Task AddProductAsync(Product product)
		{
			 await _dbContext.Products.AddAsync(product);
			 //await _dbContext.SaveChangesAsync();
		}

		public async Task<IReadOnlyList<Product>> GetAllProductsAsync()
		{
			return await _dbContext.Products.ToListAsync();
		}

		public async Task<Product?> GetProductByIdAsync(Guid id)
		{
			return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
		}
	}
}
