import {
    createAccountEndpoint,
    getAccountsEndpoint, getCreditEndpoint,
    getCreditRatesEndpoint,
    getCreditsEndpoint,
    takeCreditEndpoint
} from "./magicConst";
import {Currency, Money} from "./account";
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



export async function getCredits(token:string){
    try{
        const response=await fetch(`${getCreditsEndpoint}?userId=${token}`)
        let data:CreditData[]=await response.json();
        console.log(data);
        return data
    }
    catch (error){
        throw error;
    }
}

export async function getCredit(token:string,creditId:string){
    try{
        console.log("getting credit")
        const response=await fetch(`${getCreditEndpoint}?id=${creditId}&userId=${token}`)
        let data:CreditData=await response.json();
        console.log(data);
        return data
    }
    catch (error){
        throw error;
    }
}


export async function takeCredit(creditRateId: string, userId: string, accountId: string, currency: Currency, moneyAmount: number, monthPay: number) {
    try {
        const response = await fetch(takeCreditEndpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                creditRateId,
                userId,
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


//take credit

//pay credit
export async function getCreditRates(){
    try{
        const response=await fetch(getCreditRatesEndpoint)
        let data:CreditRate[]=await response.json();
        console.log(data);
        return data
    }
    catch (error){
        throw error;
    }
}
