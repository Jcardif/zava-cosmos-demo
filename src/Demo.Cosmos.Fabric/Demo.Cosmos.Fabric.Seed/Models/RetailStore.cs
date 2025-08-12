using System.Text.Json.Serialization;

namespace Demo.Cosmos.Fabric.Seed.Models;

public sealed class RetailStore
{
    [JsonPropertyName("id")] public string Id { get; set; } = default!;
    [JsonPropertyName("storeId")] public string StoreId { get; set; } = default!;
    [JsonPropertyName("docType")] public string DocType { get; set; } = "store";
    [JsonPropertyName("code")] public string Code { get; set; } = default!;
    [JsonPropertyName("name")] public string Name { get; set; } = default!;
    [JsonPropertyName("type")] public string Type { get; set; } = default!;
    [JsonPropertyName("managerEmployeeId")] public string? ManagerEmployeeId { get; set; }
    [JsonPropertyName("hours")] public StoreHours Hours { get; set; } = new();
    [JsonPropertyName("facilityId")] public string? FacilityId { get; set; }
}

public sealed class StoreHours
{
    [JsonPropertyName("open")] public string Open { get; set; } = "09:00";
    [JsonPropertyName("close")] public string Close { get; set; } = "21:00";
}