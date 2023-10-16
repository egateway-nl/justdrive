namespace EGateway.Common.ViewModels;

public class MongoLogViewModel
{
	public string? ActionName { get; set; }

	public string? ControllerName { get; set; }

	public dynamic? Request { get; set; }

	public dynamic? Response { get; set; }

	public string? ClientIp { get; set; }

}

public class ShippmentRequest
{
	public string? Shippment { get; set; }

	public string? PartyEori { get; set; }
}
