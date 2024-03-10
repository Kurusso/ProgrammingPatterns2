import {Currency} from "./account";
import {TransactionType} from "../components/MoneyTransaction";
import {depositEndpoint, withdrawEndpoint} from "./magicConst";

export async function performTransaction(accountId: string, money: number, currency: Currency, transactionType: TransactionType) {
    try {
        let request_url = `${(transactionType === TransactionType.Deposit ? depositEndpoint : withdrawEndpoint)}?accountId=${accountId}&money=${money}&currency=${currency}`;
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