const base_url="https://localhost:7075";
export const loginEndpoint=base_url+"/api/Auth/Login";
export const getAccountsEndpoint=base_url+"/api/Account/User/";
export const getAccountEndpoint=base_url+"/api/Account/GetInfo/";
export const createAccountEndpoint=base_url+"/api/Account/Create";
export const closeAccountEndpoint=base_url+"/api/Account/Close";
export const getUserEndpoint=base_url+"/api/Profile/";
export const depositEndpoint=base_url+"/api/Operations/Deposit";
export const withdrawEndpoint=base_url+"/api/Operations/Withdraw";
export const getCreditsEndpoint=base_url+"/api/Credit/GetUserCredits";
export const getCreditEndpoint=base_url+"/api/Credit/GetInfo";
export const repayCreditEndpoint=base_url+"/api/Credit/Repay";

export const getCreditRatesEndpoint=base_url+ "/api/CreditRates/GetAll";
export const takeCreditEndpoint=base_url+"/api/Credit/Take"
