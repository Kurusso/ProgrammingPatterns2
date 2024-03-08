namespace client_bank_backend;

public static class MagicConstants
{
    private static readonly string BaseUrlCore = "https://localhost:7143";
    public static readonly string GetAccountsEndpoint = BaseUrlCore +"/api/Account/User/";
    public static readonly string GetAccountEndpoint = BaseUrlCore +"/api/Account/GetInfo/";
    public static readonly string CreateAccountEndpoint = BaseUrlCore +"/api/Account/Create";
}