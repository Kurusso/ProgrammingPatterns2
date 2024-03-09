namespace client_bank_backend;

public static class MagicConstants
{
    private static readonly string BaseUrlCore = "https://localhost:7143";
    private static readonly string BaseUrlCredit = "https://localhost:7186";
    
    public static readonly string GetAccountsEndpoint = BaseUrlCore +"/api/Account/User/";
    public static readonly string GetAccountEndpoint = BaseUrlCore +"/api/Account/GetInfo/";
    public static readonly string CreateAccountEndpoint = BaseUrlCore +"/api/Account/Create";
    public static readonly string CloseAccountEndpoint = BaseUrlCore +"/api/Account/Create";
    public static readonly string DepositEndpoint = BaseUrlCore +"/api/Operations/Deposit";
    public static readonly string WithdrawEndpoint = BaseUrlCore +"/api/Operations/Withdraw";
    
    public static readonly string GetCreditRatesEndpoint = BaseUrlCredit +"/api/CreditRates/GetAll"; 
    public static readonly string TakeCreditEndpoint = BaseUrlCredit +"/api/Credit/Take"; 
    //https://localhost:7186/api/Credit/Take
    //?creditRateId=fa9fae7c-1ed3-40ae-bc80-31689bd40524
    //&userId=9985d7a3-caeb-40f3-8258-9a27d1548053
    //&accountId=72589b07-1c3e-42eb-9615-51ffdee45256
    //&currency=1&moneyAmount=1000&monthPay=100
    public static readonly string GetUserCreditsEndpoint = BaseUrlCredit +"/api/Credit/GetUserCredits"; 
    public static readonly string GetCreditInfoEndpoint = BaseUrlCredit +"/api/Credit/GetInfo"; //https://localhost:7186/api/Credit/GetInfo?id=590305df-657f-41d2-adfc-7720a3a61bab&userId=9985d7a3-caeb-40f3-8258-9a27d1548053
    public static readonly string RepayCreditEndpoint = BaseUrlCredit +"/api/Credit/Repay"; //https://localhost:7186/api/Credit/Repay?id=590305df-657f-41d2-adfc-7720a3a61bab&userId=9985d7a3-caeb-40f3-8258-9a27d1548053&moneyAmmount=100&currency=1&accountId=72589b07-1c3e-42eb-9615-51ffdee45256
}