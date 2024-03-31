import {OperationsHistory} from "../../../api/account";
import React from "react";
import {HistoryItem} from "./HistoryItem";

type OperationsHistoryProps = {
    transactions: OperationsHistory[]
}
export const TransactionHistory: React.FC<OperationsHistoryProps> = ({transactions}) => {

    return (
        <div className={"transaction history"}>
            <h3>Transaction History</h3>
            <div className={"transaction-items"}>
                {
                    transactions && transactions.map(item => (
                        <HistoryItem
                            key={item.id}
                            accountId={item.accountId}
                            id={item.id}
                            operationType={item.operationType}
                            moneyAmmountInAccountCurrency={item.moneyAmmountInAccountCurrency}
                            moneyAmmount={item.moneyAmmount}

                        />
                    ))
                }
            </div>
        </div>
    );
};