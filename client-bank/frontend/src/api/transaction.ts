import {Currency} from "./account";
import {TransactionType} from "../components/MoneyTransaction";
import {depositEndpoint, withdrawEndpoint} from "./magicConst";

export async function performTransaction(accountId: string, money: number, currency: Currency, transactionType: TransactionType,userId:string) {
    try {
        //https://localhost:7143/api/Operations/Deposit?accountId=50846610-5e4c-4159-8059-d38cd93d28e1&userId=50846610-5e4c-4159-8059-d38cd93d28e1&money=100&currency=0
        let request_url = `${(transactionType === TransactionType.Deposit ? depositEndpoint : withdrawEndpoint)}?accountId=${accountId}&userId=${userId}&money=${money}&currency=${currency}`;
        console.log(request_url)
        const response = await fetch(request_url, {
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
        console.error('An error occurred while performing transaction')
        throw error;
    }


}