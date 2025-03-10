using Newtonsoft.Json.Linq;

namespace Ortzschestrate.Web3.Utilities;

public static class Deployment
{
    public static async Task<string> ReadContractAddressAsync()
    {
        string dev = "";
#if DEBUG
        dev = "dev";
#endif

        var deploymentPath = $"../Ortzschestrate.Web3/deployment/{dev}/ORTBet.json";
        var content = await File.ReadAllTextAsync(deploymentPath);
        var jObject = JObject.Parse(content);
        return jObject["address"].ToString();
    }
}