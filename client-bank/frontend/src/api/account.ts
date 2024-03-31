import {magicConsts} from "./magicConst";

export enum Currency {
    Ruble,
    Dollar,
    Euro
}

export enum OperationType {
    Deposit,
    Withdraw
}

export interface Money {
    amount: number;
    currency: Currency;
}

export interface AccountData {
    id: string,
    money: Money,
    operationsHistory: OperationsHistory[]
}

export interface OperationsHistory {
    accountId: string;
    id: string;
    moneyAmmount: Money;
    moneyAmmountInAccountCurrency: number;
    operationType: OperationType;
}

export class AccountService {
    static async getAccounts(token: string) {
        try {
            console.log(token);
            const response = await fetch(magicConsts.getAccountsEndpoint + token)
            let data: AccountData[] = await response.json()
            console.log(data)
            return data
        } catch (error) {
            throw error;
        }
    }

    static async getAccount(accountId: string, userId: string) {
        try {
            const response = await fetch(`${magicConsts.getAccountEndpoint}${accountId}?userId=${userId}`);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data: AccountData = await response.json();
            return data;
        } catch (error) {
            console.error('An error occurred while fetching the account:', error);
            throw error;
        }
    }

    static async createAccount(userId: string, currency: Currency) {
        try {
            const response = await fetch(`${magicConsts.createAccountEndpoint}?userId=${userId}&currency=${currency}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
            })
            if (!response.ok) {
                const errorData = await response.text();
                throw new Error(`HTTP error! status: ${response.status}, message: ${errorData}`);
            }

        } catch (error) {
            console.error('An error occurred while creating the account')
            throw error;
        }
    }

    static async closeAccount(userId: string, accountId: string) {
        try {
            const params = new URLSearchParams({userId, accountId});
            const response = await fetch(`${magicConsts.closeAccountEndpoint}?${params}`, {method: 'DELETE'});

            if (!response.ok) {
                const errorData = await response.text();
                throw new Error(`HTTP error! status: ${response.status}, message: ${errorData}`);
            }
        } catch (error) {
            console.error('An error occurred while deleting the account');
            throw error;
        }
    }


}


