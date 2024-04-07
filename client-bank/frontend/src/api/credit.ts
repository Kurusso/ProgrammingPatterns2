import {
    magicConsts
} from "./magicConst";
import {Currency, Money} from "./account";
import {getAccessToken} from "./auth";

export interface CreditRate {
    id: string;
    name: string;
    monthPercent: number;
}

export interface CreditData {
    creditRate: CreditRate;
    fullMoneyAmount: Money;
    id: string;
    monthPayAmount: Money;
    payingAccountId: string;
    remainingDebt: Money;
    unpaidDebt: Money;
    userId: string;
}

export class CreditService {
    static async getCredits() {
        try {
            const response = await fetch(`${magicConsts.getCreditsEndpoint}`, {
                headers: {'Authorization': getAccessToken(),}
            })
            let data: CreditData[] = await response.json();
            console.log(data);
            return data
        } catch (error) {
            throw error;
        }
    }

    static async getCredit( creditId: string) {
        try {
            const response = await fetch(`${magicConsts.getCreditEndpoint}?id=${creditId}`, {
                headers: {'Authorization': getAccessToken()}
            })
            let data: CreditData = await response.json();
            console.log(data);
            return data
        } catch (error) {
            throw error;
        }
    }

    static async takeCredit(creditRateId: string, accountId: string, currency: Currency, moneyAmount: number, monthPay: number) {
        try {
            const response = await fetch(magicConsts.takeCreditEndpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': getAccessToken()
                },
                body: JSON.stringify({
                    creditRateId,
                    accountId,
                    currency: Number(currency),
                    moneyAmount,
                    monthPay
                })
            });

            const responseData = await response.text();
            const isJsonResponse = responseData && responseData.startsWith('{');

            if (!response.ok) {
                const errorData = isJsonResponse ? JSON.parse(responseData) : responseData;
                throw new Error(`HTTP error! status: ${response.status}, message: ${errorData.message || errorData}`);
            }

            return isJsonResponse ? JSON.parse(responseData) : responseData;
        } catch (error) {
            console.error('An error occurred while taking credit:', error);
            throw error;
        }
    }

    static async repayCredit(creditId: string, moneyAmmount: number, currency: Currency, accountId: string) {
        try {
            console.log()

            const response = await fetch(`${magicConsts.repayCreditEndpoint}?id=${creditId}&moneyAmmount=${moneyAmmount}&currency=${currency}&accountId=${accountId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': getAccessToken()
                },
            })

        } catch (error) {
            throw error;
        }
    }

    static async getCreditRates() {
        try {
            const response = await fetch(magicConsts.getCreditRatesEndpoint)
            let data: CreditRate[] = await response.json();
            console.log(data);
            return data
        } catch (error) {
            throw error;
        }
    }

}



