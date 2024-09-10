using System.Text.Json.Serialization;

public class CellStateApi{
    public CellStateApi(double value, int id)
    {
        Value = value;
        Id = id;
    }
    [JsonPropertyName("value")]
    public double Value { get; set; }
    [JsonPropertyName("id")]
    public int Id { get; set; }
}