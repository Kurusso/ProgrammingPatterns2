import {CurrencyInput} from "../../Input/CurrencyInput";
import {CurrencySelect} from "../../Selects/CurrencySelect";
import React, {useState} from "react";
import {Currency} from "../../../api/account";
import {useParams} from "react-router-dom";
import {TransactionService} from "../../../api/transaction";
import {isAuthenticated} from "../../../api/auth";

interface MoneyOperationProps {
    transactionType:TransactionType
    setAccount: ()=>Promise<void>//React.Dispatch<React.SetStateAction<AccountData | undefined>>
}

export enum TransactionType {
    Withdrawal,
    Deposit
}

export const MoneyTransaction: React.FC<MoneyOperationProps> = ({transactionType,setAccount}) => {
    const [selectedCurrency, setSelectedCurrency] = useState<Currency | null>(null);
    const [amount, setAmount] = useState<number>(0);
    const { accountId } = useParams<{ accountId: string }>();


    let operationName = TransactionType[transactionType];

    console.log(accountId)
    const HandleOperation = async () => {

        if (!isAuthenticated()) {
            throw new Error('Invalid token');
        }

        if(!selectedCurrency||amount<=0||!accountId)
            return;
        await TransactionService.performTransaction(accountId,amount,selectedCurrency,transactionType)

        await setAccount();

    };


    return (
        <div className={"transaction"}>
            <h4>{operationName}</h4>
            <CurrencyInput amount={amount} setAmount={setAmount}/>
            <CurrencySelect selectedCurrency={selectedCurrency}
                            setSelectedCurrency={setSelectedCurrency}/>
            <button className={"transaction-btn"} type={"button"} onClick={HandleOperation}> {operationName}</button>
        </div>
    );
};