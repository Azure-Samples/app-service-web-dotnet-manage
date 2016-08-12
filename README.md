---
services: app-service
platforms: dotnet
author: devigned
---

# Manage your web apps with the .NET SDK for Azure

This sample demonstrates how to manage your WebApps using the .NET SDK

**On this page**

- [Run this sample](#run)
- [What does Program.cs do?](#sample)
    - [Create a server farm](#create-server-farm)
    - [Create a website](#create-website)
    - [Get website details](#details)
    - [Delete a website](#delete)

<a id="run"></a>
## Run this sample

1. If you don't have it, install the [.NET Core SDK](https://www.microsoft.com/net/core).

1. Clone the repository.

    ```
    git clone https://github.com/Azure-Samples/app-service-web-dotnet-manage.git
    ```

1. Install the dependencies.

    ```
    dotnet restore
    ```

1. Create an Azure service principal either through
    [Azure CLI](https://azure.microsoft.com/documentation/articles/resource-group-authenticate-service-principal-cli/),
    [PowerShell](https://azure.microsoft.com/documentation/articles/resource-group-authenticate-service-principal/)
    or [the portal](https://azure.microsoft.com/documentation/articles/resource-group-create-service-principal-portal/).

1. Add these environment variables to your .env file using your subscription id and the tenant id, client id and client secret from the service principle that you created. 

    ```
    export AZURE_TENANT_ID={your tenant id}
    export AZURE_CLIENT_ID={your client or application id}
    export AZURE_CLIENT_SECRET={your service principal secret}
    export AZURE_SUBSCRIPTION_ID={your subscription id}
    ```

1. Run the sample.

    ```
    dotnet run
    ```

<a id="sample"></a>
## What does Program.cs do?

`Main()` gets the environment variables that you set up for this sample and calls `RunSample`.
`RunSample` starts by setting up a `WebSiteManagementClient` using those credentials.


```csharp
// Build the service credentials and Azure Resource Manager clients
var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, secret);
var resourceClient = new ResourceManagementClient(serviceCreds);
resourceClient.SubscriptionId = subscriptionId;
var webClient = new WebSiteManagementClient(serviceCreds);
webClient.SubscriptionId = subscriptionId;
```

The sample then sets up a resource group in which it will create the website.

```csharp
Random r = new Random();
int postfix = r.Next(0, 1000000);

var resourceGroupName = "sample-dotnet-app-service-group";
var westus = "westus";
var serverFarmName = "sample-server-farm";
var siteName = "sample-site-name-" + postfix;

// Create the resource group
Write("Creating resource group: {0}", westus);
resourceClient.ResourceGroups.CreateOrUpdate(resourceGroupName, new ResourceGroup { Location = westus});
```

<a id="create-server-farm"></a>
### Create a server farm

Create a server farm to host the website.

```csharp
var serverFarm = webClient.ServerFarms.CreateOrUpdateServerFarm(resourceGroupName, serverFarmName, new ServerFarmWithRichSku{
    Location = westus,
    Sku = new SkuDescription{
        Name = "S1",
        Capacity = 1,
        Tier = "Standard"
    }
});
```

<a id="create-website"></a>
### Create a website

```csharp
var site = webClient.Sites.CreateOrUpdateSite(resourceGroupName, siteName, new Site{
    Location = westus,
    ServerFarmId = serverFarm.Id
});
```

<a id="details"></a>
### Get details for the given website

```csharp
var gotSite = webClient.Sites.GetSite(resourceGroupName, siteName);
```

<a id="delete"></a>
### Delete a website

```csharp
webClient.Sites.DeleteSite(resourceGroupName, siteName);
```

## More information
Please refer to [Azure SDK for .NET](https://github.com/Azure/azure-sdk-for-net) for more information.
