using Nethereum.Web3.Accounts;
using Ortzschestrate.Infrastructure;

namespace Ortzschestrate.Web3.Utilities;

internal static class Web3Factory
{
    public static Nethereum.Web3.Web3 GetDefault()
    {
        var privateKey = Environment.GetEnvironmentVariable(EnvKeys.PrivateKey) ??
                         throw new ApplicationException($"{EnvKeys.PrivateKey} not provided.");
        var account = new Account(privateKey);
        var web3 = new Nethereum.Web3.Web3(account,
            Environment.GetEnvironmentVariable(EnvKeys.ChainUrl) ?? "http://localhost:8545/");
        return web3;
    }
}