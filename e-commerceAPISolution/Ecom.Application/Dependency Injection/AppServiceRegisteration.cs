using Ecom.Application.Interfaces;
using Ecom.Application.Services;
using Ecom.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Dependency_Injection
{
	public static class AppServiceRegisteration
	{
		public static IServiceCollection AddApplication(this IServiceCollection services  )
		{
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<ICartService, CartService>();
			services.AddScoped<IOrderService, OrderService>();
			services.AddScoped<ICheckoutService, CheckoutService>();
			services.AddScoped<IPaymentService, PaymentService>();
			services.AddValidatorsFromAssembly(typeof(AddProductDtoValidator).Assembly);
			

			return services;
		}
	}
}
