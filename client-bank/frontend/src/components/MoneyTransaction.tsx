import {CurrencyInput} from "./CurrencyInput";
import {CurrencySelect} from "./CurrencySelect";
import React, {useState} from "react";
import {Currency} from "../api/account";
import {useParams} from "react-router-dom";
import {performTransaction} from "../api/transaction";

interface MoneyOperationProps {
    transactionType:TransactionType

}

export enum TransactionType {
    Withdrawal,
    Deposit
}

export const MoneyTransaction: React.FC<MoneyOperationProps> = ({transactionType}) => {
    const [selectedCurrency, setSelectedCurrency] = useState<Currency | null>(null);
    const [amount, setAmount] = useState<number>(0);
    const {accountId} = useParams<{ accountId: string }>();

    var operationName=TransactionType[transactionType];

    console.log(accountId)
    const HandleOperation = async () => {
        if(!selectedCurrency||amount<=0||!accountId)
            return;
        await performTransaction(accountId,amount,selectedCurrency,transactionType)
    };


    return (
        <div>
            <h4>{operationName}</h4>
            <CurrencyInput amount={amount} setAmount={setAmount}/>
            <CurrencySelect selectedCurrency={selectedCurrency}
                            setSelectedCurrency={setSelectedCurrency}/>
            <button type={"button"} onClick={HandleOperation}> {operationName}</button>
        </div>
    );
};