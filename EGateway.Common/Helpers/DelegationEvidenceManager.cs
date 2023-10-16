using EGateway.DataAccess;
using EGateway.Model;
using EGateway.ViewModel;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NLog;

namespace EGateway.Common.Helpers
{
	public class DelegationEvidenceManager
	{
		private readonly DelegationEvidenceService _delegationEvidenceService;
		private readonly Logger _logger;
		private readonly IMemoryCache _cache;

		public DelegationEvidenceManager(Logger logger, DelegationEvidenceService delegationEvidenceService, IMemoryCache cache)
		{
			_delegationEvidenceService = delegationEvidenceService;
			_logger = logger;
			_cache = cache;
		}

		public async Task<bool> MakeDelegationEvidanceAsync(ShippmentResponse shippments, bool saveToDb = true)
		{
			try
			{
				//ShippmentResponse data
				string policyIssuer = "EU.EORI.BUWH0000001";
				string serviceProviders = "EU.EORI.BUGW0000001";

				var driversAccessSubject = shippments.data.Select(x => x.courier_id).Distinct().ToList();
				var FacilitiesAccessSubject = shippments.data.Select(x => x.facility_destination_id).Distinct().ToList();
				var FacilitiesOriginAccessSubject = shippments.data.Select(x => x.facility_origin_id).Distinct().ToList();
				var resultsAccessSubject = driversAccessSubject
					.Concat(FacilitiesAccessSubject)
					.Concat(FacilitiesOriginAccessSubject)
					.ToList();

				resultsAccessSubject = resultsAccessSubject.Where(x => x != null).ToList();
				resultsAccessSubject.Sort();
				string seperatorSymbol = "_-_";
				var identifierslist = new List<string> { };
				foreach (var item in shippments.data)
					identifierslist.Add(item.id + seperatorSymbol + item.courier_id + seperatorSymbol + item.facility_destination_id + seperatorSymbol + item.facility_origin_id);
				string json = @"
						{
							'delegationEvidence': {
								'notBefore': 1629980328,
								'notOnOrAfter': 1672527600,
								'policyIssuer': 'ReplacePolicyIssuer',
								'target': {
									'accessSubject': [
										'ReplaceAccessSubject'
									]
								},
								'policySets': [
									{
										'maxDelegationDepth': 0,
										'target': {
											'environment': {
												'licenses': [
													'USAID BURUNDI PILOT'
												]
											}
										},
										'policies': [
											{
												'target': {
													'resource': {
														'type': 'uri',
														'identifiers': [
															'ReplaceIdentifiers'
														],
														'attributes': [],
														'access_method': 'PUT'
													},
													'actions': [
														'iSHARE.READ',
														'iSHARE.UPDATE'
													],
													'environment': {
														'serviceProviders': [
															'ReplaceServiceProviders'
														]
													}
												},
												'rules': [
													'permit'
												]
											}
										]
									}
								]
							}
						}";

				var rootObject = new EGateway.ViewModel.Root();
				JsonConvert.PopulateObject(json, rootObject);

				//Change Value
				rootObject.delegationEvidence.Target.AccessSubject = resultsAccessSubject;
				rootObject.delegationEvidence.PolicyIssuer = policyIssuer;
				rootObject.delegationEvidence.PolicySets[0].Policies[0].Target.Resource.Identifiers = identifierslist;
				rootObject.delegationEvidence.PolicySets[0].Policies[0].Target.Environment.ServiceProviders[0] = serviceProviders;

				// serialize JSON directly to a file again
				//var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/delegationEvidence_test.json");
				//using StreamWriter file = File.CreateText(path);
				//var serializer = new JsonSerializer();
				//serializer.Serialize(file, rootObject);

				var model = new Model.DelegationEvidenceDB
				{
					Active = true,
					CreationDate = DateTime.Now,
					//Shipments = JsonConvert.SerializeObject(shippments.data!),
					Shipments = shippments,
					DelegationEvidence = rootObject
				};

				if (saveToDb)
					await _delegationEvidenceService.CreateAsync(model);
				// clear and update DE cashes
				UpdateCashesbyDE(shippments, model);


				return true;
			}
			catch (Exception e)
			{
				_logger.Error(e, "Error in MakeDelegationEvidanceAsync");
				return false;
			}
		}
		//// Todo public async Task<bool> SaveMongoDB(DelegationEvidenceDB de) 
		public void UpdateCashesbyDE(ShippmentResponse shippments, Model.DelegationEvidenceDB model)
		{
			// begin to clear all caches

			var allDrivers = shippments.data.GroupBy(x => x.courier_id).Select(x => x.Key).ToList();

			var allFacilities = shippments.data.GroupBy(x => x.facility_destination_id).Select(x => x.Key).ToList();

			foreach (var driver in allDrivers)
				_cache.Remove($"ShipmentsByDriver-{driver}");

			foreach (var facility in allFacilities)
				_cache.Remove($"ShipmentsByFacility-{facility}");

			_cache.Remove("AllShipments");

			_cache.Set("AllShipments", shippments, DateTimeOffset.Now.AddDays(1));

			_cache.Set("DelegationEvidence", model, DateTimeOffset.Now.AddDays(1));
			// end to clear all caches
		}

		public async Task<bool> CheckDelegationEvidencebyDriverbyShipment(string DriverEORI, string shipmentId)
		{
			DelegationEvidenceDB? lastData;

			lastData = _cache.Get<DelegationEvidenceDB>("DelegationEvidence");
			if (lastData == null) lastData = await _delegationEvidenceService.GetLastAsync();

			// UpdateCashesbyDE cache
			if (lastData == null) return false;
			else _cache.Set("DelegationEvidence", lastData);

			bool dateValidate = true;
			bool accessSubject = lastData.DelegationEvidence!.delegationEvidence.Target.AccessSubject.Any(x => x == DriverEORI);
			bool identifiers = lastData.DelegationEvidence.delegationEvidence.PolicySets[0].Policies[0].Target.Resource.Identifiers
				.Any(x => x.Split("_-_")[1] == DriverEORI);
			bool identifiersShipment = lastData.DelegationEvidence.delegationEvidence.PolicySets[0].Policies[0].Target.Resource.Identifiers
				.Any(x => x.Split("_-_")[0] == shipmentId);

			bool actions = lastData.DelegationEvidence.delegationEvidence.PolicySets[0].Policies[0].Target.Actions
				.Any(x => x.Contains("iSHARE.READ") || x.Contains("iSHARE.UPDATE"));

			//if (accessSubject && identifiers && dateValidate && actions) return true;
			return accessSubject && identifiers && identifiersShipment && actions && dateValidate;
		}


		public async Task<bool> CheckDelegationEvidencebyFacility(string facilityPartyEORI)
		{
			
			DelegationEvidenceDB? lastData;
			lastData = _cache.Get<DelegationEvidenceDB>("DelegationEvidence");
			if (lastData == null) lastData = await _delegationEvidenceService.GetLastAsync();

			// UpdateCashesbyDE cache
			if (lastData == null) return false;
			else _cache.Set("DelegationEvidence", lastData);


			bool dateValidate = true;
			bool accessSubject = lastData.DelegationEvidence!.delegationEvidence.Target.AccessSubject.Any(x => x == facilityPartyEORI);
			bool identifiers = lastData.DelegationEvidence.delegationEvidence.PolicySets[0].Policies[0].Target.Resource.Identifiers
				.Any(x => x.Split("_-_")[2] == facilityPartyEORI);

			bool actions = lastData.DelegationEvidence.delegationEvidence.PolicySets[0].Policies[0].Target.Actions
				.Any(x => x.Contains("iSHARE.READ") || x.Contains("iSHARE.UPDATE"));
			return accessSubject && identifiers && dateValidate && actions;
		}


		public async Task<bool> CheckDelegationEvidencebyDriver(string DriverEORI)
		{
			
			DelegationEvidenceDB? lastData;
			lastData = _cache.Get<DelegationEvidenceDB>("DelegationEvidence");
			if (lastData == null) lastData = await _delegationEvidenceService.GetLastAsync();

			// UpdateCashesbyDE cache
			if (lastData == null) return false;
			else _cache.Set("DelegationEvidence", lastData);

			bool dateValidate = true;
			bool accessSubject = lastData.DelegationEvidence!.delegationEvidence.Target.AccessSubject.Any(x => x == DriverEORI);
			bool identifiers = lastData.DelegationEvidence.delegationEvidence.PolicySets[0].Policies[0].Target.Resource.Identifiers
				.Any(x => x.Split("_-_")[1] == DriverEORI);


			bool actions = lastData.DelegationEvidence.delegationEvidence.PolicySets[0].Policies[0].Target.Actions
				.Any(x => x.Contains("iSHARE.READ") || x.Contains("iSHARE.UPDATE"));
			return accessSubject && identifiers && dateValidate && actions;


		}



		public async Task<DelegationEvidence?> LoadJsonFile(CancellationToken cancellationToken = new())
		{
			try
			{
				var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/delegationEvidence_test.json");
				var json = await File.ReadAllTextAsync(folderDetails, cancellationToken);
				return JsonConvert.DeserializeObject<Root>(json)!.delegationEvidence;
			}
			catch (Exception e)
			{
				_logger.Error(e, "Error in LoadJsonFile");
				return null;
			}

		}

		public async Task<dynamic> LoadDEfromARbyMongoDB()
		{
			var existData = await _delegationEvidenceService.GetAsync();
			return existData.LastOrDefault()!;
		}



	}
}
