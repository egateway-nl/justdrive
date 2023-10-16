using EGateway.ViewModel;
using Flurl;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace EGateway.Common.Helpers;

public class TokenClient
{
	private readonly ILogger<TokenClient> _logger;
	private readonly AssertionService _assertionService;

	public TokenClient(AssertionService assertionService, ILogger<TokenClient> logger)
	{
		_logger = logger;
		_assertionService = assertionService;
	}

	public async Task<string> GetAccessToken(string source, string clientId, ClientAssertion assertion)
	{
		_logger.LogInformation("Get access_token for {clientId} from {source}", clientId, source);

		var jwtAssertion = await _assertionService.CreateJwtAssertion(assertion);

		return await DoGetAccessToken(source, clientId, jwtAssertion);
	}

	public async Task<string> GetAccessToken(string source, string clientId, string assertion)
	{
		_logger.LogInformation("Get access_token for {clientId} and {assertion} from {source}", clientId, "***REDACTED***", source);

		return await DoGetAccessToken(source, clientId, assertion);
	}

	private async Task<string> DoGetAccessToken(string source, string clientId, string assertion)
	{
		//using var requestBody = new FormUrlEncodedContent(new Dictionary<string, string>
		//    {
		//	{"grant_type", "client_credentials"},
		//	{"scope", "iSHARE"},
		//	{"client_id", clientId},
		//	{"client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"},
		//	{"client_assertion", assertion}
		//    });

		var client = new RestClient();
		var request = new RestRequest(source.AppendPathSegment("connect/token").ToUri(), Method.Post) { Timeout = -1 };
		request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
		//request.AddBody(requestBody);

		request.AddParameter("grant_type", "client_credentials");
		request.AddParameter("scope", "iSHARE");
		request.AddParameter("client_id", clientId);
		request.AddParameter("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
		request.AddParameter("client_assertion", assertion);

		var response = await client.ExecuteAsync(request);


		//var tokenResponse = await source.AppendPathSegment("connect/token")
		//	.PostAsync(requestBody)
		//	.ReceiveJson()
		//	.ConfigureAwait(false);

		//var accessToken = (string)tokenResponse.access_token;

		//_logger.LogDebug("Retrieved {access_token}", "***REDACTED***");

		return response.ToString()!;

		//return accessToken;
	}
}