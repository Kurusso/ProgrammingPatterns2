import {Currency} from "./account";
import {TransactionType} from "../components/Accounts/Account/MoneyTransaction";
import {magicConsts} from "./magicConst";
import {getAccessToken} from "./auth";


export class TransactionService {
    static async performTransaction(accountId: string, money: number, currency: Currency, transactionType: TransactionType) {
        try {
            let request_url = `${(transactionType === TransactionType.Deposit ? magicConsts.depositEndpoint : magicConsts.withdrawEndpoint)}?accountId=${accountId}&money=${money}&currency=${currency}`;
            console.log(request_url)
            const response = await fetch(request_url, {
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
            console.error('An error occurred while performing transaction')
            throw error;
        }


    }

    static async TransferMoney(){}
}