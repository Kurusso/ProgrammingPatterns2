import React, {useEffect, useState} from "react";
import {useParams} from 'react-router-dom';
import {AccountData, Currency, getAccount} from "../api/account";
import {LogoutButton} from "../components/LogoutButton";
import {OperationsHistory} from "../components/OperationsHistory";
import {HomeButton} from "../components/HomeButton";


export const Account = () => {
    const {accountId} = useParams<{ accountId: string }>();
    const [accountData, setAccountData] = useState<AccountData>()

    useEffect(() => {
        console.log("all good")
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
            <LogoutButton/>
            <HomeButton/>
            <div>{accountId}</div>
            <div>{accountData?.money.amount} {Currency[accountData?.money.currency!]}</div>
            <div>
                <OperationsHistory operations={accountData?.operationsHistory!}/>
            </div>


        </div>
    );
};