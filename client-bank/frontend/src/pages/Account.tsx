import React from "react";
import {useParams} from 'react-router-dom';
import {Currency} from "../api/account";
import {LogoutButton} from "../components/Buttons/LogoutButton";
import {TransactionHistory} from "../components/Accounts/Account/TransactionHistory";
import {HomeButton} from "../components/Buttons/HomeButton";
import {UsernameDisplay} from "../components/UsernameDisplay";
import {MoneyTransaction, TransactionType} from "../components/Accounts/Account/MoneyTransaction";
import {useAccountData} from "../other/UseAccountData";

export const Account = () => {
    const { accountId } = useParams<{ accountId: string }>();
    const { accountData, updateAccount } = useAccountData(accountId);


    return (
        <div>
            <h2>Account</h2>
            <UsernameDisplay/>
            <LogoutButton/>
            <HomeButton/>
            <div className={"transactions"}>
                <h3>Account Operations</h3>
                <div>
                    <MoneyTransaction transactionType={TransactionType.Deposit} setAccount={updateAccount}/>
                </div>
                <div>
                    <MoneyTransaction transactionType={TransactionType.Withdrawal} setAccount={updateAccount}/>
                </div>
            </div>
            <div className={"account-information"}>
                <h3>Account Information</h3>
                <div>{accountId}</div>
                <div>{accountData?.money.amount} {Currency[accountData?.money.currency!]}</div>
                <div>
                    <TransactionHistory transactions={accountData?.operationsHistory!}/>
                </div>
            </div>


        </div>
    );
}