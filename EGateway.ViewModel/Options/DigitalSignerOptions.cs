using EGateway.ViewModel.Extenstions;

namespace EGateway.ViewModel.Options;

public class DigitalSignerOptions
{
	public string PrivateKey { get; set; }
	public string RawPublicKey { get; set; }
	public string PublicKey => RawPublicKey.ConvertToBase64Der();
	public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
	{
		ConfigurationException.AssertNotNull(RawPublicKey, $"{nameof(DigitalSignerOptions)}.{nameof(RawPublicKey)}");
		ConfigurationException.AssertNotNull(PrivateKey, $"{nameof(DigitalSignerOptions)}.{nameof(PrivateKey)}");
	}
}