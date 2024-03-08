import {getAccountsEndpoint} from "./magicConst";

export enum Currency {
    Ruble,
    Dollar,
    Euro
}

export enum OperationType {
    Deposit,
    Withdraw
}
interface Money {
    amount: number;
    currency: Currency;
}
export interface AccountData {
    id: string,
    money:Money ,
    operationsHistory:OperationHistory[]
}

export interface OperationHistory {
    accountId: string;
    id: string;
    moneyAmmount: Money;
    moneyAmmountInAccountCurrency: number;
    operationType: OperationType;
}
export async function getAccounts(token: string) {
    try {
        console.log(token);
        const response = await fetch(getAccountsEndpoint + token)
        let data:AccountData[] = await response.json()
        console.log(data)
        return data
    } catch (error) {
        throw error;
    }
}