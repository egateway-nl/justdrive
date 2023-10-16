using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;

namespace EGateway.Test.Integration;

public class EGatewayTest : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client;

	public EGatewayTest(WebApplicationFactory<Program> factory) =>
		_client = factory.CreateDefaultClient();

	#region [Theories Method(s)]

	[Theory]
	[MemberData(nameof(GetShipmentsData))]
	public async Task GetShipmentsMustBeReturnExpectedData(string accessKey, HttpStatusCode correctResponse, short validTime)
	{
		var timer = new Stopwatch();

		timer.Start();

		using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/egateway/ishare/EGateway/shipments/shipment");
		requestMessage.Headers.Add("accessTokenHeader", accessKey);

		var result = await _client.SendAsync(requestMessage);

		timer.Stop();

		timer.ElapsedMilliseconds.Should().BeLessThanOrEqualTo(validTime);

		result.StatusCode.Should().Be(correctResponse);
	}


	[Theory]
	[MemberData(nameof(GetShipmentsByDriverData))]
	public async Task GetShipmentsByDriverMustBeReturnExpectedData(string accessKey, string partyEORI, HttpStatusCode correctResponse, short validTime)
	{

		var timer = new Stopwatch();

		timer.Start();

		using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/egateway/ishare/EGateway/shipmentsByDriver?partyEORI={partyEORI}");
		requestMessage.Headers.Add("accessTokenHeader", accessKey);

		var result = await _client.SendAsync(requestMessage);

		timer.Stop();

		timer.ElapsedMilliseconds.Should().BeLessThanOrEqualTo(validTime);

		result.StatusCode.Should().Be(correctResponse);
	}


	[Theory]
	[MemberData(nameof(ShipmentsByFacilityData))]
	public async Task ShipmentsByFacilityMustBeReturnExpectedData(string accessKey, string facilityPartyEORI, HttpStatusCode correctResponse, short validTime)
	{

		var timer = new Stopwatch();

		timer.Start();

		using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/egateway/ishare/EGateway/shipmentsByFacility?facilityPartyEORI=" + facilityPartyEORI);
		requestMessage.Headers.Add("accessTokenHeader", accessKey);

		var result = await _client.SendAsync(requestMessage);

		timer.Stop();

		timer.ElapsedMilliseconds.Should().BeLessThanOrEqualTo(validTime);

		result.StatusCode.Should().Be(correctResponse);
	}


	[Theory]
	[MemberData(nameof(PickupData))]
	public async Task PickupMustBeReturnExpectedData(string accessKey, string shipment, string partyEORI, HttpStatusCode correctResponse, short validTime)
	{

		var timer = new Stopwatch();

		timer.Start();

		using var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/egateway/ishare/EGateway/shipment/pickup?shipment={shipment}&partyEORI={partyEORI}");
		requestMessage.Headers.Add("accessTokenHeader", accessKey);

		var result = await _client.SendAsync(requestMessage);

		timer.Stop();

		timer.ElapsedMilliseconds.Should().BeLessThanOrEqualTo(validTime);

		result.StatusCode.Should().Be(correctResponse);
	}

	[Theory]
	[MemberData(nameof(DeliveryData))]
	public async Task DeliveryMustBeReturnExpectedData(string accessKey, string shipment, string partyEORI, HttpStatusCode correctResponse, short validTime)
	{

		var timer = new Stopwatch();

		timer.Start();

		using var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/egateway/ishare/EGateway/shipment/Delivery?shipment={shipment}&partyEORI={partyEORI}");
		requestMessage.Headers.Add("accessTokenHeader", accessKey);

		var result = await _client.SendAsync(requestMessage);

		timer.Stop();

		timer.ElapsedMilliseconds.Should().BeLessThanOrEqualTo(validTime);

		result.StatusCode.Should().Be(correctResponse);
	}


	#endregion

	#region [Member Data]

	public static IEnumerable<object[]> GetShipmentsData() =>
			new List<object[]>
			{
				new object[]
				{
					//check without cache
					"",
					HttpStatusCode.OK,
					5000
				},
				new object[]
				{
					//check with cashe
					"",
					HttpStatusCode.OK,
					5000
				},
				new object[]
				{
					//check with cashe and Token
					"dGVzdEVnYXRld2F5Rmlyc3RSb2xlMjAyMg==",
					HttpStatusCode.OK,
					5000
				}
			};

	public static IEnumerable<object[]> GetShipmentsByDriverData() =>
			new List<object[]>
			{
				new object[]
				{
					"",
					"EU.EORI.BUDR000000",
					HttpStatusCode.Forbidden,
					5000
				},
				new object[]
				{
					"dGVzdEVnYXRld2F5Rmlyc3RSb2xlMjAyMg==",
					"EU.EORI.BUDR0000002",
					HttpStatusCode.OK,
					5000
				}
			};

	public static IEnumerable<object[]> ShipmentsByFacilityData() =>
			new List<object[]>
			{
				new object[]
				{
					"",
					"EU.EORI.BUNU000000666",
					HttpStatusCode.Forbidden,
					5000
				},
				new object[]
				{
					"dGVzdEVnYXRld2F5Rmlyc3RSb2xlMjAyMg==",
					"EU.EORI.BUNU0000006",
					HttpStatusCode.OK,
					5000
				}
			};

	public static IEnumerable<object[]> PickupData() =>
			new List<object[]>
			{
				new object[]
				{
					"",
					"",
					"EU.EORI.BUDR000000",
					HttpStatusCode.BadRequest,
					5000
				},
				new object[]
				{
					"",
					"asdasd",
					"EU.EORI.BUDR000000",
					HttpStatusCode.Forbidden,
					5000
				},
				new object[]
				{
					"dGVzdEVnYXRld2F5Rmlyc3RSb2xlMjAyMg==",
					"fbc30e8d2f824492a932827f2c8fa007",
					"EU.EORI.BUDR0000008",
					HttpStatusCode.OK,
					5000
				}
			};

	public static IEnumerable<object[]> DeliveryData() =>
			new List<object[]>
			{
				new object[]
				{
					"",
					"",
					"EU.EORI.BUDR000000",
					HttpStatusCode.BadRequest,
					5000
				},
				new object[]
				{
					"",
					"asdasd",
					"EU.EORI.BUDR000000",
					HttpStatusCode.Forbidden,
					5000
				},
				new object[]
				{
					"dGVzdEVnYXRld2F5Rmlyc3RSb2xlMjAyMg==",
					"fbc30e8d2f824492a932827f2c8fa007",
					"EU.EORI.BUDR0000008",
					HttpStatusCode.OK,
					5000
				}
			};

	#endregion
}