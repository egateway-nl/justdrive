using EGateway.Model;
using EGateway.ViewModel;
using Newtonsoft.Json;

namespace EGateway.Common.Helpers
{
	public static class NewDelegationEvidenceHelper
	{
		public static DelegationEvidenceDB MakeDelegationEvidance(ShippmentResponse shippments)
		{
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

			return new Model.DelegationEvidenceDB
			{
				Active = true,
				CreationDate = DateTime.Now,
				//Shipments = JsonConvert.SerializeObject(shippments.data!),
				Shipments = shippments,
				DelegationEvidence = rootObject
			};
		}
	}
}
