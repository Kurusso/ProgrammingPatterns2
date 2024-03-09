import {closeAccountEndpoint, createAccountEndpoint, getAccountEndpoint, getAccountsEndpoint} from "./magicConst";
import {Simulate} from "react-dom/test-utils";
import error = Simulate.error;

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

export async function getAccounts(token: string) {
    try {
        console.log(token);
        const response = await fetch(getAccountsEndpoint + token)
        let data: AccountData[] = await response.json()
        console.log(data)
        return data
    } catch (error) {
        throw error;
    }
}


export async function getAccount(accountId: string) {
    try {
        const response = await fetch(`${getAccountEndpoint}${accountId}`);

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


export async function createAccount(userId: string, currency: Currency) {
    try {
        const response = await fetch(`${createAccountEndpoint}?userId=${userId}&currency=${currency}`, {
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

export async function closeAccount(userId: string, accountId: string) {
    try {
        const params = new URLSearchParams({userId, accountId});
        const response = await fetch(`${closeAccountEndpoint}?${params}`, {method: 'DELETE'});

        if (!response.ok) {
            const errorData = await response.text();
            throw new Error(`HTTP error! status: ${response.status}, message: ${errorData}`);
        }
    } catch (error) {
        console.error('An error occurred while deleting the account');
        throw error;
    }
}