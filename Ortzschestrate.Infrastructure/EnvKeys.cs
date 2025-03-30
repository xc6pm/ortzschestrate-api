namespace Ortzschestrate.Infrastructure;

public static class EnvKeys
{
    public const string JwtSecret = "ORTZSCHESTRATE_JWT_SECRET";
    public const string JwtRefreshSecret = "ORTZSCHESTRATE_JWT_REFRESH_SECRET";

    public const string ConnectionString = "ORTZSCHESTRATE_CONNECTION_STRING";

    public const string GoogleClientId = "ORTZSCHESTRATE_GOOGLE_CLIENT_ID";
    public const string GoogleClientSecret = "ORTZSCHESTRATE_GOOGLE_CLIENT_SECRET";

    // Web3
    public const string PrivateKey = "ORTZSCHESTRATE_PRIVATE_KEY";
    public const string ChainUrl = "ORTZSCHESTRATE_CHAIN_URL";
    public const string ContractAddress = "ORTZSCHESTRATE_CONTRACT_ADDRESS";


    // Email
    public const string MailSender = "ORTZSCHESTRATE_MAIL_SENDER";
    public const string MailAddress = "ORTZSCHESTRATE_MAIL_ADDRESS";
    public const string MailPassword = "ORTZSCHESTRATE_MAIL_PASSWORD";
    
    
    public const string DisableWalletVerification = "ORTZSCHESTRATE_DISABLE_WALLET_VERIFICATION";
    
    public const string CorsWhiteList = "ORTZSCHESTRATE_CORS_WHITELIST";
}