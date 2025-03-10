using Microsoft.Extensions.DependencyInjection;
using Ortzschestrate.Web3.Actions;

namespace Ortzschestrate.Web3;

public class ServiceRegisterer
{
    public static void RegisterServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<GetStakedBalance>();
        serviceCollection.AddSingleton<StartGame>();
    }
}