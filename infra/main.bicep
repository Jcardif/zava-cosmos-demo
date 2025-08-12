@description('Cosmos DB account name (must be globally unique)')
param accountName string

@description('Cosmos SQL database name')
param databaseName string = 'core-operations'

var location  = resourceGroup().location

// ---------- Index policies ----------
var productsIndexPolicy = {
  indexingMode: 'consistent'
  automatic: true
  includedPaths: [
    {
      path: '/*'
    }
  ]
  excludedPaths: [
    {
      // keep big specs out of the index to save RU
      path: '/specifications/*'
    }
  ]
  compositeIndexes: [
    [
      { path: '/category/id', order: 'ascending' }
      { path: '/msrp', order: 'ascending' }
    ]
    [
      { path: '/category/id', order: 'ascending' }
      { path: '/launchDate', order: 'descending' }
    ]
  ]
}

var retailIndexPolicy = {
  indexingMode: 'consistent'
  automatic: true
  includedPaths: [
    {
      path: '/*'
    }
  ]
  compositeIndexes: [
    [
      { path: '/docType', order: 'ascending' }
      { path: '/productCode', order: 'ascending' }
    ]
  ]
}

// ---------- Cosmos account ----------
resource dbAccount 'Microsoft.DocumentDB/databaseAccounts@2025-05-01-preview' = {
  name: accountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    publicNetworkAccess: 'Enabled'
  }
}

// ---------- Database ----------
resource sqlDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2025-05-01-preview' = {
  name: databaseName
  parent: dbAccount
  properties: {
    resource: {
      id: databaseName
    }
  }
}

// ---------- products (pk=/code) ----------
resource productsContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-05-01-preview' = {
  name: '${accountName}/${databaseName}/products'
  properties: {
    resource: {
      id: 'products'
      partitionKey: {
        paths: [
          '/code'
        ]
        kind: 'Hash'
      }
      indexingPolicy: productsIndexPolicy
      // If you ever want hierarchical PK instead, switch to:
      // partitionKey: {
      //   paths: [
      //     '/category/id'
      //     '/code'
      //   ]
      //   kind: 'MultiHash'
      // }
    }
    options: {}
  }
  dependsOn: [
    sqlDb
  ]
}

// ---------- orders (pk=/customerId) ----------
resource ordersContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-05-01-preview' = {
  name: '${accountName}/${databaseName}/orders'
  properties: {
    resource: {
      id: 'orders'
      partitionKey: {
        paths: [
          '/customerId'
        ]
        kind: 'Hash'
      }
      // Unique keys are enforced within a partition key value (good for per-customer uniqueness)
      uniqueKeyPolicy: {
        uniqueKeys: [
          {
            paths: [
              '/orderNumber'
            ]
          }
        ]
      }
    }
    options: {}
  }
  dependsOn: [
    sqlDb
  ]
}

// ---------- retail (pk=/storeId) ----------
resource retailContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-05-01-preview' = {
  name: '${accountName}/${databaseName}/retail'
  properties: {
    resource: {
      id: 'retail'
      partitionKey: {
        paths: [
          '/storeId'
        ]
        kind: 'Hash'
      }
      indexingPolicy: retailIndexPolicy
      // One inventory row per product per store (docType='inventory')
      uniqueKeyPolicy: {
        uniqueKeys: [
          {
            paths: [
              '/docType'
              '/productId'
            ]
          }
        ]
      }
    }
    options: {}
  }
  dependsOn: [
    sqlDb
  ]
}

output endpoint string = dbAccount.properties.documentEndpoint
