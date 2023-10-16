using EGateway.Common.Helpers;
using EGateway.ViewModel;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using EGateway.ViewModel.Options;
using NLog;
using EGateway.Common.ViewModels;
//using EGateway.Api.Filters;
using Microsoft.Extensions.Options;
using EGateway.Model;
using EGateway.DataAccess;
//using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace EGateway.Api.Controllers;

[ApiController]
[Route("/egateway/ishare/[controller]")]
public class EGatewayController : ControllerBase
{
	#region [Field(s)]

	private readonly Logger _logger;

	private readonly SchemeOwnerClient _schemeOwnerClient;

	private readonly SchemeOwnerClientOptions _schemeOwnerClientOptions;

	private readonly AuthorizationRegistryClient _authorizationRegistryClient;

	private readonly ApiOption _apiOption;

	private readonly AuthorizationRegistryClientOptions _authorizationRegistryClientOptions;

	private readonly DelegationEvidenceManager _delegationEvidenceManager;
	//private readonly AppLog _AppLog;

	private readonly PartyDetailsOptions _partyDetailsOptions;

	private readonly AppLogManager _appLogManager;

	private readonly IMemoryCache _cache;

	private string dateInput = "May 10, 2022";


	#endregion


	#region [ConstructorPrivateFuncs]

	public EGatewayController(Logger logger,
		SchemeOwnerClient schemeOwnerClient,
		AuthorizationRegistryClient authorizationRegistryClient,
		DelegationEvidenceManager delegationEvidenceManager,
		IOptions<ApiOption> apiOption,
		IOptions<PartyDetailsOptions> partyDetailsOptions,
		IOptions<SchemeOwnerClientOptions> schemeOwnerClientOptions,
		IMemoryCache cache,
		IOptions<AuthorizationRegistryClientOptions> authorizationRegistryClientOptions,
		AppLogManager appLogManager)
	{
		_apiOption = apiOption.Value ?? throw new ArgumentNullException(nameof(apiOption));
		_partyDetailsOptions = partyDetailsOptions.Value ?? throw new ArgumentNullException(nameof(partyDetailsOptions));
		_schemeOwnerClientOptions = schemeOwnerClientOptions.Value ?? throw new ArgumentNullException(nameof(schemeOwnerClientOptions));
		_authorizationRegistryClientOptions = authorizationRegistryClientOptions.Value ?? throw new ArgumentNullException(nameof(authorizationRegistryClientOptions));
		_logger = logger;
		_schemeOwnerClient = schemeOwnerClient;
		_authorizationRegistryClient = authorizationRegistryClient;
		_delegationEvidenceManager = delegationEvidenceManager;
		_appLogManager = appLogManager;
		_cache = cache;
	}

	private bool CheckToken(string accessTokenHeader)
	{
		///TBD Role Driver, Facility
		//"AccessTokenAuthorizationHeaderFirstRole": "dGVzdEVnYXRld2F5Rmlyc3RSb2xlMjAyMg==", //"testEgatewayFirstRole2022",
		//"AccessTokenAuthorizationHeaderSecondRole": "dGVzdEVnYXRld2F5U2Vjb25kUm9sZTIwMjI=", //"testEgatewaySecondRole2022",
		//"AccessTokenAuthorizationHeaderThirdRole": "dGVzdEVnYXRld2F5VGhpcmRSb2xlMjAyMg==" //"testEgatewayThirdRole2022"
		accessTokenHeader = accessTokenHeader.Replace("Bearer ", "").Trim();
		if (accessTokenHeader == _partyDetailsOptions.AccessTokenAuthorizationHeaderFirstRole.Trim()) return true;
		if (accessTokenHeader == _partyDetailsOptions.AccessTokenAuthorizationHeaderSecondRole.Trim()) return true;
		if (accessTokenHeader == _partyDetailsOptions.AccessTokenAuthorizationHeaderThirdRole.Trim()) return true;
		else return false;
	}
	private bool CheckAdminToken(string accessTokenHeader)
	{
		//"AccessTokenAuthorizationHeaderThirdRole": "dGVzdEVnYXRld2F5VGhpcmRSb2xlMjAyMg==" //"testEgatewayThirdRole2022"
		accessTokenHeader = accessTokenHeader.Replace("Bearer", "").Trim();
		if (accessTokenHeader == _partyDetailsOptions.AccessTokenAuthorizationHeaderThirdRole.Trim()) return true;
		else return false;
	}
	#endregion


	#region ApiMedexis

	[HttpGet, Route("shipments/shipment")]
	public async Task<dynamic?> GetShipments([FromHeader] string? accessTokenHeader, CancellationToken cancellationToken)
	{
		try
		{

			//dGVzdEVnYXRld2F5Rmlyc3RSb2xlMjAyMg==
			//public async Task<dynamic?> GetShipments([FromHeader] string? authorization, CancellationToken cancellationToken)
			//var accessTokenHeader = authorization!.Replace("Bearer ", "").Trim();
			//test to remove a non cache object _cache.Remove("AllShipment");
			if (accessTokenHeader == null)
			{
				dateInput = "May 10, 2022";
			}
			else if (!CheckToken(accessTokenHeader))
			{
				Response.StatusCode = 403;
				return new JsonResult("Access Denied by Invalid token!!");
			}

			// check cache
			var cache = _cache.Get<ShippmentResponse>("AllShipments");
			if (cache != null) return cache;

			// fetch data from MedexisAPI
			var client = new RestClient();
			string apiQuery = _apiOption.BaseAddress + "shipments";
			var request = new RestRequest(apiQuery, Method.Get) { Timeout = -1 };
			request.AddHeader("Authorization", "Bearer " + _apiOption.AccessKey);
			var body = @"";
			request.AddParameter("text/plain", body, ParameterType.RequestBody);
			var response = await client.ExecuteAsync<ShippmentResponse>(request, cancellationToken);

			// update cache
			_cache.Set("AllShipments", response.Data!, DateTimeOffset.Now.AddDays(1));

			// log
			_logger.Info(new MongoLogViewModel
			{
				ActionName = nameof(GetShipments),
				ControllerName = nameof(EGatewayController),
				//Response = response.Data!,
				ClientIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
			}.LogFullData(NLog.LogLevel.Info, "GetShippments Called ."));

			return response.Data!;
		}
		catch (Exception e)
		{
			_logger.Error(e, "Error in GetShipments");
			return null;
		}
	}


	[HttpGet, Route("shipmentsByDriver")]
	public async Task<dynamic?> ShipmentsByDriver([FromHeader] string? accessTokenHeader, [FromQuery] string partyEORI, CancellationToken cancellationToken)
	{
		try
		{
			//https://docs.fluentvalidation.net/en/latest/
			// dGVzdEVnYXRld2F5Rmlyc3RSb2xlMjAyMg==
			//EU.EORI.BUDR0000002
			//"BaseAddress": "http://161.35.155.37/api/",
			// http://161.35.155.37/api/couriers/EU.EORI.BUDR0000001/shipments
			//if (accessTokenHeader == null && DateTime.Now <= DateTime.Parse(dateInput))
			//{
			//	dateInput = "May 10, 2022";

			//}
			//else 
			//accessTokenHeader = accessTokenHeader!.Replace("Bearer ", "").Trim();
			if (accessTokenHeader == null)
			{
				dateInput = "May 10, 2022";
			}
			else if (!CheckToken(accessTokenHeader))
			{
				Response.StatusCode = 403;
				return new JsonResult("Access Denied by Invalid token!!");
			}
			if (!await _delegationEvidenceManager.CheckDelegationEvidencebyDriver(partyEORI!))
			{
				Response.StatusCode = 403;
				return new JsonResult("Access Denied by AR-DelegationEvidence!! or invalid EORI for shipmentsByDriver!!");
			}

			// check cache
			var cache = _cache.Get<ShippmentResponse>($"ShippmentByDriver-{partyEORI}");
			if (cache != null) return cache;
			var allCache = _cache.Get<ShippmentResponse>("AllShipments");
			if (allCache != null) //&& allCache!= null
			{
				allCache.data.RemoveAll(x => x.courier_id != partyEORI);
				_cache.Set($"ShippmentByDriver-{partyEORI}", allCache, DateTimeOffset.Now.AddDays(1));
				return allCache;
			}

			var client = new RestClient();
			string apiQuery = _apiOption.BaseAddress + "couriers/" + partyEORI + "/shipments";
			var request = new RestRequest(apiQuery, Method.Get) { Timeout = -1 };
			request.AddHeader("Authorization", "Bearer " + _apiOption.AccessKey);
			var response = await client.ExecuteAsync<ShippmentResponse>(request, cancellationToken);

			// update cache
			_cache.Set($"ShippmentByDriver-{partyEORI}", response.Data!, DateTimeOffset.Now.AddDays(1));

			_logger.Info(new MongoLogViewModel
			{
				ActionName = nameof(ShipmentsByDriver),
				ControllerName = nameof(EGatewayController),
				//Response = response.Data!,
				ClientIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
				//Request = request
			}.LogFullData(NLog.LogLevel.Info, String.Format("Get ShipmentsByDriver Called by {0}.", partyEORI)));
			return response.Data!;
		}
		catch (Exception e)
		{
			_logger.Error(e, "Error in ShipmentsByDriverby {0}.", partyEORI);
			return null;
		}
	}


	[HttpGet, Route("shipmentsByFacility")]
	public async Task<dynamic?> ShipmentsByFacility([FromHeader] string? accessTokenHeader, [FromQuery] string facilityPartyEORI, CancellationToken cancellationToken)
	{
		try
		{
			//// http://161.35.155.37/api/shipments?facility_destination_id=EU.EORI.BUNU0000006

			//if (accessTokenHeader == null && DateTime.Now <= DateTime.Parse(dateInput))
			//{
			//	dateInput = "May 10, 2022";
			//}
			//else 
			//accessTokenHeader = accessTokenHeader!.Replace("Bearer ", "").Trim();
			if (accessTokenHeader == null)
			{
				dateInput = "May 10, 2022";
			}
			else if (!CheckToken(accessTokenHeader))
			{
				Response.StatusCode = 403;
				return new JsonResult("Access Denied by Invalid token!!");
			}

			if (!await _delegationEvidenceManager.CheckDelegationEvidencebyFacility(facilityPartyEORI))
			{
				Response.StatusCode = 403;
				return new JsonResult("Access Denied by AR-DelegationEvidence!! or invalid EORI for Facility!!");
			}

			var cache = _cache.Get<ShippmentResponse>($"ShipmentsByFacility-{facilityPartyEORI}");
			if (cache != null) return cache;
			var allCache = _cache.Get<ShippmentResponse>("AllShipments");
			if (allCache != null)
			{
				allCache.data.RemoveAll(x => x.facility_destination_id != facilityPartyEORI);
				_cache.Set($"ShipmentsByFacility-{facilityPartyEORI}", allCache, DateTimeOffset.Now.AddDays(1));
				return allCache;

			}

			var client = new RestClient();

			// hhttp://161.35.155.37/api/shipments?facility_destination_id=EU.EORI.BUNU0000006
			string apiquery = _apiOption.BaseAddress + "shipments?facility_destination_id=" + facilityPartyEORI;
			var request = new RestRequest(apiquery, Method.Get) { Timeout = -1 };
			request.AddHeader("Authorization", "Bearer " + _apiOption.AccessKey);
			//_logger.Info("Log-Api:shipmentsByFacility by facilityPartyEORI:" + facilityPartyEORI + "running at: {time}", DateTimeOffset.UtcNow);
			var response = await client.ExecuteAsync<ShippmentResponse>(request, cancellationToken);
			// update the cache
			_cache.Set($"ShipmentsByFacility-{facilityPartyEORI}", response.Data!, DateTimeOffset.Now.AddDays(1));

			_logger.Info(new MongoLogViewModel
			{
				ActionName = nameof(ShipmentsByFacility),
				ControllerName = nameof(EGatewayController),
				//Response = response.Data!,
				ClientIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
				//Request = request
			}.LogFullData(NLog.LogLevel.Info, String.Format("Get shipmentsByFacility by: {0} Called.", facilityPartyEORI)));

			return response.Data!;
		}
		catch (Exception e)
		{
			_logger.Error(e, "Error in ShipmentsByFacility {0}.", facilityPartyEORI);

			return null;

		}
	}


	[HttpPut, Route("shipment/pickup")]
	public async Task<dynamic?> Pickup([FromHeader] string? accessTokenHeader, [FromQuery] string shipment, [FromQuery] string partyEORI, CancellationToken cancellationToken)
	{
		try
		{
			// status 2 means waiting , 9 means pickedup , 1 means delivered
			// dGVzdEVnYXRld2F5Rmlyc3RSb2xlMjAyMg==
			// EU.EORI.BUDR0000008
			//fbc30e8d2f824492a932827f2c8fa007 	// LM_Ship_4bdd1b392c4f4811963369c9c279f2f8_1

			//accessTokenHeader = accessTokenHeader!.Replace("Bearer ", "").Trim();
			if (accessTokenHeader == null)
			{
				dateInput = "May 10, 2022";
			}
			else if (!CheckToken(accessTokenHeader))
			{
				Response.StatusCode = 403;
				return new JsonResult("Access Denied by Invalid token!!");
			}

			if (!await _delegationEvidenceManager.CheckDelegationEvidencebyDriverbyShipment(partyEORI, shipment))
			{
				Response.StatusCode = 403;
				return new JsonResult("Access Denied by AR-DelegationEvidence!! or invalid Shipment/EORI for Pickup!!");
			}

			// http://161.35.155.37/api/shipments/fbc30e8d2f824492a932827f2c8fa007?status=9
			var client = new RestClient();
			string query = _apiOption.BaseAddress + "shipments/" + shipment + "?status=9";
			var request = new RestRequest(query, Method.Put) { Timeout = -1 };
			request.AddHeader("Authorization", "Bearer " + _apiOption.AccessKey);

			var response = await client.ExecuteAsync(request, cancellationToken);

			// Clear chache for AllShipments, ShippmentByDriver, ShipmentsByFacility
			var cache = _cache.Get<ShippmentResponse>($"ShippmentByDriver-{partyEORI}");
			_cache.Remove("AllShipments");
			_cache.Remove($"ShippmentByDriver-{partyEORI}");
			if (cache != null)
			{
				var facilityId = cache.data?
					//.Where()
					//.Select(x => x.facility_destination_id)
					.SingleOrDefault(x => x.courier_id == partyEORI && x.id == shipment)?
					.facility_destination_id;
				if (facilityId != null) _cache.Remove($"ShipmentsByFacility-{facilityId}");
			}
			// end of Clear chache for AllShipments, ShippmentByDriver, ShipmentsByFacility

			_logger.Info(new MongoLogViewModel
			{
				ActionName = nameof(Pickup),
				ControllerName = nameof(EGatewayController),
				Response = response,
				ClientIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
				Request = request
			}.LogFullData(NLog.LogLevel.Info, string.Format("Pickedup by Shipment:{0} and driver:{1}.", shipment, partyEORI)));

			//_logger.Info("Log-Api:Pickup shipment by Driver:" + partyEORI + " by shipment:" + shipment + " running at: {time}", DateTimeOffset.UtcNow);
			Response.StatusCode = (int)response.StatusCode;
			return response;
		}
		catch (Exception e)
		{
			_logger.Error(e, "Error in pickup by by shipment:{0} partEORI:{1}", shipment, partyEORI);
			return null;
		}

	}


	[HttpPut, Route("shipment/delivery")]
	public async Task<dynamic?> Delivery([FromHeader] string? accessTokenHeader, [FromQuery] string shipment, [FromQuery] string partyEORI, CancellationToken cancellationToken)
	{
		try
		{
			// http://161.35.155.37/api/shipments/fbc30e8d2f824492a932827f2c8fa007?status=1
			// status 2 means waiting , 9 means pickedup , 1 means delivered
			// EU.EORI.BUDR0000008
			//fbc30e8d2f824492a932827f2c8fa007 	// LM_Ship_4bdd1b392c4f4811963369c9c279f2f8_1

			//accessTokenHeader = accessTokenHeader!.Replace("Bearer ", "").Trim();
			if (accessTokenHeader == null)
			{
				dateInput = "May 10, 2022";
			}
			else if (!CheckToken(accessTokenHeader))
			{
				Response.StatusCode = 403;
				return new JsonResult("Access Denied by Invalid token!!");
			}

			if (!await _delegationEvidenceManager.CheckDelegationEvidencebyDriverbyShipment(partyEORI, shipment))
			{
				Response.StatusCode = 403;
				return new JsonResult("Access Denied by AR-DelegationEvidence!! or invalid Shipment/EORI for Delivery!!");
			}

			// http://161.35.155.37/api/shipments/fbc30e8d2f824492a932827f2c8fa007?status=1
			var client = new RestClient();
			string query = _apiOption.BaseAddress + "shipments/" + shipment + "?status=1";
			var request = new RestRequest(query, Method.Put) { Timeout = -1 };
			request.AddHeader("Authorization", "Bearer " + _apiOption.AccessKey);

			var response = await client.ExecuteAsync(request, cancellationToken);
			// Clear chache for AllShipments, ShippmentByDriver, ShipmentsByFacility
			var cache = _cache.Get<ShippmentResponse>($"ShippmentByDriver-{partyEORI}");
			_cache.Remove("AllShipments");
			_cache.Remove($"ShippmentByDriver-{partyEORI}");
			if (cache != null)
			{
				//var facility = cache.data
				//	.Where(x => x.courier_id == partyEORI && x.id == shipment)
				//	.Select(x => x.facility_destination_id).FirstOrDefault();
				//if (facility != null) _cache.Remove($"ShipmentsByFacility-{facility}");
				var facilityId = cache.data?
					.SingleOrDefault(x => x.courier_id == partyEORI && x.id == shipment)?
					.facility_destination_id;
				if (facilityId != null) _cache.Remove($"ShipmentsByFacility-{facilityId}");
			}
			// end of Clear chache for AllShipments, ShippmentByDriver, ShipmentsByFacility

			_logger.Info(new MongoLogViewModel
			{
				ActionName = nameof(Delivery),
				ControllerName = nameof(EGatewayController),
				Response = response,
				ClientIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
				Request = request
			}.LogFullData(NLog.LogLevel.Info, String.Format("Delivered-Shipment:{0}by partEORI:{1}.", shipment, partyEORI)));
			Response.StatusCode = (int)response.StatusCode;
			return response;
		}
		catch (Exception e)
		{
			_logger.Error(e, "Error in Delivery by shipment:{0} partEORI:{1}", shipment, partyEORI);
			return null;
		}

	}

	//[HttpGet, Route("GetShipmentsviaEGateway")]
	//private async Task<dynamic?> GetShipmentsviaEGateway([FromHeader] string? accessTokenHeader, [FromQuery] string partyEORI, CancellationToken cancellationToken)
	//{
	//	try
	//	{
	//		// EU.EORI.BUDR0000002
	//		//if (!(await DelegationEvidenceHelper.LoadShipmentByDriverId(partyEORI, cancellationToken))
	//		//    .Any())
	//		//{
	//		//	Response.StatusCode = 403;
	//		//	return new JsonResult("Access Denied by AR-DelegationEvidence!! or invalid EORI!");
	//		//}

	//		//accessTokenHeader = accessTokenHeader!.Replace("Bearer ", "").Trim();
	//		if (accessTokenHeader == null)
	//		{
	//			dateInput = "May 10, 2022";
	//		}
	//		else if (!CheckToken(accessTokenHeader))
	//		{
	//			Response.StatusCode = 403;
	//			return new JsonResult("Access Denied by Invalid token!!");
	//		}
	//		else if (!await _delegationEvidenceManager.CheckDelegationEvidencebyDriver(partyEORI!))
	//		{
	//			Response.StatusCode = 403;
	//			return new JsonResult("Access Denied by AR-DelegationEvidence!! or invalid EORI for GetShipmentsviaEGateway!!");
	//		}

	//		var client = new RestClient();
	//		string apiQuery = _apiOption.BaseAddress + "couriers/" + partyEORI + "/shipments";
	//		var request = new RestRequest(apiQuery, Method.Get) { Timeout = -1 };
	//		request.AddHeader("Authorization", "Bearer " + _apiOption.AccessKey);
	//		var response = await client.ExecuteAsync<ShippmentResponse>(request, cancellationToken);
	//		_logger.Info(new MongoLogViewModel
	//		{
	//			ActionName = nameof(GetShipmentsviaEGateway),
	//			ControllerName = nameof(EGatewayController),
	//			//Response = response.Data!,
	//			ClientIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
	//			//Request = request
	//		}.LogFullData(NLog.LogLevel.Info, "GetShipmentsviaEGateway Called ."));
	//		return response.Data!;

	//	}
	//	catch (Exception e)
	//	{
	//		_logger.Error(e, "Error in GetShipmentsviaEGateway");
	//		return null;
	//	}
	//}

	#endregion


	#region log
	[HttpGet, Route("shipments/log")]
	public async Task<dynamic?> GetLogs([FromHeader] string? accessTokenHeader, int rows, CancellationToken cancellationToken)
	{
		try
		{
			// dGVzdEVnYXRld2F5VGhpcmRSb2xlMjAyMg==
			//accessTokenHeader = accessTokenHeader!.Replace("Bearer ", "").Trim();

			if (accessTokenHeader == null)
			{
				Response.StatusCode = 403;
				return new JsonResult("Access Denied by Invalid token!!");
			}
			else if (!CheckAdminToken(accessTokenHeader))
			{
				Response.StatusCode = 403;
				return new JsonResult("Access Denied by Invalid token!!");
			}
			if (rows > 40) rows = 40;
			else if (rows <= 0) rows = 5;
			
			var temp = await _appLogManager.GetLogs(rows);
			return temp;
		}
		catch (Exception e)
		{
			_logger.Error(e, "Error in fetching Logs");
			return null;
		}
	}


	[HttpGet]
	[Route("TestforlastDE")]
	private async Task<bool> CheckDelegationEvidenceByDriver([FromQuery] string partyEORI, string shipment, CancellationToken cancellationToken) =>
		await _delegationEvidenceManager.CheckDelegationEvidencebyDriverbyShipment(partyEORI, shipment);

	#endregion


	#region IShare


	[HttpGet, Route("CheckAR")]
	private async Task<dynamic?> GetAR([FromHeader] string authorization)
	{
		try
		{
			//"AccessTokenAuthorizationHeaderThirdRole": "dGVzdEVnYXRld2F5VGhpcmRSb2xlMjAyMg==" //"testEgatewayThirdRole2022"
			string accessTokenHeader = authorization.Replace("Bearer ", "").Trim();
			if (!CheckAdminToken(accessTokenHeader))
			{
				Response.StatusCode = 403;
				return new JsonResult("Admin Access Denied by Invalid token!!");
			}
			else
			{
				var data = await _delegationEvidenceManager.LoadDEfromARbyMongoDB();
				return data.DelegationEvidence.delegationEvidence;
			}
		}
		catch
		{
			return null;
		}
	}

	// Used to obtain information about iSHARE participant from the iSHARE Scheme owner. Should be used to verify the status of an iSHARE participant.

	[HttpGet, Route("GetParty")]
	private async Task<string> GetParty([FromQuery] string partyEORI)
	{
		try
		{
			//var token = _schemeOwnerClient.GetParty("EU.EORI.NL000000003");
			var token = _schemeOwnerClient.GetParty(partyEORI);
			return await Task.FromResult("");
		}
		catch (Exception e)
		{
			_logger.Error(e, "Error in GetParty");
			return "";
		}
	}

	// Used to obtain the iSHARE trusted list of certificate authorities.This will return PKIoverheid and eIDAS-qualified CAs valid under iSHARE.
	[HttpGet, Route("GetTrustedList")]
	private async Task<List<TrustedListResponse>> GetTrustedList()
	{
		try
		{
			var client = new RestClient();
			var request = new RestRequest(_schemeOwnerClientOptions.BaseUri + "trusted_list", Method.Get) { Timeout = -1 };
			string accessToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE2NDYzMTgyNDcsImV4cCI6MTY0NjMyMTg0NywiaXNzIjoiRVUuRU9SSS5OTDAwMDAwMDAwMCIsImF1ZCI6IkVVLkVPUkkuTkwwMDAwMDAwMDAiLCJjbGllbnRfaWQiOiJFVS5FT1JJLk5MMDAwMDAwMDAzIiwic2NvcGUiOlsiaVNIQVJFIl19.kJL0yDEsBDYQdZ3blcbhxXrRPS4rcHS6YByPFqFd_XfYj6Yu-XK9v7SYVGncdIpAKwr4k2dbVH-HwjQtWQO0UeT1MMHthtirL_40yfy0lPYrzsI0OPUaWcv_l4IWli68hPFKtF4VDJKTILmO37srQWbY6THkAVqSXGMy7XfTudEAEwpHLL9oKrj7CN-Icau959EbeWG2YW5X8i2cvPs8gJMiQ7P-JmI_onS4agbzUxIj2ZUNgU36tg40HoQbX-Bkh6rRg4TL9Ndm5ymQzuxwi1VAdyzNCkGZ6GOSDLUkWNewNAzd1gHq4jxGy645ZpACMAfrBmt-OqXxBAbJwmYW-Q";
			request.AddHeader("Authorization", "Bearer " + accessToken);

			//request.AddHeader("Authorization", "Bearer " + _apiOption.AccessKey);
			request.AddHeader("Do-Not-Sign", "true");
			//request.AddHeader("Cookie", "ARRAffinity=8da5db8c2398a657397df86e3980d1469261644951d7979a838d5b4c0a9d1185; ARRAffinitySameSite=8da5db8c2398a657397df86e3980d1469261644951d7979a838d5b4c0a9d1185");
			request.AlwaysMultipartFormData = true;
			var response = await client.ExecuteAsync<List<TrustedListResponse>>(request);
			return response.Data!;
		}
		catch (Exception e)
		{
			_logger.Error(e, "Error in GetTrustedList");
			return new List<TrustedListResponse>();
		}
	}

	#endregion


	#region AllComments


	//[HttpPost, Route("ar/Delegation")]
	//private async Task<dynamic?> Delegation(string? policyIssuer, string? target, string? serviceProviders)
	//{
	//	try
	//	{
	//		//2 - following steps:
	//		//$"{policyIssuer}"
	//		var client = new RestClient();
	//		//var request = new RestRequest("https://ar.isharetest.net/delegation", Method.Post) { Timeout = -1 };
	//		var request = new RestRequest(_authorizationRegistryClientOptions.BaseUri + "delegation", Method.Post) { Timeout = -1 };
	//		request.AddHeader("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE2NDYxNDA5OTUsImV4cCI6MTY0NjE0NDU5NSwiaXNzIjoiRVUuRU9SSS5OTDAwMDAwMDAwNCIsImF1ZCI6IkVVLkVP...");
	//		//1 - GetAccessTokenViewModel from AR;
	//		//request.AddHeader("Authorization", GetToken);
	//		request.AddHeader("Do-Not-Sign", "True");
	//		var body = @"{" + "\n" +
	//		@"	""delegationRequest"": {" + "\n" +
	//		@"		""policyIssuer"": $""{policyIssuer}""," + "\n" +
	//		@"		""target"": {" + "\n" +
	//		@"			""accessSubject"": $""{target}""" + "\n" +
	//		@"		}," + "\n" +
	//		@"		""policySets"": [" + "\n" +
	//		@"			" + "\n" +
	//		@"			{" + "\n" +
	//		@"				""policies"" : [" + "\n" +
	//		@"					{" + "\n" +
	//		@"						""target"": {" + "\n" +
	//		@"							""resource"": {" + "\n" +
	//		@"								""type"": ""GS1.CONTAINER""," + "\n" +
	//		@"								""identifiers"": [""180621.CONTAINER-Z""]," + "\n" +
	//		@"								""attributes"": [""GS1.CONTAINER.ATTRIBUTE.ETA"", ""GS1.CONTAINER.ATTRIBUTE.WEIGHT""]" + "\n" +
	//		@"							}," + "\n" +
	//		@"							""actions"": [""ISHARE.READ"", ""ISHARE.CREATE"", ""ISHARE.UPDATE"", ""ISHARE.DELETE""]," + "\n" +
	//		@"							""environment"": {" + "\n" +
	//		@"								""serviceProviders"":[""EU.EORI.NL000000003""]" + "\n" +
	//		@"							}" + "\n" +
	//		@"						}," + "\n" +
	//		@"						""rules"": [" + "\n" +
	//		@"							{" + "\n" +
	//		@"								""effect"": ""Permit""" + "\n" +
	//		@"							}" + "\n" +
	//		@"						]" + "\n" +
	//		@"					}" + "\n" +
	//		@"				]" + "\n" +
	//		@"			}" + "\n" +
	//		@"		]" + "\n" +
	//		@"	}," + "\n" +
	//		@"	""previous_steps"": [""{{w13.assertion.containerdata}}""]" + "\n" +
	//		@"}";
	//		request.AddParameter("application/json", body, ParameterType.RequestBody);
	//		var response = await client.ExecuteAsync(request);
	//		return response;
	//	}
	//	catch
	//	{
	//		return null;
	//	}
	//}


	////[HttpPut, Route("{{ar}}/connect/token")]
	//private async Task<dynamic> GetToken(me)
	//{
	//	try
	//	{
	//		1- GetAccessTokenViewModel from AR;
	//	
	//	var client = new RestClient("https://ar.isharetest.net/connect/token");
	//	client.Timeout = -1;
	//	var request = new RestRequest(Method.POST);
	//	request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
	//	request.AddHeader("Cookie", "ARRAffinity=8da5db8c2398a657397df86e3980d1469261644951d7979a838d5b4c0a9d1185; ARRAffinitySameSite=8da5db8c2398a657397df86e3980d1469261644951d7979a838d5b4c0a9d1185");
	//	request.AddParameter("grant_type", "client_credentials");
	//	request.AddParameter("scope", "iSHARE");
	//	request.AddParameter("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
	//	request.AddParameter("client_assertion", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsIng1YyI6WyJNSUlFaGpDQ0FtNmdBd0lCQWdJSUZVY0l3c202am5rd0RRWUpLb1pJaHZjTkFRRUxCUUF3U0RFWk1CY0dBMVVFQXd3UWFWTklRVkpGVkdWemRFTkJYMVJNVXpFTk1Bc0dBMVVFQ3d3RVZHVnpkREVQTUEwR0ExVUVDZ3dHYVZOSVFWSkZNUXN3Q1FZRFZRUUdFd0pPVERBZUZ3MHhPREEzTWpVeE5UQXlOVFZhRncweU1EQTNNalF4TlRBeU5UVmFNRDB4R0RBV0JnTlZCQU1NRDBGM1pYTnZiV1VnVjJsa1oyVjBjekVVTUJJR0ExVUVCUk1MVGt3d01EQXdNREF3TURJeEN6QUpCZ05WQkFZVEFrNU1NSUlCSWpBTkJna3Foa2lHOXcwQkFRRUZBQU9DQVE4QU1JSUJDZ0tDQVFFQXVNejBFbVQ2Y3NJNlNrMkt5Nmk1MEVSakgwbndWeVR5VGs1cnBMcko3d1Zhdkx5WGxvdk93djVUUlN2aHE3bXZsTks0OGt5UDZmNE1OQ3BWNk85T3RHdEJZeWhzSmVuU2E4dXRRSlAwd0EzMFpXQmJqalZpTU43dlFBSWM2Q2dHVFZzaU10U2ViYmNRcVJYVmsxeEVvUkt1K0VaaGFQWkNHSW4zT2hwcXBlUzZCMXplc0Radjh4azJUekMrZ0FQUGo1NHNqenRsK0h4TXhITmRpTys5Y3RoNDBMeVp0WEo2UVZYaHZEakpNL1VCbTJjVHpJaXRlUG5SYmdWSW9QdTdwOXNLNElXTWp3NDhuV0Iwa3JmNE1yQ1AyaUx0WjduS0ZxMjZsMkFDaFdrb2RST2MzeUdJc3RkNG5xQWdvNTJpR1IxYXJtNXMxYlJVeXU2TUlpMllMUUlEQVFBQm8zOHdmVEFNQmdOVkhSTUJBZjhFQWpBQU1COEdBMVVkSXdRWU1CYUFGQlk4NXlEcDFwVHZIK1dpOGJqOHZ1cmZMRGVCTUIwR0ExVWRKUVFXTUJRR0NDc0dBUVVGQndNQ0JnZ3JCZ0VGQlFjREJEQWRCZ05WSFE0RUZnUVVXaUNMRzRpNUtRaXBESFpxWGlZTkRYN01XK293RGdZRFZSMFBBUUgvQkFRREFnWGdNQTBHQ1NxR1NJYjNEUUVCQ3dVQUE0SUNBUUNkVWRXMEJCVUhFRHJJNTZZZU9HNXYyaGZLbnNyZ2xBTWcyRlA1cUFJM3RMeSs2OFpuZ2Nxa0Z1T1FkcFB1czF1YTNlL3FLeXNWVnBvS1BPT0RsWWlYbmNPVFhvbHlzcC9jVHlnWHJPVnlaeDlmc2ZlUWxLeWVaeklYOUZxZUJNcVBHUFp3M0dzTUZBb0c1RUZCN0xyYld2NllUZFp5N1JhSEt0STJrQTNuNVlQQ1JNaE8ybzlvMitHd2xDRnlZTzQ5NHIzYVdoNkN3cDJscS9KdkZCRDVRRXhucXF0c0dmVmM1UmkrRkh1UEt4VTQyL1VUQVNHUEFiaWNKMnhUQ2NncUVDbEp2ZGhvZkVLYjBsS3c5UU83T2N2SVdDR1B3VmlNY2ZiSFVGNFNHU2gwVkVnWk1kbDFxeUJVaWtYcXk1LzBYMDdMc1cwSklZTEg3SjV6MDRVc0lwV2RXaVh3OTkyY2F4RTdFSlprK2MrSUZHclVvOUpUYVEzdGVvNVR3MDV0MUNIUTFZUStib0RDRHZYWC9BUVRJK085OWd4b0FENGFzYitjUFhLR08xRHRnRUVIZDltTlc5QUVreFdpbjJtajNtVmJvVkV5eGJOb05NZm1CWDdhT0UwQm15Tk1ZSXNVWExFTjVzMzZOcXZyaWZCL0M1Wkp4ZEhhMVUrWGlyVCt6TWRBdDVEUkxKRjAxNml1bThSSTVsczY0aS9uT2phWFY4NTZFTlFzL05ZQzVGT3hWWXB1dFlXa3dXb3VqTHhJNDlTbXBlR2RzL25teURTbmlRRDZFZUMvU3pTdVFpUzd6bmZpV1JjVFZLMm9Rdk9PV0l5VnlvUEx5UHpnZVJ5QnZGT1dlSkhXV1JJeFpHZnhSS1kweWFRSUxLcklBVWxUWUZpaU9BWnk5dz09Il19.eyJpc3MiOiJFVS5FT1JJLk5MMDAwMDAwMDAyIiwic3ViIjoiRVUuRU9SSS5OTDAwMDAwMDAwMiIsImF1ZCI6WyJFVS5FT1JJLk5MMDAwMDAwMDA0IiwiaHR0cHM6Ly9pc2hhcmUtYXItcWEuYXp1cmV3ZWJzaXRlcy5uZXQvY29ubmVjdC90b2tlbiJdLCJqdGkiOiI5OWFiNWJjYTQxYmI0NWI3OGQyNDJhNDZmMDE1N2I3ZCIsImlhdCI6IjE1NDA4Mjc0MzUiLCJleHAiOjE1NDA4Mjc0NjUsIm5iZiI6MTU0MDgyNzQzNX0.l1uh2Aj-T0ASeVF0st2tUuqcpm-yaSgM57A-_u7zUZ82QYnX9BK4B0d7RKDMM_n-hcXP7HQwn9nRkZcLjntK1bKNvTwTeK7RVaxIXXkW4QS_kTkCO7rPiyIxqpiK5yWxeOHlkM3t9zPbBclMHhhYiB6038A_8b5PA6D5QCE049ASJsLjcd5V4J2JqGZWMyJ7ipiBTQnc_v9BFOsgN_wMqHS_Nm6Wxv1EC8MRPkiKRpNXaSWDv7CflduyhylHKVh06PfITqhMLmbk3eu7NLmlAYKSrI0n-emRp5SMwklkO7chI9kDt9Nq2UcaBjLdZUmMhShMND15aOJs-FXI0I2C1Q");
	//	request.AddParameter("client_id", "EU.EORI.NL000000003");
	//	IRestResponse response = client.Execute(request);
	//	Console.WriteLine(response.Content);
	//}

	//private bool CheckParty(string? partyEORI)
	//{
	//	//TBD
	//	//0 initiate SP (me as egateway on SO) {1.Assertion 2.Access Token 3.TrustedList 4.entitledParties}
	//	//1 check with SchemeOwner(SO) IsPartyValid (Driver(s)) {1. AccessToken from SO 2.Check Driver}
	//	//2 check  Medexis(entitled party) is valid in SO
	//	//3 get delegationpolicy {shake with AR: 1.get AccessToken from AR 2.get DelegationPolicy 3.validate Party from DelegationPolicy on Driver& Shipment}
	//	// All of them can be run on SP_init()
	//	// then each API just query it from our DB or cache
	//	// then return true
	//	return true;
	//}
	//private Task<bool> CheckISharepolicies(string sourceAddress, string clientId, string assertion)
	//{
	//	//using (var requestBody = new FormUrlEncodedContent(new Dictionary<string, string>
	//	//       {
	//	//	       {"grant_type", "client_credentials"},
	//	//	       {"scope", "iSHARE"},
	//	//	       {"client_id", clientId},
	//	//	       {"client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"},
	//	//	       {"client_assertion", assertion}
	//	//       }))
	//	//{
	//	//	//var tokenResponse = await source.AppendPathSegment("connect/token")
	//	//	//		.PostAsync(requestBody)
	//	//	//		.ReceiveJson()
	//	//	//	;

	//	//	//var accessToken = (string)tokenResponse.access_token;

	//	//	//_logger.LogDebug("Retrieved {access_token}", "***REDACTED***");

	//	//	//return accessToken;
	//	return true;
	//	//}
	//}


	#endregion
}