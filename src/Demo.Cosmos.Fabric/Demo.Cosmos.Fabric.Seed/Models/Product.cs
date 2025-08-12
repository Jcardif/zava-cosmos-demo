using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demo.Cosmos.Fabric.Seed.Models;

public sealed class Product
{
    [JsonPropertyName("id")] public string Id { get; set; } = default!;
    [JsonPropertyName("code")] public string Code { get; set; } = default!;
    [JsonPropertyName("name")] public string Name { get; set; } = default!;
    [JsonPropertyName("category")] public CategorySnapshot Category { get; set; } = new();
    [JsonPropertyName("type")] public string Type { get; set; } = default!;
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("msrp")] public decimal Msrp { get; set; }
    [JsonPropertyName("costPrice")] public decimal CostPrice { get; set; }
    [JsonPropertyName("launchDate")] public DateTime LaunchDate { get; set; }
    [JsonPropertyName("size")] public string Size { get; set; } = default!;
    [JsonPropertyName("color")] public string Color { get; set; } = default!;
    // Specifications may be arbitrary JSON; JsonElement keeps it flexible
    [JsonPropertyName("specifications")] public JsonElement Specifications { get; set; }
    [JsonPropertyName("isCustomizable")] public bool IsCustomizable { get; set; }
    // (optional) lineage to original SQL category id if present in source data
    [JsonPropertyName("sourceCategoryId")] public string? SourceCategoryId { get; set; }
}