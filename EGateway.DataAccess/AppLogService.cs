using EGateway.Model;
using MongoDB.Driver;

namespace EGateway.DataAccess;

public class AppLogService
{
	private readonly IMongoCollection<dynamic> _appLog;

	public AppLogService(string connectionString, string database, string collection)
	{
		var mongoClient = new MongoClient(connectionString);

		var mongoDatabase = mongoClient.GetDatabase(database);

		_appLog = mongoDatabase.GetCollection<dynamic>(collection);
	}
	public async Task<List<dynamic>> GetAsync()
	{
		var data = await _appLog.Find(_ => true).ToListAsync();

		return data.Take(5).ToList();
	}

	public async Task<List<dynamic>> GetLastAsync(int rows)
	{
		try
		{
			var data = await _appLog.Find(_ => true).ToListAsync();
			data = data.TakeLast(rows).ToList();
			//data = data.OrderByDescending(x => x).Take(rows).ToList();

			return data;
		}
		catch
		{
			return null;

		}
	}
}