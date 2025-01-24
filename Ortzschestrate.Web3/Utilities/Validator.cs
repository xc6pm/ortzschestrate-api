using Nethereum.Hex.HexConvertors.Extensions;

namespace Ortzschestrate.Web3.Utilities;

public class Validator
{
    public static bool IsValidEthereumAddressHexFormat(string address)
    {
        return !string.IsNullOrWhiteSpace(address) &&
               address.HasHexPrefix() &&
               address.Length == 40 &&
               address.IsHex();
    }
}