using System.Text.Json.Serialization;

namespace Demo.Cosmos.Fabric.Seed.Models;

public sealed class Order
{
    [JsonPropertyName("id")] public string Id { get; set; } = default!;
    [JsonPropertyName("customerId")] public string CustomerId { get; set; } = default!;
    [JsonPropertyName("orderNumber")] public string OrderNumber { get; set; } = default!;
    [JsonPropertyName("orderDate")] public DateTime OrderDate { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = default!;
    [JsonPropertyName("payment")] public PaymentInfo Payment { get; set; } = new();
    [JsonPropertyName("shipping")] public ShippingInfo Shipping { get; set; } = new();
    [JsonPropertyName("billingAddress")] public string? BillingAddress { get; set; }
    [JsonPropertyName("salesEmployeeId")] public string? SalesEmployeeId { get; set; }
    [JsonPropertyName("amounts")] public Amounts Amounts { get; set; } = new();
    [JsonPropertyName("lineItems")] public List<OrderLineItem> LineItems { get; set; } = new();
}

public sealed class Amounts
{
    [JsonPropertyName("subTotal")] public decimal SubTotal { get; set; }
    [JsonPropertyName("tax")] public decimal Tax { get; set; }
    [JsonPropertyName("shipping")] public decimal Shipping { get; set; }
    [JsonPropertyName("discount")] public decimal Discount { get; set; }
    [JsonPropertyName("total")] public decimal Total { get; set; }
}

public sealed class PaymentInfo
{
    [JsonPropertyName("method")] public string Method { get; set; } = default!;
    [JsonPropertyName("status")] public string Status { get; set; } = default!;
}

public sealed class ShippingInfo
{
    [JsonPropertyName("address")] public string? Address { get; set; }
    [JsonPropertyName("shipDate")] public DateTime? ShipDate { get; set; }
    [JsonPropertyName("deliveryDate")] public DateTime? DeliveryDate { get; set; }
    [JsonPropertyName("facilityId")] public string? FacilityId { get; set; }
}



public sealed class OrderLineItem
{
    [JsonPropertyName("productId")] public string ProductId { get; set; } = default!;
    [JsonPropertyName("productCode")] public string ProductCode { get; set; } = default!;
    [JsonPropertyName("name")] public string Name { get; set; } = default!;
    [JsonPropertyName("unitPrice")] public decimal UnitPrice { get; set; }
    [JsonPropertyName("quantity")] public int Quantity { get; set; }
    [JsonPropertyName("lineTotal")] public decimal LineTotal { get; set; }
}