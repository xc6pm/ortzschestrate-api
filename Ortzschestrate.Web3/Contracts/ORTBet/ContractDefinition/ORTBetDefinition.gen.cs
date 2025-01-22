using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace Ortzschestrate.Web3.Contracts.ORTBet.ContractDefinition
{


    public partial class ORTBetDeployment : ORTBetDeploymentBase
    {
        public ORTBetDeployment() : base(BYTECODE) { }
        public ORTBetDeployment(string byteCode) : base(byteCode) { }
    }

    public class ORTBetDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "";
        public ORTBetDeploymentBase() : base(BYTECODE) { }
        public ORTBetDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class CancelPendingGameFunction : CancelPendingGameFunctionBase { }

    [Function("cancelPendingGame")]
    public class CancelPendingGameFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "_gameId", 1)]
        public virtual byte[] GameId { get; set; }
    }

    public partial class CreatePendingGameFunction : CreatePendingGameFunctionBase { }

    [Function("createPendingGame")]
    public class CreatePendingGameFunctionBase : FunctionMessage
    {
        [Parameter("string", "_gameId", 1)]
        public virtual string GameId { get; set; }
        [Parameter("address", "_player1Address", 2)]
        public virtual string Player1Address { get; set; }
        [Parameter("uint256", "_betAmount", 3)]
        public virtual BigInteger BetAmount { get; set; }
    }

    public partial class DepositBetAmountFunction : DepositBetAmountFunctionBase { }

    [Function("depositBetAmount")]
    public class DepositBetAmountFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "_gameId", 1)]
        public virtual byte[] GameId { get; set; }
    }

    public partial class EndGameFunction : EndGameFunctionBase { }

    [Function("endGame")]
    public class EndGameFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "_gameId", 1)]
        public virtual byte[] GameId { get; set; }
        [Parameter("uint8", "_gameResult", 2)]
        public virtual byte GameResult { get; set; }
    }

    public partial class FinishedGamesFunction : FinishedGamesFunctionBase { }

    [Function("finishedGames", typeof(FinishedGamesOutputDTO))]
    public class FinishedGamesFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class GamesFunction : GamesFunctionBase { }

    [Function("games", typeof(GamesOutputDTO))]
    public class GamesFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class JoinGameFunction : JoinGameFunctionBase { }

    [Function("joinGame")]
    public class JoinGameFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "_gameId", 1)]
        public virtual byte[] GameId { get; set; }
        [Parameter("address", "_player2Address", 2)]
        public virtual string Player2Address { get; set; }
    }

    public partial class OwnerFunction : OwnerFunctionBase { }

    [Function("owner", "address")]
    public class OwnerFunctionBase : FunctionMessage
    {

    }

    public partial class RenounceOwnershipFunction : RenounceOwnershipFunctionBase { }

    [Function("renounceOwnership")]
    public class RenounceOwnershipFunctionBase : FunctionMessage
    {

    }

    public partial class TransferOwnershipFunction : TransferOwnershipFunctionBase { }

    [Function("transferOwnership")]
    public class TransferOwnershipFunctionBase : FunctionMessage
    {
        [Parameter("address", "newOwner", 1)]
        public virtual string NewOwner { get; set; }
    }

    public partial class UserStakesFunction : UserStakesFunctionBase { }

    [Function("userStakes", "uint256")]
    public class UserStakesFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class WithdrawStakesFunction : WithdrawStakesFunctionBase { }

    [Function("withdrawStakes")]
    public class WithdrawStakesFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_amount", 1)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class ChangeReturnedEventDTO : ChangeReturnedEventDTOBase { }

    [Event("ChangeReturned")]
    public class ChangeReturnedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "gameId", 1, true )]
        public virtual byte[] GameId { get; set; }
        [Parameter("address", "playerAddress", 2, true )]
        public virtual string PlayerAddress { get; set; }
        [Parameter("uint256", "changeAmount", 3, false )]
        public virtual BigInteger ChangeAmount { get; set; }
    }

    public partial class GameCanceledEventDTO : GameCanceledEventDTOBase { }

    [Event("GameCanceled")]
    public class GameCanceledEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "gameId", 1, true )]
        public virtual byte[] GameId { get; set; }
    }

    public partial class GameCreatedEventDTO : GameCreatedEventDTOBase { }

    [Event("GameCreated")]
    public class GameCreatedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "gameId", 1, true )]
        public virtual byte[] GameId { get; set; }
        [Parameter("address", "player1", 2, true )]
        public virtual string Player1 { get; set; }
        [Parameter("uint256", "betAmount", 3, false )]
        public virtual BigInteger BetAmount { get; set; }
    }

    public partial class GameEndedEventDTO : GameEndedEventDTOBase { }

    [Event("GameEnded")]
    public class GameEndedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "gameId", 1, true )]
        public virtual byte[] GameId { get; set; }
        [Parameter("uint8", "result", 2, false )]
        public virtual byte Result { get; set; }
    }

    public partial class GameJoinedEventDTO : GameJoinedEventDTOBase { }

    [Event("GameJoined")]
    public class GameJoinedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "gameId", 1, true )]
        public virtual byte[] GameId { get; set; }
        [Parameter("address", "player2", 2, true )]
        public virtual string Player2 { get; set; }
    }

    public partial class GameStakesDepositedEventDTO : GameStakesDepositedEventDTOBase { }

    [Event("GameStakesDeposited")]
    public class GameStakesDepositedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "gameId", 1, true )]
        public virtual byte[] GameId { get; set; }
        [Parameter("address", "playerAddress", 2, true )]
        public virtual string PlayerAddress { get; set; }
        [Parameter("bool", "isDepositerPlayer1", 3, false )]
        public virtual bool IsDepositerPlayer1 { get; set; }
    }

    public partial class OwnershipTransferredEventDTO : OwnershipTransferredEventDTOBase { }

    [Event("OwnershipTransferred")]
    public class OwnershipTransferredEventDTOBase : IEventDTO
    {
        [Parameter("address", "previousOwner", 1, true )]
        public virtual string PreviousOwner { get; set; }
        [Parameter("address", "newOwner", 2, true )]
        public virtual string NewOwner { get; set; }
    }

    public partial class StakesWithdrawnEventDTO : StakesWithdrawnEventDTOBase { }

    [Event("StakesWithdrawn")]
    public class StakesWithdrawnEventDTOBase : IEventDTO
    {
        [Parameter("address", "playerAddress", 1, true )]
        public virtual string PlayerAddress { get; set; }
        [Parameter("uint256", "amount", 2, false )]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class OwnableInvalidOwnerError : OwnableInvalidOwnerErrorBase { }

    [Error("OwnableInvalidOwner")]
    public class OwnableInvalidOwnerErrorBase : IErrorDTO
    {
        [Parameter("address", "owner", 1)]
        public virtual string Owner { get; set; }
    }

    public partial class OwnableUnauthorizedAccountError : OwnableUnauthorizedAccountErrorBase { }

    [Error("OwnableUnauthorizedAccount")]
    public class OwnableUnauthorizedAccountErrorBase : IErrorDTO
    {
        [Parameter("address", "account", 1)]
        public virtual string Account { get; set; }
    }









    public partial class FinishedGamesOutputDTO : FinishedGamesOutputDTOBase { }

    [FunctionOutput]
    public class FinishedGamesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "betAmount", 1)]
        public virtual BigInteger BetAmount { get; set; }
        [Parameter("address", "player1Address", 2)]
        public virtual string Player1Address { get; set; }
        [Parameter("address", "player2Address", 3)]
        public virtual string Player2Address { get; set; }
        [Parameter("uint8", "result", 4)]
        public virtual byte Result { get; set; }
    }

    public partial class GamesOutputDTO : GamesOutputDTOBase { }

    [FunctionOutput]
    public class GamesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "betAmount", 1)]
        public virtual BigInteger BetAmount { get; set; }
        [Parameter("address", "player1Address", 2)]
        public virtual string Player1Address { get; set; }
        [Parameter("address", "player2Address", 3)]
        public virtual string Player2Address { get; set; }
        [Parameter("bool", "started", 4)]
        public virtual bool Started { get; set; }
        [Parameter("bool", "player1Paid", 5)]
        public virtual bool Player1Paid { get; set; }
        [Parameter("bool", "player2Paid", 6)]
        public virtual bool Player2Paid { get; set; }
    }



    public partial class OwnerOutputDTO : OwnerOutputDTOBase { }

    [FunctionOutput]
    public class OwnerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }





    public partial class UserStakesOutputDTO : UserStakesOutputDTOBase { }

    [FunctionOutput]
    public class UserStakesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }


}
