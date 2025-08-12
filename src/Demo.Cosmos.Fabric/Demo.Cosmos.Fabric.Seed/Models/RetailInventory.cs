using System.Text.Json.Serialization;

namespace Demo.Cosmos.Fabric.Seed.Models;

public sealed class RetailInventory
{
    [JsonPropertyName("id")] public string Id { get; set; } = default!;
    [JsonPropertyName("storeId")] public string StoreId { get; set; } = default!;
    [JsonPropertyName("docType")] public string DocType { get; set; } = "inventory";
    [JsonPropertyName("productId")] public string ProductId { get; set; } = default!;
    [JsonPropertyName("productCode")] public string ProductCode { get; set; } = default!;
    [JsonPropertyName("currentStock")] public int CurrentStock { get; set; }
    [JsonPropertyName("minimumStock")] public int MinimumStock { get; set; }
    [JsonPropertyName("maximumStock")] public int MaximumStock { get; set; }
    [JsonPropertyName("lastRestockDate")] public DateTime? LastRestockDate { get; set; }
    [JsonPropertyName("lastStockCheckDate")] public DateTime? LastStockCheckDate { get; set; }
    [JsonPropertyName("updatedDate")] public DateTime UpdatedDate { get; set; }
}