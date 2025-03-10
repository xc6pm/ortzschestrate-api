using System.Text;
using Nethereum.Util.HashProviders;
using Ortzschestrate.Web3.Contracts.ORTBet.ContractDefinition;
using Ortzschestrate.Web3.Utilities;

namespace Ortzschestrate.Web3.Actions;

public enum GameResult : byte
{
    Draw,
    Player1Won,
    Player2Won
}

public class ResolveGame
{
    public async Task DoAsync(Guid gameId, GameResult result)
    {
        var gameIdBytes = new Sha3KeccackHashProvider().ComputeHash(
            Encoding.UTF8.GetBytes(gameId.ToString()));

        var resolveGame = new ResolveGameFunction
        {
            GameId = gameIdBytes,
            Result = (byte)result
        };

        var web3 = Web3Factory.GetDefault();

        var handler = web3.Eth.GetContractTransactionHandler<ResolveGameFunction>();
        var contractAddress = await Deployment.ReadContractAddressAsync();

        await handler.SendRequestAsync(contractAddress, resolveGame);
    }
}