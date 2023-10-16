using EGateway.ViewModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EGateway.Model
{
	public class DelegationEvidenceDB
	{
		public DelegationEvidenceDB() =>
			CreationDate = DateTime.Now;

		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		public ShippmentResponse? Shipments { get; set; }

		public Root? DelegationEvidence { get; set; }

		public bool Active { get; set; }

		public DateTime CreationDate { get; set; }
	}
}
