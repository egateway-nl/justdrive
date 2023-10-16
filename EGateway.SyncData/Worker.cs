using EGateway.Common.Helpers;
using EGateway.DataAccess;
using EGateway.ViewModel;
using EGateway.ViewModel.Options;
using Newtonsoft.Json;
using NLog;
using RestSharp;

namespace EGateway.SyncData
{
	public class Worker : BackgroundService
	{
		private readonly Logger _logger;

		private readonly ApiOption _apiOption;

		private readonly DelegationEvidenceService _delegationEvidenceService;

		private readonly DelegationEvidenceManager _delegationEvidenceManager;

		public Worker(Logger logger, ApiOption apiOption, DelegationEvidenceManager delegationEvidenceMaker, DelegationEvidenceService delegationEvidenceService)
		{
			_logger = logger;
			_apiOption = apiOption;
			_delegationEvidenceService = delegationEvidenceService;
			_delegationEvidenceManager = delegationEvidenceMaker;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var client = new RestClient();
				var request = new RestRequest(_apiOption.BaseAddress + "shipments", Method.Get) { Timeout = -1 };
				request.AddHeader("Authorization", "Bearer " + _apiOption.AccessKey);
				var body = @"";
				request.AddParameter("text/plain", body, ParameterType.RequestBody);
				var response = await client.ExecuteAsync<ShippmentResponse>(request, stoppingToken);

				var existData = await _delegationEvidenceService.GetAsync();

				if (response.Data != null)
				{
					if (!existData.Any())
					{
						await _delegationEvidenceManager.MakeDelegationEvidanceAsync(response.Data!, stoppingToken);
						_logger.Info($"Worker running at: {DateTimeOffset.Now} with result : Our Data Was Null and Created Again .");
					}
					else
					{
						if (response.Data.data.Count != existData.Last().Shipments!.data.Count)
						{
							await _delegationEvidenceManager.MakeDelegationEvidanceAsync(response.Data!, stoppingToken);
							_logger.Info($"Worker running at: {DateTimeOffset.Now} with result : Data Count Was Not Equal and Created Again .");
						}
						else
						{
							/// ToDo : Check Solution for Compare Collections
							bool isEqual = Enumerable.SequenceEqual(response.Data.data.OrderBy(e => e.id), existData.Last().Shipments!.data.OrderBy(e => e.id));
							if (!isEqual)
							{
								await _delegationEvidenceManager.MakeDelegationEvidanceAsync(response.Data!, stoppingToken);
								_logger.Info($"Worker running at: {DateTimeOffset.Now} with result : Data Count Was Not Equal and Created Again .");
							}
							//else
							//{
							//	//var medexisJson = JavaScriptSerializer().Serialize(response.Data.data.OrderBy(e => e.id));
							//}
						}
					}
				}
				else
					_logger.Info($"Worker running at: {DateTimeOffset.Now} with result : No Shipment Added .");

				await Task.Delay(TimeSpan.FromMinutes(20), stoppingToken);
			}
		}
	}
}