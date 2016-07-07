using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;

// Azure Management dependencies
using Microsoft.Rest.Azure.Authentication;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Azure.Management.WebSites.Models;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
            var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
            var secret = Environment.GetEnvironmentVariable("AZURE_SECRET");
            var subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");
            if(new List<string>{ tenantId, clientId, secret, subscriptionId }.Any(i => String.IsNullOrEmpty(i))) {
                Console.WriteLine("Please provide ENV vars for AZURE_TENANT_ID, AZURE_CLIENT_ID, AZURE_SECRET and AZURE_SUBSCRIPTION_ID.");
            }
            else
            {
                RunSample(tenantId, clientId, secret, subscriptionId).Wait();                
            }
        }

        public static async Task RunSample(string tenantId, string clientId, string secret, string subscriptionId)
        {
            // Build the service credentials and Azure Resource Manager clients
            var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, secret);
            var resourceClient = new ResourceManagementClient(serviceCreds);
            resourceClient.SubscriptionId = subscriptionId;
            var webClient = new WebSiteManagementClient(serviceCreds);
            webClient.SubscriptionId = subscriptionId;

            Random r = new Random();
            int postfix = r.Next(0, 1000000);

            var resourceGroupName = "sample-dotnet-app-service-group";
            var westus = "westus";
            var serverFarmName = "sample-server-farm";
            var siteName = "sample-site-name-" + postfix;

            // Create the resource group
            Write("Creating resource group: {0}", westus);
            resourceClient.ResourceGroups.CreateOrUpdate(resourceGroupName, new ResourceGroup { Location = westus});

            Write("Creating Server Farm named {0} in resource group {1}", serverFarmName, resourceGroupName);
            var serverFarm = webClient.ServerFarms.CreateOrUpdateServerFarm(resourceGroupName, serverFarmName, new ServerFarmWithRichSku{
                Location = westus,
                Sku = new SkuDescription{
                    Name = "S1",
                    Capacity = 1,
                    Tier = "Standard"
                }
            });

            Write("Creating Site named {0} in server farm {1}", siteName, serverFarmName);
            var site = webClient.Sites.CreateOrUpdateSite(resourceGroupName, siteName, new Site{
                Location = westus,
                ServerFarmId = serverFarm.Id
            });

            Write("Getting a site by name");
            var gotSite = webClient.Sites.GetSite(resourceGroupName, siteName);
            Write("Found site named {0}", gotSite.Name);

            Write("You can visit your newly created site at http://{0} or in the Azure Portal via https://portal.azure.com", gotSite.HostNames.FirstOrDefault());
            Write("Press any key to continue and delete the sample resources");
            Console.ReadLine();

            Write("Deleting the Site named {0}", siteName);
            webClient.Sites.DeleteSite(resourceGroupName, siteName);

            Write("Deleting resource group {0}", resourceGroupName);
            resourceClient.ResourceGroups.Delete(resourceGroupName);
        }

        private static void Write(string format, params object[] items) 
        {
            Console.WriteLine(String.Format(format, items));
        }
    }
}
