using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EGateway.ViewModel;
using EGateway.ViewModel.Options;
using Microsoft.Extensions.Options;
using NLog;

namespace EGateway.Common.Helpers;

public class SchemeOwnerClient
{
	private readonly Logger _logger;
	private readonly TokenClient _tokenClient;
	private readonly SchemeOwnerClientOptions _schemeOwnerClientOptions;
	private readonly PartyDetailsOptions _partyDetailsOptions;

	public SchemeOwnerClient(TokenClient tokenClient,
		IOptions<PartyDetailsOptions> partyDetailsOptions,
		IOptions<SchemeOwnerClientOptions> schemeOwnerClientOptions,
		Logger logger)
	{
		_logger = logger;
		_tokenClient = tokenClient;
		_partyDetailsOptions = partyDetailsOptions.Value ?? throw new ArgumentNullException(nameof(partyDetailsOptions));
		_schemeOwnerClientOptions = schemeOwnerClientOptions.Value ?? throw new ArgumentNullException(nameof(schemeOwnerClientOptions));
	}

	public async Task<Party> GetParty(string partyId)
	{
		_logger.Info("Party id used: {partyId}", partyId);

		var accessToken = await GetAccessToken(new ClientAssertion(
			_partyDetailsOptions.ClientId,
			_schemeOwnerClientOptions.ClientId));


		var request = await _schemeOwnerClientOptions.BaseUri
				.AppendPathSegment("parties")
				.AppendPathSegment(partyId)
				.WithOAuthBearerToken(accessToken)
				.GetJsonAsync()
			;

		if (request == null)
		{
			_logger.Info("No party with client id {partyId} was found at SO.", partyId);
			return null;
		}

		var partyClaim = new JwtSecurityTokenHandler().ReadJwtToken((string)request.party_token).Claims.FirstOrDefault(c => c.Type == "party_info") as Claim;
		var party = JsonConvert.DeserializeObject<Party>(partyClaim.Value);

		_logger.Info("Party status : {status}", party!.Adherence.Status);

		return party;
	}

	private async Task<string> GetAccessToken(ClientAssertion clientAssertion)
	{
		var assertion = new ClientAssertion
		{
			Subject = _partyDetailsOptions.ClientId,
			Issuer = _partyDetailsOptions.ClientId,
			Audience = _schemeOwnerClientOptions.ClientId,
			JwtId = clientAssertion.JwtId,
			IssuedAt = clientAssertion.IssuedAt,
			Expiration = clientAssertion.Expiration
		};

		var accessToken = await _tokenClient
			.GetAccessToken(_schemeOwnerClientOptions.BaseUri,
				_partyDetailsOptions.ClientId,
				assertion);
		return accessToken;
	}
	public async Task<string> GetDirectAccessToken(string partyId)
	{

		string accessToken = await GetAccessToken(new ClientAssertion(
			_partyDetailsOptions.ClientId,
			_schemeOwnerClientOptions.ClientId));
		return accessToken;
	}

}