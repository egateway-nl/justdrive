using System.IdentityModel.Tokens.Jwt;
using EGateway.ViewModel;
using EGateway.ViewModel.Options;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace EGateway.Common.Helpers;

public class AuthorizationRegistryClient
{
	private readonly ILogger _logger;
	private readonly TokenClient _tokenClient;
	private readonly AuthorizationRegistryClientOptions _authorizationRegistryClientOptions;
	private readonly PartyDetailsOptions _partyDetailsOptions;


	public AuthorizationRegistryClient(TokenClient tokenClient,
		IOptions<PartyDetailsOptions> partyDetailsOptions,
		IOptions<AuthorizationRegistryClientOptions> authorizationRegistryClientOptions,
		ILogger<AuthorizationRegistryClient> logger)
	{
		_logger = logger;
		_tokenClient = tokenClient;
		_partyDetailsOptions = partyDetailsOptions.Value ?? throw new ArgumentNullException(nameof(partyDetailsOptions));
		_authorizationRegistryClientOptions = authorizationRegistryClientOptions.Value ?? throw new ArgumentNullException(nameof(authorizationRegistryClientOptions));
	}

	public async Task<DelegationEvidence> GetDelegation(DelegationMask mask, string client_assertion)
	{
		_logger.LogInformation("Get delegation for {policyIssuer} and {target}", mask?.DelegationRequest?.PolicyIssuer, mask?.DelegationRequest?.Target?.AccessSubject);
		var jObjectMask = TransformToJObject(mask);

		var accessToken = await GetAccessToken(new ClientAssertion(_partyDetailsOptions.ClientId,
				_authorizationRegistryClientOptions.ClientId))
			;

		JObject response;
		try
		{
			response = await _authorizationRegistryClientOptions.BaseUri
					.AppendPathSegment("delegation")
					.WithHeader("previous_steps", JsonConvert.SerializeObject(new[] { client_assertion }))
					.WithOAuthBearerToken(accessToken)
					.PostJsonAsync(jObjectMask)
					.ReceiveJson<JObject>()
				;

			return GetDelegationEvidenceFromResponse(response);
		}
		catch (Exception ex)
		{
			_logger.LogInformation("Get delegation returns {delegationResponse}", ex.Message);
			return null;
		}
	}

	private static DelegationEvidence GetDelegationEvidenceFromResponse(JObject response)
	{
		var delegationToken = response.GetValue("delegation_token", StringComparison.OrdinalIgnoreCase).ToString();

		var handler = new JwtSecurityTokenHandler();
		var token = handler.ReadJwtToken(delegationToken);
		var delegation = token.Claims.FirstOrDefault(c => c.Type == "delegationEvidence");

		return JsonConvert.DeserializeObject<DelegationEvidence>(delegation.Value);
	}

	private static bool IsPermitRule(DelegationEvidence delegation)
		=> delegation.PolicySets
			.SelectMany(policySet => policySet.Policies)
			.All(policy => policy.Rules.Any(rule => rule == "Permit"));

	private static JObject TransformToJObject(DelegationMask mask)
	{
		var serializedMask = JsonConvert.SerializeObject(mask, new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
		});

		return JsonConvert.DeserializeObject<JObject>(serializedMask);
	}

	private async Task<string> GetAccessToken(ClientAssertion clientAssertion)
	{
		var assertion = new ClientAssertion
		{
			Subject = _partyDetailsOptions.ClientId,
			Issuer = _partyDetailsOptions.ClientId,
			Audience = _authorizationRegistryClientOptions.ClientId,
			JwtId = clientAssertion.JwtId,
			IssuedAt = clientAssertion.IssuedAt,
			Expiration = clientAssertion.Expiration
		};

		var accessToken = await _tokenClient
			.GetAccessToken(_authorizationRegistryClientOptions.BaseUri,
				_partyDetailsOptions.ClientId,
				assertion);

		return accessToken;
	}
}