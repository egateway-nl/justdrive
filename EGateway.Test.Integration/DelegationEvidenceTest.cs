using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;

namespace EGateway.Test.Integration;

public class DelegationEvidenceTest : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client;

	public DelegationEvidenceTest(WebApplicationFactory<Program> factory) =>
		_client = factory.CreateDefaultClient();

	#region [Theories Method(s)]

	//[Theory]
	//[MemberData(nameof(GetDelegationEvidenceData))]
	//public async Task GetShipmentsMustBeReturnExpectedData(string partyEori, string shipment, bool correctResponse, short validTime)
	//{
	//	var timer = new Stopwatch();

	//	timer.Start();

	//	using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/egateway/ishare/EGateway/?partyEori={partyEori}&shipment={shipment}");

	//	var result = await _client.SendAsync(requestMessage);

	//	timer.Stop();

	//	timer.ElapsedMilliseconds.Should().BeLessThanOrEqualTo(validTime);

	//	string resultString = result.Content.ReadAsStringAsync().Result;

	//	resultString.Should().Be(correctResponse.ToString().ToLower());
	//}

	//#endregion

	//#region [Member Data]

	//public static IEnumerable<object[]> GetDelegationEvidenceData() =>
	//		new List<object[]>
	//		{
	//			new object[]
	//			{
	//				"EU.EORI.BUDR00008888",
	//				"fbc30e8d2f824492a932827f2c8f3307",
	//				false,
	//				5000
	//			},
	//			new object[]
	//			{
	//				"EU.EORI.BUDR0000008",
	//				"fbc30e8d2f824492a932827f2c8fa007",
	//				true,
	//				5000
	//			}
	//		};

	#endregion
}