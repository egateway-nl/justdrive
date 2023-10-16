using System.IdentityModel.Tokens.Jwt;
using EGateway.ViewModel.Extenstions;
using Microsoft.IdentityModel.Tokens;

namespace EGateway.Common.Helpers;

public class TokenGenerator
{
	private readonly DigitalSigner _sign;

	public TokenGenerator(DigitalSigner sign)
	{
		_sign = sign;
	}

	public async Task<string> GenerateToken(JwtHeader header, JwtPayload payload)
	{
		var jwtSecurityToken = new JwtSecurityToken(header, payload);
		var rawDataBytes = System.Text.Encoding.UTF8.GetBytes(jwtSecurityToken.EncodedHeader + "." + jwtSecurityToken.EncodedPayload);

		var digest = rawDataBytes.ToSha256();

		var signature = await _sign.SignAsync(SecurityAlgorithms.RsaSha256, digest);
		var encodedSignature = Base64UrlEncoder.Encode(signature);

		return jwtSecurityToken.EncodedHeader + "." + jwtSecurityToken.EncodedPayload + "." + encodedSignature;
	}
}