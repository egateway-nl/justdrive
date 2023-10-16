using Microsoft.Extensions.Hosting;

namespace EGateway.ViewModel.Options;

public class AuthorizationRegistryClientOptions
{
	public string BaseUri { get; set; }
	public string Thumbprint { get; set; }
	public string ClientId { get; set; }
	public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
	{
		var required = validateConfigurationOptions?.Environment == Environments.Production;
		ConfigurationException.AssertThumbprint(Thumbprint, $"{nameof(AuthorizationRegistryClientOptions)}.{nameof(Thumbprint)}", required: required);
		ConfigurationException.AssertUri(BaseUri, $"{nameof(AuthorizationRegistryClientOptions)}.{nameof(BaseUri)}");
		ConfigurationException.AssertNotNull(ClientId, $"{nameof(AuthorizationRegistryClientOptions)}.{nameof(ClientId)}");
	}
}