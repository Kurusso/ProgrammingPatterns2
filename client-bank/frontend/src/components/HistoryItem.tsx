import {Currency, OperationsHistory, OperationType} from "../api/account";

export const HistoryItem: React.FC<OperationsHistory> = ({
                                                            id,
                                                            accountId,
                                                            operationType,
                                                            moneyAmmountInAccountCurrency,
                                                            moneyAmmount
                                                        }) => {
    return (
        <div>
            <div>OperationType: {OperationType[operationType]}</div>
            <div>{moneyAmmountInAccountCurrency}</div>
            <div>{moneyAmmount.amount} {Currency[moneyAmmount.currency]}</div>
        </div>
    );
};