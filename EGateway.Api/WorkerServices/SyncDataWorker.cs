using EGateway.Common.Helpers;
using EGateway.DataAccess;
using EGateway.ViewModel;
using EGateway.ViewModel.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NLog;
using RestSharp;

namespace EGateway.Api.WorkerServices
{
	public class SyncDataWorker : BackgroundService
	{
		private readonly Logger _logger;

		private readonly ApiOption _apiOption;

		private readonly DelegationEvidenceService _delegationEvidenceService;

		private readonly DelegationEvidenceManager _delegationEvidenceManager;

		public SyncDataWorker(
			Logger logger,
			IOptions<ApiOption> apiOption,
			DelegationEvidenceManager delegationEvidenceMaker,
			DelegationEvidenceService delegationEvidenceService)
		{
			_logger = logger;
			_apiOption = apiOption.Value ?? throw new ArgumentNullException(nameof(apiOption));
			_delegationEvidenceService = delegationEvidenceService;
			_delegationEvidenceManager = delegationEvidenceMaker;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				while (true)
				{
					var client = new RestClient();
					var request = new RestRequest(_apiOption.BaseAddress + "shipments", Method.Get) { Timeout = -1 };
					request.AddHeader("Authorization", "Bearer " + _apiOption.AccessKey);
					var body = @"";
					request.AddParameter("text/plain", body, ParameterType.RequestBody);
					var response = await client.ExecuteAsync<ShippmentResponse>(request, stoppingToken);

					// ToDo : Remove All Shippment Cache (Include AllShippments and ByDriver and ByFacility)

					// ToDo (Optional) : Set All Removed Caches

					var existData = await _delegationEvidenceService.GetLastAsync();
					//var lastExistData = existData.Last().Shipments!.data;

					if (response.Data != null)
					{
						if (existData == null)
						{
							// ToDo : Romove Old and Set DelegationEvidence Cache

							await _delegationEvidenceManager.MakeDelegationEvidanceAsync(response.Data!);

							_logger.Info($"Worker running at: {DateTimeOffset.Now} with result : Our Data Was Null and Created Again .");
						}
						else
						{
							if (response.Data.data.Count != existData.Shipments!.data.Count)
							{
								// ToDo : Romove Old and Set DelegationEvidence Cache

								var result = await _delegationEvidenceManager.MakeDelegationEvidanceAsync(response.Data!);

								_logger.Info(result
									? $"Worker running at: {DateTimeOffset.Now} with result : Data Count Was Not Equal and Created Again ."
									: "Error");
							}
							//else
							//{

							//	bool isEqual = Enumerable.SequenceEqual(response.Data.data.OrderBy(e => e.id), existData.Last().Shipments!.data.OrderBy(e => e.id));
							//	//bool isEqual = Enumerable.SequenceEqual(response.Data.data, lastExistData);
							//	/// ToDo : Check Solution for Compare Collections
							//	if (!isEqual)
							//	{
							//		await _delegationEvidenceManager.MakeDelegationEvidanceAsync(response.Data!, stoppingToken);
							//		_logger.Info($"Worker running at: {DateTimeOffset.Now} with result : Data Count Was Not Equal and Created Again .");
							//	}
							//	//else
							//	//{
							//	//	//var medexisJson = JavaScriptSerializer().Serialize(response.Data.data.OrderBy(e => e.id));
							//	//}
							//}
						}
					}
					else
						_logger.Info($"Worker running at: {DateTimeOffset.Now} with result : No Shipment Added .");

					await Task.Delay(TimeSpan.FromDays(1));
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error in Woker Agent for UAT Testing");
			}
		}
	}
}
