﻿using System.Globalization;

namespace EGateway.ViewModel.Extenstions;

public static class EpochTimeExtensions
{
	public static string ToEpoch(this DateTime value)
	{
		var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		var elapsedTime = value - epoch;
		return ((long)elapsedTime.TotalSeconds).ToString(CultureInfo.CurrentCulture);
	}
}