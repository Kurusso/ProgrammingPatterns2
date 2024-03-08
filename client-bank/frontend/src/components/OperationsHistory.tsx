import {OperationHistory} from "../api/account";
import React from "react";
import {HistoryItem} from "./HistoryItem";

type OperationsHistoryProps = {
    operations: OperationHistory[]
}
export const OperationsHistory: React.FC<OperationsHistoryProps> = ({operations}) => {

    return (
        <div>
            <h3>Operations History</h3>
            <div>
                {
                    operations && operations.map(item => (
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