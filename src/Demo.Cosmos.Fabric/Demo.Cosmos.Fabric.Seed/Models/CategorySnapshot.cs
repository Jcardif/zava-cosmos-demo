using System.Text.Json.Serialization;

namespace Demo.Cosmos.Fabric.Seed.Models;

public sealed class CategorySnapshot
{
    [JsonPropertyName("id")] public string Id { get; set; } = default!;
    [JsonPropertyName("name")] public string Name { get; set; } = default!;
    [JsonPropertyName("type")] public string Type { get; set; } = default!;
}