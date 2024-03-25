export class magicConsts{
    static base_url="http://localhost:5082";
    static loginEndpoint=this.base_url+"/api/Auth/Login";
    static getAccountsEndpoint=this.base_url+"/api/Account/User/";
    static getAccountEndpoint=this.base_url+"/api/Account/GetInfo/";
    static createAccountEndpoint=this.base_url+"/api/Account/Create";
    static closeAccountEndpoint=this.base_url+"/api/Account/Close";
    static getUserEndpoint=this.base_url+"/api/Profile";
    static depositEndpoint=this.base_url+"/api/Operations/Deposit";
    static withdrawEndpoint=this.base_url+"/api/Operations/Withdraw";
    static getCreditsEndpoint=this.base_url+"/api/Credit/GetUserCredits";
    static getCreditEndpoint=this.base_url+"/api/Credit/GetInfo";
    static repayCreditEndpoint=this.base_url+"/api/Credit/Repay";

    static getCreditRatesEndpoint=this.base_url+ "/api/CreditRates/GetAll";
    static takeCreditEndpoint=this.base_url+"/api/Credit/Take"

}

