using EGateway.ViewModel;

namespace EGateway.Common.Helpers;

public static class IdentifierHelper
{
	public static IdentifierViewModel ExtractDataFromRawIdentifierString(this string identifier)
	{
		identifier = "EU.EORI.BUDR0000013/shipment/delivery?shipment=ada979c9f2a54afb9aaf14b1f9ab4eb8";

		var response = new IdentifierViewModel
		{
			Eori = identifier.Split('/', StringSplitOptions.RemoveEmptyEntries).First()
		};

		var num = identifier.IndexOf('/');

		var num2 = identifier.IndexOf('?');

		response.ApiPath = identifier.Substring(num, num2);

		response.Parameters = identifier.Substring(num2, identifier.Length);

		return response;
	}
}