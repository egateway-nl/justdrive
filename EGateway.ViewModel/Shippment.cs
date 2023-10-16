namespace EGateway.ViewModel;

public class Shippment
{
	public string id { get; set; }
	public string facility_origin_id { get; set; }
	public string facility_destination_id { get; set; }
	public string supply_path_id { get; set; }
	public string code { get; set; }
	public int status { get; set; }
	public DateTime shipment_date { get; set; }
	public string courier_id { get; set; }
	public object courier_vehicle_id { get; set; }
	public DateTime? planned_date { get; set; }
	public DateTime? effective_date { get; set; }
	public string comments { get; set; }
	public string associated_barcode { get; set; }
	public DateTime created_at { get; set; }
	public string created_by { get; set; }
	public DateTime updated_at { get; set; }
	public string updated_by { get; set; }
	public object deleted_at { get; set; }
	public object deleted_by { get; set; }
	public int is_deleted { get; set; }
	public List<object> shipment_details { get; set; }
}