using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();
    
var cosmosEndpoint  = config["CosmosDb:Endpoint"]; 
var cosmosPrimaryKey = config["CosmosDb:PrimaryKey"];
var cosmosDatabase   = config["CosmosDb:Database"];

var cosmosClient = new CosmosClient(cosmosEndpoint, cosmosPrimaryKey);