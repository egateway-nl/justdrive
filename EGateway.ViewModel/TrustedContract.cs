namespace EGateway.ViewModel;

public class TrustedContract
{
	public string subject { get; set; }
	public string certificate_fingerprint { get; set; }
	public string validity { get; set; }
	public string status { get; set; }
}