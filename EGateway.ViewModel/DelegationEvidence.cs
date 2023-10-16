﻿namespace EGateway.ViewModel;

public class DelegationEvidence
{
	public int NotBefore { get; set; }
	public int NotOnOrAfter { get; set; }
	public string PolicyIssuer { get; set; }
	public Target Target { get; set; }
	public IList<PolicySet> PolicySets { get; set; }
}