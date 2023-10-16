namespace EGateway.ViewModel;

public class DelegationRequest
{
	public string PolicyIssuer { get; set; }

	public Target Target { get; set; }

	public List<DelegationRequestPolicySet> PolicySets { get; set; }
}