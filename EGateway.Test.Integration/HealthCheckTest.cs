using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EGateway.Test.Integration;

public class HealthCheckTest : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client;

	public HealthCheckTest(WebApplicationFactory<Program> factory) =>
		_client = factory.CreateDefaultClient();

	[Fact]
	public async Task HealthCheckMustBeReturnExpectedData()
	{
		var result = await _client.GetStringAsync("/HealthCheck");

		result.Should().Be("Healthy");
	}
}