using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Ortzschestrate.Web3.Contracts.ORTBet.ContractDefinition
{
    public partial class Game : GameBase { }

    public class GameBase 
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
}
