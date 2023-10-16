using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using EGateway.ViewModel.Extenstions;
using EGateway.ViewModel.Options;
using Microsoft.Extensions.Options;

namespace EGateway.Common.Helpers;

public class DigitalSigner
{
	private readonly DigitalSignerOptions _options;

	public DigitalSigner(IOptions<DigitalSignerOptions> options)
	{
		_options = options.Value ?? throw new ArgumentNullException(nameof(options));
	}
	public Task<string> GetPublicKey() => Task.FromResult(_options.PublicKey);

	public async Task<byte[]> SignAsync(string algorithm, byte[] digest)
	{
		var rsa = _options.PrivateKey.ConvertToRsa();

		var signature = rsa.SignHash(digest, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
		return await Task.FromResult(signature);
	}

	public async Task<bool> VerifyAsync(string algorithm, byte[] digest, byte[] signature)
	{
		using var cert = new X509Certificate2(Convert.FromBase64String(_options.PublicKey));
		var csp = (RSACng)cert.PublicKey.GetRSAPublicKey()!;
#pragma warning disable CA1416 // Validate platform compatibility
		var verified = csp.VerifyHash(digest, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
#pragma warning restore CA1416 // Validate platform compatibility
		return await Task.FromResult(verified);
	}
}