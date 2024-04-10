namespace client_bank_backend;

public static class MagicConstants
{
    private static readonly string BaseUrlCore = "https://localhost:7143";
    private static readonly string BaseUrlCredit = "https://localhost:7186";
    private static readonly string BaseUrlUserService = "https://localhost:7212";
    private static readonly string BaseUrlUserSettings = "https://localhost:7059";
    
    public static readonly string GetAccountsEndpoint = BaseUrlCore +"/api/Account/User/";
    public static readonly string GetAccountEndpoint = BaseUrlCore +"/api/Account/GetInfo/";
    public static readonly string CreateAccountEndpoint = BaseUrlCore +"/api/Account/Create";
    public static readonly string CloseAccountEndpoint = BaseUrlCore +"/api/Account/Close";
    public static readonly string DepositEndpoint = BaseUrlCore +"/api/Operations/Deposit";
    public static readonly string WithdrawEndpoint = BaseUrlCore +"/api/Operations/Withdraw";
    public static readonly string TransferEndpoint = BaseUrlCore +"/api/Operations/Transfer";
    
    //https://localhost:7143/api/Operations/Transfer
    
    
    public static readonly string GetCreditRatesEndpoint = BaseUrlCredit +"/api/CreditRates/GetAll"; 
    public static readonly string TakeCreditEndpoint = BaseUrlCredit +"/api/Credit/Take"; 

    
    public static readonly string GetUserCreditsEndpoint = BaseUrlCredit +"/api/Credit/GetUserCredits"; 
    public static readonly string GetCreditInfoEndpoint = BaseUrlCredit +"/api/Credit/GetInfo"; //https://localhost:7186/api/Credit/GetInfo?id=590305df-657f-41d2-adfc-7720a3a61bab&userId=9985d7a3-caeb-40f3-8258-9a27d1548053
    public static readonly string RepayCreditEndpoint = BaseUrlCredit +"/api/Credit/Repay"; //https://localhost:7186/api/Credit/Repay?id=590305df-657f-41d2-adfc-7720a3a61bab&userId=9985d7a3-caeb-40f3-8258-9a27d1548053&moneyAmmount=100&currency=1&accountId=72589b07-1c3e-42eb-9615-51ffdee45256

    public static readonly string LoginEndpoint = BaseUrlUserService + "/api/clients/login";
    public static readonly string GetUserProfileEndpoint = BaseUrlUserService + "/api/clients/info";
    public static readonly string AuthorizeEndpoint = BaseUrlUserService + "/auth";
    public static readonly string AuthorizeTokenEndpoint = BaseUrlUserService + "/auth/token";
    
    public static readonly string ValidateTokenEndpoint = BaseUrlUserService + "/auth/validate";
    
    public static readonly string GetThemeEndpoint = BaseUrlUserSettings + "/api/theme";
    public static readonly string ChangeThemeEndpoint = BaseUrlUserSettings + "/api/theme";
    public static readonly string GetHiddenAccountsEndpoint = BaseUrlUserSettings + "/api/hiddenAccount/Accounts";
    public static readonly string ChangeHiddenAccountsEndpoint = BaseUrlUserSettings + "/api/hiddenAccount/Visibility";

    
    
    public static readonly string AccountHub = BaseUrlCore + "/client";

}