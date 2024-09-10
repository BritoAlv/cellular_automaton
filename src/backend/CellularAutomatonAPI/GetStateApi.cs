using System.Text.Json;
using System.Text.Json.Serialization;
public class GetStateApi
{
	[JsonPropertyName("cells")]
	public List<List<CellStateApi>> Cells {get; set;}
	[JsonPropertyName("percents")]
	public List<double> Percents {get; set;}
	public GetStateApi(List<List<CellStateApi>> cells, List<double> percents)
	{
		Cells = cells;
		Percents = percents;
	}
}
