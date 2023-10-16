using EGateway.Common.Helpers;
using EGateway.DataAccess;
using EGateway.SyncData;
using EGateway.ViewModel.Options;
using NLog.Web;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetLogger("Info");

try
{
	IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
	    services.AddSingleton(_ => NLogBuilder.ConfigureNLog("nlog.config").GetLogger("Info"));

	    services.AddSingleton(_ => new ApiOption
	    {
		    AccessKey = context.Configuration["ApiDestination:AccessKey"],
		    BaseAddress = context.Configuration["ApiDestination:BaseAddress"],
		    UserName = context.Configuration["ApiDestination:UserName"],
		    PassWord = context.Configuration["ApiDestination:PassWord"],
		    //RojoAccessKey = builder.Configuration["ApiDestination:RojoAccessKey"],
		    //RojoBaseAddress = builder.Configuration["ApiDestination:RojoBaseAddress"]
	    });

	    services.AddSingleton<DelegationEvidenceManager>();

	    services.AddSingleton(_ => new DelegationEvidenceService(
		context.Configuration["EGatewayDatabase:ConnectionString"],
		context.Configuration["EGatewayDatabase:DatabaseName"],
		context.Configuration["EGatewayDatabase:CollectionName"]));

	    services.AddHostedService<Worker>();
    })
    .Build();

	await host.RunAsync();
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