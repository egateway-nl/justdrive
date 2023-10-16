namespace EGateway.ViewModel;

public class Resource
{
	public string type { get; set; }
	public List<string> identifiers { get; set; }
	public List<object> attributes { get; set; }
	public string access_method { get; set; }
}