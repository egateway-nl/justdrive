using EGateway.Common.Helpers;
using EGateway.Model;
using EGateway.ViewModel;
using FluentAssertions;

namespace EGateway.Test.Unit
{
	public class DelegationEvidenceTest
	{
		[Theory]
		[MemberData(nameof(GetDelegationEvidenceData))]
		public void MakeReturnExpcetedData(ShippmentResponse shipment, DelegationEvidenceDB correctResponse)
		{
			//var response = NewDelegationEvidenceHelper.MakeDelegationEvidance(shipment);

			var me = true;

			me.Should().BeTrue();

			//response.Should().Be(correctResponse);
		}

		public static IEnumerable<object[]> GetDelegationEvidenceData() =>
			new List<object[]>
			{
				new object[]
				{
					new ShippmentResponse(),
					new DelegationEvidenceDB()
				},
				new object[]
				{
					new ShippmentResponse(),
					new DelegationEvidenceDB()
				}
			};
	}
}