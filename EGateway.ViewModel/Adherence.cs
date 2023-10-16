﻿using Newtonsoft.Json;

namespace EGateway.ViewModel;

public class Adherence
{
	[JsonProperty("status")]
	public string Status { get; set; }

	[JsonProperty("start_date")]
	public DateTime StartDate { get; set; }

	[JsonProperty("end_date")]
	public DateTime? EndDate { get; set; }
}