import {getAccountsEndpoint, getCreditsEndpoint} from "./magicConst";
import {Money} from "./account";
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
//take credit

//pay credit