using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using Ortzschestrate.Web3.Contracts.ORTBet.ContractDefinition;

namespace Ortzschestrate.Web3.Contracts.ORTBet
{
    public partial class ORTBetService: ORTBetServiceBase
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.IWeb3 web3, ORTBetDeployment oRTBetDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<ORTBetDeployment>().SendRequestAndWaitForReceiptAsync(oRTBetDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.IWeb3 web3, ORTBetDeployment oRTBetDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<ORTBetDeployment>().SendRequestAsync(oRTBetDeployment);
        }

        public static async Task<ORTBetService> DeployContractAndGetServiceAsync(Nethereum.Web3.IWeb3 web3, ORTBetDeployment oRTBetDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, oRTBetDeployment, cancellationTokenSource);
            return new ORTBetService(web3, receipt.ContractAddress);
        }

        public ORTBetService(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

    }


    public partial class ORTBetServiceBase: ContractWeb3ServiceBase
    {

        public ORTBetServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

        public virtual Task<string> DepositStakesRequestAsync(DepositStakesFunction depositStakesFunction)
        {
             return ContractHandler.SendRequestAsync(depositStakesFunction);
        }

        public virtual Task<string> DepositStakesRequestAsync()
        {
             return ContractHandler.SendRequestAsync<DepositStakesFunction>();
        }

        public virtual Task<TransactionReceipt> DepositStakesRequestAndWaitForReceiptAsync(DepositStakesFunction depositStakesFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositStakesFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> DepositStakesRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<DepositStakesFunction>(null, cancellationToken);
        }

        public virtual Task<GamesOutputDTO> GamesQueryAsync(GamesFunction gamesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GamesFunction, GamesOutputDTO>(gamesFunction, blockParameter);
        }

        public virtual Task<GamesOutputDTO> GamesQueryAsync(byte[] returnValue1, BlockParameter blockParameter = null)
        {
            var gamesFunction = new GamesFunction();
                gamesFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GamesFunction, GamesOutputDTO>(gamesFunction, blockParameter);
        }

        public Task<BigInteger> GetBalanceQueryAsync(GetBalanceFunction getBalanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetBalanceFunction, BigInteger>(getBalanceFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetBalanceQueryAsync(string userAddress, BlockParameter blockParameter = null)
        {
            var getBalanceFunction = new GetBalanceFunction();
                getBalanceFunction.UserAddress = userAddress;
            
            return ContractHandler.QueryAsync<GetBalanceFunction, BigInteger>(getBalanceFunction, blockParameter);
        }

        public virtual Task<GetGameOutputDTO> GetGameQueryAsync(GetGameFunction getGameFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetGameFunction, GetGameOutputDTO>(getGameFunction, blockParameter);
        }

        public virtual Task<GetGameOutputDTO> GetGameQueryAsync(byte[] gameId, BlockParameter blockParameter = null)
        {
            var getGameFunction = new GetGameFunction();
                getGameFunction.GameId = gameId;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetGameFunction, GetGameOutputDTO>(getGameFunction, blockParameter);
        }

        public Task<BigInteger> GetLockedStakeQueryAsync(GetLockedStakeFunction getLockedStakeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetLockedStakeFunction, BigInteger>(getLockedStakeFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetLockedStakeQueryAsync(string userAddress, BlockParameter blockParameter = null)
        {
            var getLockedStakeFunction = new GetLockedStakeFunction();
                getLockedStakeFunction.UserAddress = userAddress;
            
            return ContractHandler.QueryAsync<GetLockedStakeFunction, BigInteger>(getLockedStakeFunction, blockParameter);
        }

        public Task<BigInteger> LockedStakesQueryAsync(LockedStakesFunction lockedStakesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<LockedStakesFunction, BigInteger>(lockedStakesFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> LockedStakesQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var lockedStakesFunction = new LockedStakesFunction();
                lockedStakesFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<LockedStakesFunction, BigInteger>(lockedStakesFunction, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public virtual Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public virtual Task<string> RenounceOwnershipRequestAsync(RenounceOwnershipFunction renounceOwnershipFunction)
        {
             return ContractHandler.SendRequestAsync(renounceOwnershipFunction);
        }

        public virtual Task<string> RenounceOwnershipRequestAsync()
        {
             return ContractHandler.SendRequestAsync<RenounceOwnershipFunction>();
        }

        public virtual Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(RenounceOwnershipFunction renounceOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(renounceOwnershipFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<RenounceOwnershipFunction>(null, cancellationToken);
        }

        public virtual Task<string> ResolveGameRequestAsync(ResolveGameFunction resolveGameFunction)
        {
             return ContractHandler.SendRequestAsync(resolveGameFunction);
        }

        public virtual Task<TransactionReceipt> ResolveGameRequestAndWaitForReceiptAsync(ResolveGameFunction resolveGameFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(resolveGameFunction, cancellationToken);
        }

        public virtual Task<string> ResolveGameRequestAsync(byte[] gameId, byte result)
        {
            var resolveGameFunction = new ResolveGameFunction();
                resolveGameFunction.GameId = gameId;
                resolveGameFunction.Result = result;
            
             return ContractHandler.SendRequestAsync(resolveGameFunction);
        }

        public virtual Task<TransactionReceipt> ResolveGameRequestAndWaitForReceiptAsync(byte[] gameId, byte result, CancellationTokenSource cancellationToken = null)
        {
            var resolveGameFunction = new ResolveGameFunction();
                resolveGameFunction.GameId = gameId;
                resolveGameFunction.Result = result;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(resolveGameFunction, cancellationToken);
        }

        public virtual Task<string> StartGameRequestAsync(StartGameFunction startGameFunction)
        {
             return ContractHandler.SendRequestAsync(startGameFunction);
        }

        public virtual Task<TransactionReceipt> StartGameRequestAndWaitForReceiptAsync(StartGameFunction startGameFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(startGameFunction, cancellationToken);
        }

        public virtual Task<string> StartGameRequestAsync(string gameId, string player1, string player2, BigInteger stakeAmount)
        {
            var startGameFunction = new StartGameFunction();
                startGameFunction.GameId = gameId;
                startGameFunction.Player1 = player1;
                startGameFunction.Player2 = player2;
                startGameFunction.StakeAmount = stakeAmount;
            
             return ContractHandler.SendRequestAsync(startGameFunction);
        }

        public virtual Task<TransactionReceipt> StartGameRequestAndWaitForReceiptAsync(string gameId, string player1, string player2, BigInteger stakeAmount, CancellationTokenSource cancellationToken = null)
        {
            var startGameFunction = new StartGameFunction();
                startGameFunction.GameId = gameId;
                startGameFunction.Player1 = player1;
                startGameFunction.Player2 = player2;
                startGameFunction.StakeAmount = stakeAmount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(startGameFunction, cancellationToken);
        }

        public virtual Task<string> TransferOwnershipRequestAsync(TransferOwnershipFunction transferOwnershipFunction)
        {
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public virtual Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(TransferOwnershipFunction transferOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public virtual Task<string> TransferOwnershipRequestAsync(string newOwner)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public virtual Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(string newOwner, CancellationTokenSource cancellationToken = null)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public Task<BigInteger> UserBalancesQueryAsync(UserBalancesFunction userBalancesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UserBalancesFunction, BigInteger>(userBalancesFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> UserBalancesQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var userBalancesFunction = new UserBalancesFunction();
                userBalancesFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<UserBalancesFunction, BigInteger>(userBalancesFunction, blockParameter);
        }

        public virtual Task<string> WithdrawStakesRequestAsync(WithdrawStakesFunction withdrawStakesFunction)
        {
             return ContractHandler.SendRequestAsync(withdrawStakesFunction);
        }

        public virtual Task<TransactionReceipt> WithdrawStakesRequestAndWaitForReceiptAsync(WithdrawStakesFunction withdrawStakesFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawStakesFunction, cancellationToken);
        }

        public virtual Task<string> WithdrawStakesRequestAsync(BigInteger amount)
        {
            var withdrawStakesFunction = new WithdrawStakesFunction();
                withdrawStakesFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(withdrawStakesFunction);
        }

        public virtual Task<TransactionReceipt> WithdrawStakesRequestAndWaitForReceiptAsync(BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var withdrawStakesFunction = new WithdrawStakesFunction();
                withdrawStakesFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawStakesFunction, cancellationToken);
        }

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(DepositStakesFunction),
                typeof(GamesFunction),
                typeof(GetBalanceFunction),
                typeof(GetGameFunction),
                typeof(GetLockedStakeFunction),
                typeof(LockedStakesFunction),
                typeof(OwnerFunction),
                typeof(RenounceOwnershipFunction),
                typeof(ResolveGameFunction),
                typeof(StartGameFunction),
                typeof(TransferOwnershipFunction),
                typeof(UserBalancesFunction),
                typeof(WithdrawStakesFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(GameResolvedEventDTO),
                typeof(GameStartedEventDTO),
                typeof(OwnershipTransferredEventDTO),
                typeof(StakesDepositedEventDTO),
                typeof(StakesLockedEventDTO),
                typeof(StakesUnlockedEventDTO),
                typeof(StakesWithdrawnEventDTO)
            };
        }

        public override List<Type> GetAllErrorTypes()
        {
            return new List<Type>
            {
                typeof(OwnableInvalidOwnerError),
                typeof(OwnableUnauthorizedAccountError)
            };
        }
    }
}
