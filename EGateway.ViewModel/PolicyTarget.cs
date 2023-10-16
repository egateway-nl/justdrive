namespace EGateway.ViewModel;

public class PolicyTarget
{
	public PolicyTargetResource Resource { get; set; }

	public PolicyTargetEnvironment Environment { get; set; }

	public List<string> Actions { get; set; }
}