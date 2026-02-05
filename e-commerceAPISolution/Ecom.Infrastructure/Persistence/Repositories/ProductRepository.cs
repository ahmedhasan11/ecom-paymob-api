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

		private IQueryable<Product> ApplyFilters(
		IQueryable<Product> query,
		string? search,
		decimal? minPrice,
		decimal? maxPrice)
		{
			if (!string.IsNullOrWhiteSpace(search))
			{
				query = query.Where(p => p.Name.Contains(search));
			}

			if (minPrice.HasValue)
			{
				query = query.Where(p => p.Price.Amount >= minPrice.Value);
			}

			if (maxPrice.HasValue)
			{
				query = query.Where(p => p.Price.Amount <= maxPrice.Value);
			}

			return query;
		}

		public async Task AddProductAsync(Product product)
		{
			 await _dbContext.Products
				.AddAsync(product);
			 //await _dbContext.SaveChangesAsync();
		}
		public async Task<IReadOnlyList<Product>> GetProductsAsync(string? search, decimal? minPrice, decimal? maxPrice,
																	 int pageNumber, int pageSize)
		{
			var query = _dbContext.Products.AsQueryable();
			query = ApplyFilters(query, search, minPrice, maxPrice);
			return await query.OrderByDescending(p => p.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
		}
		public async Task<int> GetTotalProductsCountAsync(string? search, decimal? minPrice, decimal? maxPrice )
		{
			var query = _dbContext.Products.AsQueryable();
			query= ApplyFilters(query, search, minPrice,maxPrice);
			return await query.CountAsync();
		}
		public async Task<Product?> GetProductByIdAsync(Guid id)
		{
			return await _dbContext.Products
				.FirstOrDefaultAsync(p => p.Id == id);
		}


		public  Task UpdateProductAsync(Product product)
		{
			return Task.CompletedTask;
		}

		public Task DeleteProductAsync(Product product)
		{
			_dbContext.Products
				.Remove(product);
			return Task.CompletedTask;
		}


	}
}
