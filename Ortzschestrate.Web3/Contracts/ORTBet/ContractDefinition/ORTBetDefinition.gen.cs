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

    public partial class DepositStakesFunction : DepositStakesFunctionBase { }

    [Function("depositStakes")]
    public class DepositStakesFunctionBase : FunctionMessage
    {

    }

    public partial class GamesFunction : GamesFunctionBase { }

    [Function("games", typeof(GamesOutputDTO))]
    public class GamesFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class LockedStakesFunction : LockedStakesFunctionBase { }

    [Function("lockedStakes", "uint256")]
    public class LockedStakesFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
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

    public partial class ResolveGameFunction : ResolveGameFunctionBase { }

    [Function("resolveGame")]
    public class ResolveGameFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "_gameId", 1)]
        public virtual byte[] GameId { get; set; }
        [Parameter("uint8", "_result", 2)]
        public virtual byte Result { get; set; }
    }

    public partial class StartGameFunction : StartGameFunctionBase { }

    [Function("startGame")]
    public class StartGameFunctionBase : FunctionMessage
    {
        [Parameter("string", "_gameId", 1)]
        public virtual string GameId { get; set; }
        [Parameter("address", "_player1", 2)]
        public virtual string Player1 { get; set; }
        [Parameter("address", "_player2", 3)]
        public virtual string Player2 { get; set; }
        [Parameter("uint256", "_stakeAmount", 4)]
        public virtual BigInteger StakeAmount { get; set; }
    }

    public partial class TransferOwnershipFunction : TransferOwnershipFunctionBase { }

    [Function("transferOwnership")]
    public class TransferOwnershipFunctionBase : FunctionMessage
    {
        [Parameter("address", "newOwner", 1)]
        public virtual string NewOwner { get; set; }
    }

    public partial class UserBalancesFunction : UserBalancesFunctionBase { }

    [Function("userBalances", "uint256")]
    public class UserBalancesFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class WithdrawStakesFunction : WithdrawStakesFunctionBase { }

    [Function("withdrawStakes")]
    public class WithdrawStakesFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "amount", 1)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class GameResolvedEventDTO : GameResolvedEventDTOBase { }

    [Event("GameResolved")]
    public class GameResolvedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "gameId", 1, true )]
        public virtual byte[] GameId { get; set; }
        [Parameter("uint8", "result", 2, false )]
        public virtual byte Result { get; set; }
    }

    public partial class GameStartedEventDTO : GameStartedEventDTOBase { }

    [Event("GameStarted")]
    public class GameStartedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "gameId", 1, true )]
        public virtual byte[] GameId { get; set; }
        [Parameter("address", "player1", 2, true )]
        public virtual string Player1 { get; set; }
        [Parameter("address", "player2", 3, true )]
        public virtual string Player2 { get; set; }
        [Parameter("uint256", "stakeAmount", 4, false )]
        public virtual BigInteger StakeAmount { get; set; }
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

    public partial class StakesDepositedEventDTO : StakesDepositedEventDTOBase { }

    [Event("StakesDeposited")]
    public class StakesDepositedEventDTOBase : IEventDTO
    {
        [Parameter("address", "player", 1, true )]
        public virtual string Player { get; set; }
        [Parameter("uint256", "amount", 2, false )]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class StakesLockedEventDTO : StakesLockedEventDTOBase { }

    [Event("StakesLocked")]
    public class StakesLockedEventDTOBase : IEventDTO
    {
        [Parameter("address", "player", 1, true )]
        public virtual string Player { get; set; }
        [Parameter("uint256", "amount", 2, false )]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class StakesUnlockedEventDTO : StakesUnlockedEventDTOBase { }

    [Event("StakesUnlocked")]
    public class StakesUnlockedEventDTOBase : IEventDTO
    {
        [Parameter("address", "player", 1, true )]
        public virtual string Player { get; set; }
        [Parameter("uint256", "amount", 2, false )]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class StakesWithdrawnEventDTO : StakesWithdrawnEventDTOBase { }

    [Event("StakesWithdrawn")]
    public class StakesWithdrawnEventDTOBase : IEventDTO
    {
        [Parameter("address", "player", 1, true )]
        public virtual string Player { get; set; }
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



    public partial class GamesOutputDTO : GamesOutputDTOBase { }

    [FunctionOutput]
    public class GamesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "player1", 1)]
        public virtual string Player1 { get; set; }
        [Parameter("address", "player2", 2)]
        public virtual string Player2 { get; set; }
        [Parameter("uint256", "stakeAmount", 3)]
        public virtual BigInteger StakeAmount { get; set; }
        [Parameter("bool", "active", 4)]
        public virtual bool Active { get; set; }
    }

    public partial class LockedStakesOutputDTO : LockedStakesOutputDTOBase { }

    [FunctionOutput]
    public class LockedStakesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class OwnerOutputDTO : OwnerOutputDTOBase { }

    [FunctionOutput]
    public class OwnerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }









    public partial class UserBalancesOutputDTO : UserBalancesOutputDTOBase { }

    [FunctionOutput]
    public class UserBalancesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }


}
