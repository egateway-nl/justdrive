using EGateway.DataAccess;
using EGateway.ViewModel;
using Newtonsoft.Json;
using NLog;

namespace EGateway.Common.Helpers
{
	public class AppLogManager
	{
		private readonly AppLogService _appLogService;
		//private readonly Logger _logger;

		public AppLogManager(AppLogService appLogService)
		{
			_appLogService = appLogService;
		}



		public async Task<List<dynamic>> GetLogs()
		{
			var existData = await _appLogService.GetAsync();
			//var lastData = existData.LastOrDefault();


			return existData;
		}

		public async Task<List<dynamic>> GetLogs(int rows)
		{
			var existData = await _appLogService.GetLastAsync(rows);
			//var lastData = existData.LastOrDefault();


			return existData;
		}



	}
}
