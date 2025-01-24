using Org.BouncyCastle.Math;
using Ortzschestrate.Infrastructure;
using Ortzschestrate.Web3.Contracts.ORTBet.ContractDefinition;
using Ortzschestrate.Web3.Utilities;

namespace Ortzschestrate.Web3.Actions;

public class GetStakedBalance
{
    public async Task<BigInteger> Do(string playerAddress)
    {
        var web3 = Web3Factory.GetDefault();

        var userBalanceFunction = new UserBalancesFunction()
        {
            ReturnValue1 = playerAddress
        };

        var handler = web3.Eth.GetContractQueryHandler<UserBalancesFunction>();
        var balance = await handler.QueryAsync<BigInteger>(
            Environment.GetEnvironmentVariable(EnvKeys.ContractAddress),
            userBalanceFunction);

        return balance;
    }
}