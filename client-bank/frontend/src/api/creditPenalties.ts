import {magicConsts} from "./magicConst";
import {getAccessToken} from "./auth";
import {Currency, Money} from "./account";

export interface Penalty {
    isPaidOff: boolean;
    creditId: string;
    amount: Money; // You'll need to define a Money interface or type
}


export class creditPenalties {
    static async GetUserPenalties() {
        const response = await fetch(magicConsts.getUserPenaltiesEndpoint, {
            headers: {
                'Authorization': getAccessToken()
            },
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        let data:Penalty[]=await response.json();
        return data;
    }

    static async GetCreditPenalties() {
        const response = await fetch(magicConsts.getCreditPenaltiesEndpoint, {
            headers: {
                'Authorization': getAccessToken()
            },
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        let data:Penalty[]=await response.json();
        return data;
    }

    static async RepayPenalty(id: string, moneyAmmount: number, currency: Currency, accountId: string | null) {
        let requestUrl = `${magicConsts.repayPenaltyEndpoint}?id=${id}&moneyAmmount=${moneyAmmount}&currency=${Currency}&accountId=${accountId}`

        const response = await fetch(requestUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': getAccessToken()
            }
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    }
}
