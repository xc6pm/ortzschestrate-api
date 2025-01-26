using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;

namespace Ortzschestrate.Web3.Utilities;

public class Validator
{
    public static bool IsValidEthereumAddressHexFormat(string address)
    {
        return !string.IsNullOrWhiteSpace(address) &&
               address.HasHexPrefix() &&
               address.IsValidEthereumAddressLength() &&
               address.IsHex();
    }
}