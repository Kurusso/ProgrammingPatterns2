export class magicConsts {
    static base_url = "https://localhost:7075";

    static loginEndpoint = this.base_url + "/api/Auth/Login";

    //Accounts
    static getAccountsEndpoint = this.base_url + "/api/Account/User/";
    static getAccountEndpoint = this.base_url + "/api/Account/GetInfo/";
    static createAccountEndpoint = this.base_url + "/api/Account/Create";
    static closeAccountEndpoint = this.base_url + "/api/Account/Close";
    static getUserEndpoint = this.base_url + "/api/Profile";

    //Operations
    static depositEndpoint = this.base_url + "/api/Operations/Deposit";
    static withdrawEndpoint = this.base_url + "/api/Operations/Withdraw";

    //Credits
    static getCreditsEndpoint = this.base_url + "/api/Credit/GetUserCredits";
    static getCreditEndpoint = this.base_url + "/api/Credit/GetInfo";
    static repayCreditEndpoint = this.base_url + "/api/Credit/Repay";
    static getCreditRatesEndpoint = this.base_url + "/api/CreditRates/GetAll";
    static takeCreditEndpoint = this.base_url + "/api/Credit/Take"

    //WebSocket
    static AccountInfoHub = this.base_url + "/AccountHub";

    //Auth
    static authService_url = "https://localhost:7212"
    static LoginRedirectUrl = this.authService_url + "/auth"

}

