import {magicConsts} from "./magicConst";
import {getAccessToken} from "./auth";

export enum Currency {
    Ruble,
    Dollar,
    Euro
}

export enum OperationType {
    Deposit,
    Withdraw,
    TransferGet,
    TransferSend,
}

export interface Money {
    amount: number;
    currency: Currency;
}

export interface AccountData {
    id: string,
    money: Money,
    operationsHistory: OperationsHistory[]
    isHidden:boolean
}

export interface OperationsHistory {
    accountId: string;
    id: string;
    moneyAmmount: Money;
    moneyAmmountInAccountCurrency: number;
    operationType: OperationType;
}

export class AccountService {
    static async getAccounts() {
        try {

            const response = await fetch(magicConsts.getAccountsEndpoint,
                {
                    headers:
                        {
                            'Authorization': getAccessToken(),
                        }
                })
            let data: AccountData[] = await response.json()
            console.log(`accounts:`)
            console.log(data)
            return data
        } catch (error) {
            throw error;
        }
    }

    static async getAccount(accountId: string) {
        try {
            const response = await fetch(`${magicConsts.getAccountEndpoint}${accountId}`, {
                headers:
                    {
                        'Authorization': getAccessToken(),
                    }
            });

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

    static async createAccount(currency: Currency) {
        try {
            const response = await fetch(`${magicConsts.createAccountEndpoint}?currency=${currency}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': getAccessToken(),
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

    static async closeAccount(accountId: string) {
        try {
            const params = new URLSearchParams({accountId});//api/Account/Close?accountId=c2fa4712-2213-402d-ac31-b30162fb0f08
            const response = await fetch(`${magicConsts.closeAccountEndpoint}?${params}`, {
                method: 'DELETE',
                headers: {'Authorization': getAccessToken()}
            });

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


