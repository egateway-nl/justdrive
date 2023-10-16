using EGateway.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EGateway.DataAccess;

public class DelegationEvidenceService
{
	private readonly IMongoCollection<DelegationEvidenceDB> _delegationEvidenceCollection;

	private readonly IMongoDatabase _database;

	public DelegationEvidenceService(string connectionString, string database, string collection)
	{
		var mongoClient = new MongoClient(connectionString);

		_database = mongoClient.GetDatabase(database);

		_delegationEvidenceCollection = _database.GetCollection<DelegationEvidenceDB>(collection);
	}

	public async Task<List<DelegationEvidenceDB>> GetAsync() =>
	    await _delegationEvidenceCollection.Find(_ => true).ToListAsync();

	//public async Task<DelegationEvidenceDB> GetLastAsync() =>
	//    await _delegationEvidenceCollection.Find(_ => true).SortByDescending(x => x.CreationDate).Limit(1).FirstOrDefaultAsync();

	public async Task<DelegationEvidenceDB?> GetLastAsync()
	{
		try
		{
			var sort = Builders<DelegationEvidenceDB>.Sort.Descending("_id");

			var data = await _delegationEvidenceCollection.Find(_ => true).Sort(sort).Limit(1).FirstOrDefaultAsync();

			return data;
		}
		catch
		{
			return null;
		}
	}

	public async Task<DelegationEvidenceDB?> GetAsync(string id) =>
	    await _delegationEvidenceCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

	public async Task CreateAsync(DelegationEvidenceDB newEntity) =>
	    await _delegationEvidenceCollection.InsertOneAsync(newEntity);

	public async Task UpdateAsync(string id, DelegationEvidenceDB updatedEntity) =>
	    await _delegationEvidenceCollection.ReplaceOneAsync(x => x.Id == id, updatedEntity);

	public async Task RemoveAsync(string id) =>
	    await _delegationEvidenceCollection.DeleteOneAsync(x => x.Id == id);
}