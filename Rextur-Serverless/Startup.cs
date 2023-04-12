using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Rextur_Serverless.Handler;
using Rextur_Serverless.Interfaces;
using Rextur_Serverless.Services;
using System;

[assembly: FunctionsStartup(typeof(Rextur_Serverless.Startup))]
namespace Rextur_Serverless;

public class Startup : FunctionsStartup
{
	public override void Configure(IFunctionsHostBuilder builder)
	{
		builder.Services.AddLogging();

		builder.Services.AddHttpClient();

		builder.Services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

		builder.Services.AddHttpClient<IAuthService, AuthService>("authReservaFacil", c =>
		{
			c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("URL_RESERVAFACIL"));
		});

		builder.Services.AddHttpClient<IReservaFacilService, ReservaFacilService>("ticketsReservafacil", c =>
		{
			c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("URL_RESERVAFACIL"));
		}).AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>(); ;
		
		builder.Services.AddScoped<IAuthService, AuthService>();
		builder.Services.AddScoped<IReservaFacilService, ReservaFacilService>();
	}

}
