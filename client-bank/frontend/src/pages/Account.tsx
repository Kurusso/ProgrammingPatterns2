import React, {ChangeEvent, useEffect, useState} from "react";
import {useParams} from 'react-router-dom';
import {AccountData, Currency, getAccount} from "../api/account";
import {LogoutButton} from "../components/LogoutButton";
import {TransactionHistory} from "../components/TransactionHistory";
import {HomeButton} from "../components/HomeButton";
import {UsernameDisplay} from "../components/UserameDisplay";
import {CurrencyInput} from "../components/CurrencyInput";
import {CurrencySelect} from "../components/CurrencySelect";
import {MoneyTransaction, TransactionType} from "../components/MoneyTransaction";


export const Account = () => {
    const {accountId} = useParams<{ accountId: string }>();
    const [accountData, setAccountData] = useState<AccountData>()

    useEffect(() => {
        const fetchData = async () => {
            try {
                const account = await getAccount(accountId!);
                console.log(account)
                setAccountData(account)
                console.log('Account Fetched :', account);
            } catch (error) {
                console.error('Error fetching account:', error);
            }
        }

        fetchData();
    }, []);

    return (
        <div>
            <h2>Account</h2>
            <UsernameDisplay/>
            <LogoutButton/>
            <HomeButton/>
            <div>
                <h3>Account Operations</h3>
                <div>
                    <MoneyTransaction transactionType={TransactionType.Deposit}/>
                </div>
                <div>
                    <MoneyTransaction transactionType={TransactionType.Withdrawal}/>
                </div>
            </div>
            <div>
                <h3>Account Information</h3>
                <div>{accountId}</div>
                <div>{accountData?.money.amount} {Currency[accountData?.money.currency!]}</div>
                <div>
                    <TransactionHistory transactions={accountData?.operationsHistory!}/>
                </div>
            </div>


        </div>
    );
};