using Ecom.Application.Interfaces;
using Hangfire;

namespace e_commerceAPI.Hangfire
{
	public static class HangfireJobsExtensions
	{
		public static void AddHangfireJobs(this IApplicationBuilder app)
		{
			RecurringJob.AddOrUpdate<IReservationExpirationService>(
				"reservation-expiration-job",
				service => service.ExpireReservationsAsync(CancellationToken.None),
				Cron.Minutely);

			RecurringJob.AddOrUpdate<IPaymentExpirationService>(
				"payment-expiration-job",
				service => service.ExpirePaymentsAsync(CancellationToken.None),
				Cron.Minutely);
		}
	}
}
