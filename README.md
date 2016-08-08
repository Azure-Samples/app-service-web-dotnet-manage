# app-service-web-dotnet-manage
This sample demonstrates how to manage your WebApps using the .NET SDK

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
