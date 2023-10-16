using Microsoft.Extensions.Hosting;

namespace EGateway.ViewModel.Options;

public class SchemeOwnerClientOptions
{
	public string BaseUri { get; set; }
	public string Thumbprint { get; set; }
	public string ClientId { get; set; }
	public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
	{
		var required = validateConfigurationOptions?.Environment == Environments.Production;
		ConfigurationException.AssertThumbprint(Thumbprint, $"{nameof(SchemeOwnerClientOptions)}.{nameof(Thumbprint)}", required: required);
		ConfigurationException.AssertNotNull(ClientId, $"{nameof(SchemeOwnerClientOptions)}.{nameof(ClientId)}");
		ConfigurationException.AssertUri(BaseUri, $"{nameof(SchemeOwnerClientOptions)}.{nameof(BaseUri)}");
	}
}