using EGateway.Api.WorkerServices;
using EGateway.Common.Helpers;
using EGateway.DataAccess;
using EGateway.ViewModel.Options;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
//var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = Environments.Production });

builder.Host.ConfigureLogging(x => x.ClearProviders().SetMinimumLevel(LogLevel.Trace));

builder.Host.UseNLog();

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetLogger("Info");

try
{
	builder.Services.AddHealthChecks();

	builder.Services.AddMemoryCache();

	builder.Services.Configure<PartyDetailsOptions>(options => builder.Configuration.GetSection("EGateway").Bind(options));
	builder.Services.Configure<ApiOption>(options => builder.Configuration.GetSection("ApiDestination").Bind(options));
	builder.Services.Configure<DigitalSignerOptions>(options => builder.Configuration.GetSection("DigitalSigner").Bind(options));
	builder.Services.Configure<SchemeOwnerClientOptions>(options => builder.Configuration.GetSection("SchemeOwner").Bind(options));
	builder.Services.Configure<AuthorizationRegistryClientOptions>(options => builder.Configuration.GetSection("AuthorizationRegistry").Bind(options));

	builder.Services.AddTransient<SchemeOwnerClient>();
	builder.Services.AddTransient<DigitalSigner>();
	builder.Services.AddTransient<AssertionService>();
	builder.Services.AddTransient<TokenGenerator>();
	builder.Services.AddTransient<TokenClient>();
	builder.Services.AddTransient<AuthorizationRegistryClient>();

	builder.Services.AddSingleton(_ => NLogBuilder.ConfigureNLog("nlog.config").GetLogger("Info"));

	builder.Services.AddSingleton<DelegationEvidenceManager>();
	builder.Services.AddSingleton<AppLogManager>();

	builder.Services.AddSingleton(_ => new DelegationEvidenceService(
		builder.Configuration["EGatewayDatabase:ConnectionString"],
		builder.Configuration["EGatewayDatabase:DatabaseName"],
		builder.Configuration["EGatewayDatabase:CollectionName"]));

	builder.Services.AddSingleton(_ => new AppLogService(
		builder.Configuration["EGatewayDatabase:ConnectionString"],
		builder.Configuration["EGatewayDatabase:DatabaseName"],
		builder.Configuration["EGatewayDatabase:LogCollectionName"]));

	// Add services to the container.

	builder.Services.AddControllers();

	// ASPNETCORE_ENVIRONMENT = Staging
	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

	if (!builder.Environment.IsProduction())
		builder.Services
				.AddEndpointsApiExplorer()
				.AddSwaggerGen();

	builder.Services.AddHostedService<SyncDataWorker>();

	var app = builder.Build();

	if (!builder.Environment.IsProduction())
		app.UseSwagger()
		.UseSwaggerUI();

	//builder.Services.AddEndpointsApiExplorer();
	//builder.Services.AddSwaggerGen();

	//builder.Services.AddHostedService<SyncDataWorker>();

	//var app = builder.Build();

	//// Configure the HTTP request pipeline.
	//app.UseSwagger()
	//	.UseSwaggerUI()
	//	.UseHttpsRedirection()
	//	.UseStaticFiles();

	// Configure the HTTP request pipeline.
	app.UseHttpsRedirection()
		.UseStaticFiles();

	//app.UseAuthorization();

	app.MapControllers();

	app.MapHealthChecks("HealthCheck");

	await app.RunAsync();
}
catch (Exception exception)
{
	logger.Error(exception, "Program Stopped Because of Exception !");
	throw;
}
finally
{
	NLog.LogManager.Shutdown();
}


public partial class Program { }