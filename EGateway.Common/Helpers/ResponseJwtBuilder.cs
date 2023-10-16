using System.Collections;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EGateway.ViewModel.Extenstions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace EGateway.Common.Helpers;

public class ResponseJwtBuilder
{
	private readonly DigitalSigner _digitalSigner;
	private readonly TokenGenerator _tokenGenerator;

	public ResponseJwtBuilder(DigitalSigner digitalSigner, TokenGenerator tokenGenerator)
	{
		_digitalSigner = digitalSigner;
		_tokenGenerator = tokenGenerator;
	}

	public async Task<string> Create(object payloadObject, string subject, string issuer, string audience, string payloadObjectClaim, IContractResolver contractResolver = null)
	{
		var payload = BuildJwtPayload(payloadObject, issuer, subject, audience, payloadObjectClaim, contractResolver);
		var header = await BuildJwtHeader();

		var token = await _tokenGenerator.GenerateToken(header, payload);
		return token;
	}

	private async Task<JwtHeader> BuildJwtHeader()
	{
		var header = new JwtHeader();

		var publicKey = await _digitalSigner.GetPublicKey();
		header.Add("x5c", new[] { publicKey });
		header.Add("alg", SecurityAlgorithms.RsaSha256);
		header.Add("typ", "JWT");

		return header;
	}

	private JwtPayload BuildJwtPayload(object payloadObject, string issuer, string subject, string audience, string payloadObjectClaim, IContractResolver jsonResolver = null)
	{
		var claims = new List<Claim>
		{
			new Claim("iss", issuer),
			new Claim("sub", subject),
			new Claim("jti", Guid.NewGuid().ToString("N", CultureInfo.CurrentCulture)),
			new Claim("iat", DateTime.UtcNow.ToEpoch(), ClaimValueTypes.Integer),
			new Claim("exp", DateTime.UtcNow.AddSeconds(30).ToEpoch(), ClaimValueTypes.Integer)
		};

		if (audience != null)
		{
			claims.Add(new Claim("aud", audience));
		}

		if (payloadObject is string stringPayload)
		{
			var internalPayload = JsonConvert.DeserializeObject<JObject>(stringPayload).ToDictionary();

			return new JwtPayload(claims) {
				{payloadObjectClaim, CreateCustomPayload(internalPayload, jsonResolver)}
			};
		}

		if (payloadObject is IEnumerable enumerablePayload)
		{
			var internalPayloads = new List<IDictionary<string, object>>();
			foreach (var item in enumerablePayload)
			{
				internalPayloads.Add(CreateCustomPayload(item, jsonResolver));
			}

			return new JwtPayload(claims) {
				{payloadObjectClaim, internalPayloads }
			};
		}

		return new JwtPayload(claims) {
			{payloadObjectClaim, CreateCustomPayload(payloadObject, jsonResolver)}
		};
	}

	private static IDictionary<string, object> CreateCustomPayload(object payloadObject, IContractResolver jsonResolver)
	{
		var jsonSettings = new JsonSerializerSettings
		{
			Formatting = Formatting.None,
			NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
		};
		if (jsonResolver != null)
		{
			jsonSettings.ContractResolver = jsonResolver;
		}

		var internalJson = JsonConvert.SerializeObject(payloadObject, jsonSettings);

		var internalPayload = JsonConvert.DeserializeObject<JObject>(internalJson).ToDictionary();
		return internalPayload;
	}
}