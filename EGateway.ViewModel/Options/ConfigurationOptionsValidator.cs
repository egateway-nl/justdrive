namespace EGateway.ViewModel.Options;

public class ConfigurationOptionsValidator
{
	public string Environment { get; set; }

	public readonly ConfigurationOptionsValidator NullValidator = new();
}