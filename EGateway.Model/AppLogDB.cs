using EGateway.ViewModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EGateway.Model
{
	public class AppLogDB
	{
		public AppLogDB() =>
			Date = DateTime.Now;

		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		public string? Level { get; set; }

		public string? Message { get; set; }

		public string? Logger { get; set; }

		//public int? ThreadID { get; set; }

		//public string? ThreadName { get; set; }

		//public int? ProessID { get; set; }

		//public string? ProessName { get; set; }

		//public object? CustomData { get; set; }

		public DateTime Date { get; set; }
	}
}
