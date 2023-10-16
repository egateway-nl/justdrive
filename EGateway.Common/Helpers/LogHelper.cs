using EGateway.Common.ViewModels;
using NLog;

namespace EGateway.Common.Helpers
{
	public static class LogHelper
	{
		public static LogEventInfo LogFullData(this MongoLogViewModel mongoLog, LogLevel? logLevel, string message = "")
		{
			var generatedEvent = new LogEventInfo(logLevel ?? LogLevel.Info, "Info", message);

			generatedEvent.Properties.Add("CustomData", mongoLog);

			return generatedEvent;
		}
	}
}
