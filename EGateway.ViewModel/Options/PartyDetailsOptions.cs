namespace EGateway.ViewModel.Options;

public class PartyDetailsOptions
{
	public string ClientId { get; set; }
	public string Name { get; set; }
	public string BaseUri { get; set; }
	public string AccessTokenAuthorizationHeaderFirstRole { get; set; }

	public string AccessTokenAuthorizationHeaderSecondRole { get; set; }

	public string AccessTokenAuthorizationHeaderThirdRole { get; set; }

	public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
	{
		ConfigurationException.AssertNotNull(ClientId, $"{nameof(PartyDetailsOptions)}.{nameof(ClientId)}");
		ConfigurationException.AssertNotNull(Name, $"{nameof(PartyDetailsOptions)}.{nameof(Name)}");
		ConfigurationException.AssertUri(BaseUri, $"{nameof(PartyDetailsOptions)}.{nameof(BaseUri)}");
		ConfigurationException.AssertUri(AccessTokenAuthorizationHeaderFirstRole, $"{nameof(PartyDetailsOptions)}.{nameof(AccessTokenAuthorizationHeaderFirstRole)}");
		ConfigurationException.AssertUri(AccessTokenAuthorizationHeaderSecondRole, $"{nameof(PartyDetailsOptions)}.{nameof(AccessTokenAuthorizationHeaderSecondRole)}");
		ConfigurationException.AssertUri(AccessTokenAuthorizationHeaderThirdRole, $"{nameof(PartyDetailsOptions)}.{nameof(AccessTokenAuthorizationHeaderThirdRole)}");

	}
}