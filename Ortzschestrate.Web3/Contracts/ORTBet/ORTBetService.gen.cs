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

        public virtual Task<string> CancelPendingGameRequestAsync(CancelPendingGameFunction cancelPendingGameFunction)
        {
             return ContractHandler.SendRequestAsync(cancelPendingGameFunction);
        }

        public virtual Task<TransactionReceipt> CancelPendingGameRequestAndWaitForReceiptAsync(CancelPendingGameFunction cancelPendingGameFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(cancelPendingGameFunction, cancellationToken);
        }

        public virtual Task<string> CancelPendingGameRequestAsync(byte[] gameId)
        {
            var cancelPendingGameFunction = new CancelPendingGameFunction();
                cancelPendingGameFunction.GameId = gameId;
            
             return ContractHandler.SendRequestAsync(cancelPendingGameFunction);
        }

        public virtual Task<TransactionReceipt> CancelPendingGameRequestAndWaitForReceiptAsync(byte[] gameId, CancellationTokenSource cancellationToken = null)
        {
            var cancelPendingGameFunction = new CancelPendingGameFunction();
                cancelPendingGameFunction.GameId = gameId;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(cancelPendingGameFunction, cancellationToken);
        }

        public virtual Task<string> CreatePendingGameRequestAsync(CreatePendingGameFunction createPendingGameFunction)
        {
             return ContractHandler.SendRequestAsync(createPendingGameFunction);
        }

        public virtual Task<TransactionReceipt> CreatePendingGameRequestAndWaitForReceiptAsync(CreatePendingGameFunction createPendingGameFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createPendingGameFunction, cancellationToken);
        }

        public virtual Task<string> CreatePendingGameRequestAsync(string gameId, string player1Address, BigInteger betAmount)
        {
            var createPendingGameFunction = new CreatePendingGameFunction();
                createPendingGameFunction.GameId = gameId;
                createPendingGameFunction.Player1Address = player1Address;
                createPendingGameFunction.BetAmount = betAmount;
            
             return ContractHandler.SendRequestAsync(createPendingGameFunction);
        }

        public virtual Task<TransactionReceipt> CreatePendingGameRequestAndWaitForReceiptAsync(string gameId, string player1Address, BigInteger betAmount, CancellationTokenSource cancellationToken = null)
        {
            var createPendingGameFunction = new CreatePendingGameFunction();
                createPendingGameFunction.GameId = gameId;
                createPendingGameFunction.Player1Address = player1Address;
                createPendingGameFunction.BetAmount = betAmount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createPendingGameFunction, cancellationToken);
        }

        public virtual Task<string> DepositBetAmountRequestAsync(DepositBetAmountFunction depositBetAmountFunction)
        {
             return ContractHandler.SendRequestAsync(depositBetAmountFunction);
        }

        public virtual Task<TransactionReceipt> DepositBetAmountRequestAndWaitForReceiptAsync(DepositBetAmountFunction depositBetAmountFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositBetAmountFunction, cancellationToken);
        }

        public virtual Task<string> DepositBetAmountRequestAsync(byte[] gameId)
        {
            var depositBetAmountFunction = new DepositBetAmountFunction();
                depositBetAmountFunction.GameId = gameId;
            
             return ContractHandler.SendRequestAsync(depositBetAmountFunction);
        }

        public virtual Task<TransactionReceipt> DepositBetAmountRequestAndWaitForReceiptAsync(byte[] gameId, CancellationTokenSource cancellationToken = null)
        {
            var depositBetAmountFunction = new DepositBetAmountFunction();
                depositBetAmountFunction.GameId = gameId;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositBetAmountFunction, cancellationToken);
        }

        public virtual Task<string> EndGameRequestAsync(EndGameFunction endGameFunction)
        {
             return ContractHandler.SendRequestAsync(endGameFunction);
        }

        public virtual Task<TransactionReceipt> EndGameRequestAndWaitForReceiptAsync(EndGameFunction endGameFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(endGameFunction, cancellationToken);
        }

        public virtual Task<string> EndGameRequestAsync(byte[] gameId, byte gameResult)
        {
            var endGameFunction = new EndGameFunction();
                endGameFunction.GameId = gameId;
                endGameFunction.GameResult = gameResult;
            
             return ContractHandler.SendRequestAsync(endGameFunction);
        }

        public virtual Task<TransactionReceipt> EndGameRequestAndWaitForReceiptAsync(byte[] gameId, byte gameResult, CancellationTokenSource cancellationToken = null)
        {
            var endGameFunction = new EndGameFunction();
                endGameFunction.GameId = gameId;
                endGameFunction.GameResult = gameResult;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(endGameFunction, cancellationToken);
        }

        public virtual Task<FinishedGamesOutputDTO> FinishedGamesQueryAsync(FinishedGamesFunction finishedGamesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<FinishedGamesFunction, FinishedGamesOutputDTO>(finishedGamesFunction, blockParameter);
        }

        public virtual Task<FinishedGamesOutputDTO> FinishedGamesQueryAsync(byte[] returnValue1, BlockParameter blockParameter = null)
        {
            var finishedGamesFunction = new FinishedGamesFunction();
                finishedGamesFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryDeserializingToObjectAsync<FinishedGamesFunction, FinishedGamesOutputDTO>(finishedGamesFunction, blockParameter);
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

        public virtual Task<string> JoinGameRequestAsync(JoinGameFunction joinGameFunction)
        {
             return ContractHandler.SendRequestAsync(joinGameFunction);
        }

        public virtual Task<TransactionReceipt> JoinGameRequestAndWaitForReceiptAsync(JoinGameFunction joinGameFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(joinGameFunction, cancellationToken);
        }

        public virtual Task<string> JoinGameRequestAsync(byte[] gameId, string player2Address)
        {
            var joinGameFunction = new JoinGameFunction();
                joinGameFunction.GameId = gameId;
                joinGameFunction.Player2Address = player2Address;
            
             return ContractHandler.SendRequestAsync(joinGameFunction);
        }

        public virtual Task<TransactionReceipt> JoinGameRequestAndWaitForReceiptAsync(byte[] gameId, string player2Address, CancellationTokenSource cancellationToken = null)
        {
            var joinGameFunction = new JoinGameFunction();
                joinGameFunction.GameId = gameId;
                joinGameFunction.Player2Address = player2Address;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(joinGameFunction, cancellationToken);
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

        public Task<BigInteger> UserStakesQueryAsync(UserStakesFunction userStakesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UserStakesFunction, BigInteger>(userStakesFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> UserStakesQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var userStakesFunction = new UserStakesFunction();
                userStakesFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<UserStakesFunction, BigInteger>(userStakesFunction, blockParameter);
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
                typeof(CancelPendingGameFunction),
                typeof(CreatePendingGameFunction),
                typeof(DepositBetAmountFunction),
                typeof(EndGameFunction),
                typeof(FinishedGamesFunction),
                typeof(GamesFunction),
                typeof(JoinGameFunction),
                typeof(OwnerFunction),
                typeof(RenounceOwnershipFunction),
                typeof(TransferOwnershipFunction),
                typeof(UserStakesFunction),
                typeof(WithdrawStakesFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(ChangeReturnedEventDTO),
                typeof(GameCanceledEventDTO),
                typeof(GameCreatedEventDTO),
                typeof(GameEndedEventDTO),
                typeof(GameJoinedEventDTO),
                typeof(GameStakesDepositedEventDTO),
                typeof(OwnershipTransferredEventDTO),
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
