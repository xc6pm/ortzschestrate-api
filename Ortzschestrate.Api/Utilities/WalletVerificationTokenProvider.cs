using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Ortzschestrate.Api.Utilities;

public class WalletVerificationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
{
    public const string Key = "WalletVerificationTokenProvider";
    public const string Purpose = "WALLET_VERIFICATION";

    public WalletVerificationTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<WalletVerificationTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}

public class WalletVerificationTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public WalletVerificationTokenProviderOptions()
    {
        Name = "WalletDataProtectorTokenProvider";
        TokenLifespan = TimeSpan.FromMinutes(30);
    }
}