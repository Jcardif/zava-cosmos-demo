Here’s a crisp, developer-friendly reference of your Cosmos DB (NoSQL) structure and item shapes, exactly how your C# seeder should write them.

# Overview

* **Database**: `zavacore`
* **Consistency**: Session
* **Containers**:

  * **products** - partition key `/code`
  * **orders** - partition key `/customerId`
  * **retail** - partition key `/storeId` (stores and inventory live together and are separated by `docType`)

# products container

**Purpose**: Full catalog. One document per product variant. Category info is embedded for fast filtering.

**Indexing policy**: include `/*`, exclude `/specifications/*`, composite `(category.id, msrp)` and `(category.id, launchDate)`.

### Item shape

| Field              | Type                | Required | Notes                                                                                  |
| ------------------ | ------------------- | -------: | -------------------------------------------------------------------------------------- |
| `id`               | string              |        ✓ | ProductId GUID from your source. Stays stable.                                         |
| `code`             | string              |        ✓ | ProductCode. Partition key value. Unique across the catalog.                           |
| `name`             | string              |        ✓ | Display name.                                                                          |
| `category.id`      | string              |        ✓ | Category GUID snapshot.                                                                |
| `category.name`    | string              |        ✓ | One of: ZavaCore Field, ZavaCore Systems, ZavaCore Nano, Pro-data Sensors.             |
| `category.type`    | string              |        ✓ | One of: B2C, B2B.                                                                      |
| `type`             | string              |        ✓ | One of: Athletic, Extreme, Lumalux, Systems, Nano, Sensors.                            |
| `description`      | string              |          | Optional marketing copy.                                                               |
| `msrp`             | number              |        ✓ | Price shown to buyers.                                                                 |
| `costPrice`        | number              |        ✓ | Internal cost.                                                                         |
| `launchDate`       | string (yyyy-MM-dd) |        ✓ | Use date only.                                                                         |
| `size`             | string              |        ✓ | One of: S, M, L, XL, XXL, NA.                                                          |
| `color`            | string              |        ✓ | One of: BlackOrange, DeepRedRed, DrBlueWhite, CharcoalSilver, TealOrange, OrangeBlack. |
| `specifications`   | object              |          | Arbitrary JSON. Excluded from index to save RU.                                        |
| `isCustomizable`   | bool                |        ✓ | Default false.                                                                         |
| `sourceCategoryId` | string              |          | Optional lineage back to SQL CategoryId.                                               |

### Example

```json
{
  "id": "8b7c3c7a-8a5a-4a78-9c36-4a26f2b2b2d1",
  "code": "ZX-ALPHA-001",
  "name": "ZavaCore Field Athletic S BlackOrange",
  "category": { "id": "c8d6e64a-6c9f-4e3e-9b8a-02bbfe7a20fd", "name": "ZavaCore Field", "type": "B2C" },
  "type": "Athletic",
  "description": "Light, durable field unit.",
  "msrp": 149.99,
  "costPrice": 85.25,
  "launchDate": "2025-02-18",
  "size": "S",
  "color": "BlackOrange",
  "specifications": { "sensor": "v3", "ipRating": "IP67" },
  "isCustomizable": false
}
```

# orders container

**Purpose**: One document per order with embedded line items.

**Uniqueness**: Unique key on `/orderNumber` (enforced per customer partition).

### Item shape

| Field                   | Type              | Required | Notes                                                     |
| ----------------------- | ----------------- | -------: | --------------------------------------------------------- |
| `id`                    | string            |        ✓ | New GUID per order.                                       |
| `customerId`            | string            |        ✓ | Partition key value.                                      |
| `orderNumber`           | string            |        ✓ | Human readable order number. Must be unique per customer. |
| `orderDate`             | string (ISO 8601) |        ✓ | UTC, e.g., `2025-06-15T14:12:55Z`.                        |
| `status`                | string            |        ✓ | One of: Pending, Processing, Shipped, Delivered.          |
| `payment.method`        | string            |        ✓ | One of: Card, MobilePayment, BankTransfer, Check.         |
| `payment.status`        | string            |        ✓ | One of: Pending, Authorized, Captured, Failed, Refunded.  |
| `shipping.address`      | string            |          | Text or structured address.                               |
| `shipping.shipDate`     | string (ISO 8601) |          | When handed to the carrier.                               |
| `shipping.deliveryDate` | string (ISO 8601) |          | When marked delivered.                                    |
| `shipping.facilityId`   | string            |          | Fulfillment center id.                                    |
| `billingAddress`        | string            |          | Optional text.                                            |
| `salesEmployeeId`       | string            |          | Optional link to rep.                                     |
| `amounts.subTotal`      | number            |        ✓ | Sum of line totals before tax, shipping, discount.        |
| `amounts.tax`           | number            |        ✓ | Calculated tax.                                           |
| `amounts.shipping`      | number            |        ✓ | Shipping charge.                                          |
| `amounts.discount`      | number            |        ✓ | Discount applied.                                         |
| `amounts.total`         | number            |        ✓ | Final amount due.                                         |
| `lineItems[]`           | array             |        ✓ | 1..n line items. See below.                               |

**lineItems element**

| Field         | Type   | Required | Notes                                |
| ------------- | ------ | -------: | ------------------------------------ |
| `productId`   | string |        ✓ | Links back to products.id.           |
| `productCode` | string |        ✓ | Copy of product code for fast reads. |
| `name`        | string |        ✓ | Snapshot of product name.            |
| `unitPrice`   | number |        ✓ | Price used for this order.           |
| `quantity`    | int    |        ✓ | Positive integer.                    |
| `lineTotal`   | number |        ✓ | unitPrice \* quantity.               |

### Example

```json
{
  "id": "b3c2d9ab-2c77-4f1c-9a6e-9a2a1c4db111",
  "customerId": "b4a6dc20-8e61-4a1a-9d5e-b77d2e4eb2e8",
  "orderNumber": "ORD-2025-000123",
  "orderDate": "2025-08-10T14:12:55Z",
  "status": "Processing",
  "payment": { "method": "Card", "status": "Authorized" },
  "shipping": {
    "address": "On file",
    "shipDate": "2025-08-11T10:00:00Z",
    "deliveryDate": null,
    "facilityId": "84d8b1c1-0a3c-4a67-9e3e-2f7d2c1e0001"
  },
  "billingAddress": "On file",
  "salesEmployeeId": "f6a5c419-2222-4f18-bbbb-000000000002",
  "amounts": { "subTotal": 299.98, "tax": 48.00, "shipping": 6.99, "discount": 0, "total": 354.97 },
  "lineItems": [
    {
      "productId": "8b7c3c7a-8a5a-4a78-9c36-4a26f2b2b2d1",
      "productCode": "ZX-ALPHA-001",
      "name": "ZavaCore Field Athletic S BlackOrange",
      "unitPrice": 149.99,
      "quantity": 2,
      "lineTotal": 299.98
    }
  ]
}
```

# retail container

**Purpose**: Stores and their inventory in one place. Two item shapes identified by `docType`.

**Indexing policy**: include `/*`, composite `(docType, productCode)` to paginate store inventory.

**Uniqueness**: Unique key on `("/docType","/productId")` so the same store cannot duplicate the same product row.

### Store item shape (`docType = "store"`)

| Field               | Type   | Required | Notes                                                             |
| ------------------- | ------ | -------: | ----------------------------------------------------------------- |
| `id`                | string |        ✓ | StoreId GUID.                                                     |
| `storeId`           | string |        ✓ | Same value as id. Partition key value for all docs in this store. |
| `docType`           | string |        ✓ | Always `"store"`.                                                 |
| `code`              | string |        ✓ | Short store code, e.g., CHI01.                                    |
| `name`              | string |        ✓ | Store display name.                                               |
| `type`              | string |        ✓ | Flagship, Outlet, etc.                                            |
| `managerEmployeeId` | string |          | Optional.                                                         |
| `hours.open`        | string |        ✓ | e.g., `"09:00"`.                                                  |
| `hours.close`       | string |        ✓ | e.g., `"21:00"`.                                                  |
| `facilityId`        | string |          | Link to fulfillment center.                                       |

**Example - store**

```json
{
  "id": "b0f0db64-1234-4bcd-9abc-00aabbccdd00",
  "storeId": "b0f0db64-1234-4bcd-9abc-00aabbccdd00",
  "docType": "store",
  "code": "CHI01",
  "name": "Chicago Flagship",
  "type": "Flagship",
  "managerEmployeeId": "d55d3f9d-3344-43f6-8b05-a71949bf6ead",
  "hours": { "open": "09:00", "close": "21:00" },
  "facilityId": "84d8b1c1-0a3c-4a67-9e3e-2f7d2c1e0001"
}
```

### Inventory item shape (`docType = "inventory"`)

| Field                | Type                | Required | Notes                                                     |                |
| -------------------- | ------------------- | -------: | --------------------------------------------------------- | -------------- |
| `id`                 | string              |        ✓ | Suggested: \`inv                                          | {productId}\`. |
| `storeId`            | string              |        ✓ | Partition key value of the store.                         |                |
| `docType`            | string              |        ✓ | Always `"inventory"`.                                     |                |
| `productId`          | string              |        ✓ | Matches products.id. Part of the unique key with docType. |                |
| `productCode`        | string              |        ✓ | For quick lookups and joins in the app.                   |                |
| `currentStock`       | int                 |        ✓ | Current on hand.                                          |                |
| `minimumStock`       | int                 |        ✓ | Reorder buffer.                                           |                |
| `maximumStock`       | int                 |        ✓ | Capacity or target.                                       |                |
| `lastRestockDate`    | string (yyyy-MM-dd) |          | Optional.                                                 |                |
| `lastStockCheckDate` | string (yyyy-MM-dd) |          | Optional.                                                 |                |
| `updatedDate`        | string (ISO 8601)   |        ✓ | Last mutation time in UTC.                                |                |

**Example - inventory**

```json
{
  "id": "inv|8b7c3c7a-8a5a-4a78-9c36-4a26f2b2b2d1",
  "storeId": "b0f0db64-1234-4bcd-9abc-00aabbccdd00",
  "docType": "inventory",
  "productId": "8b7c3c7a-8a5a-4a78-9c36-4a26f2b2b2d1",
  "productCode": "ZX-ALPHA-001",
  "currentStock": 42,
  "minimumStock": 5,
  "maximumStock": 120,
  "lastRestockDate": "2025-08-01",
  "lastStockCheckDate": "2025-08-07",
  "updatedDate": "2025-08-07T16:15:00Z"
}
```

# Indexing reference

**products**

```json
{
  "indexingMode": "consistent",
  "automatic": true,
  "includedPaths": [{ "path": "/*" }],
  "excludedPaths": [{ "path": "/specifications/*" }],
  "compositeIndexes": [
    [
      { "path": "/category/id", "order": "ascending" },
      { "path": "/msrp", "order": "ascending" }
    ],
    [
      { "path": "/category/id", "order": "ascending" },
      { "path": "/launchDate", "order": "descending" }
    ]
  ]
}
```

**retail**

```json
{
  "indexingMode": "consistent",
  "automatic": true,
  "includedPaths": [{ "path": "/*" }],
  "compositeIndexes": [
    [
      { "path": "/docType", "order": "ascending" },
      { "path": "/productCode", "order": "ascending" }
    ]
  ]
}
```

# Seeding guidance for C\#

* Use **UpsertItemAsync** to keep runs idempotent.
* Import order: **products** first, then **retail** (store docs first, then inventory rows), then **orders**.
* Use the correct partition key values on write:

  * products: `new PartitionKey(product.code)`
  * orders: `new PartitionKey(order.customerId)`
  * retail: `new PartitionKey(store.storeId or inventory.storeId)`
* Prefer **Bulk** support in the SDK with `AllowBulkExecution = true`. Use parallelism, but keep per-partition concurrency reasonable.
