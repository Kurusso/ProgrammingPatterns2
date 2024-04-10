import {AccountData, AccountService} from "../api/account";
import {useEffect, useState} from "react";
import * as signalR from "@microsoft/signalr";
import {magicConsts} from "../api/magicConst";
import {getAccessToken, isAuthenticated} from "../api/auth";

export const useAccountData = (accountId: string | undefined) => {
    const [accountData, setAccountData] = useState<AccountData>();
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

    const updateAccount = async () => {
        try {

            if (!isAuthenticated()) {
                throw new Error('Invalid token');
            }

            if (accountId != null) {
                const account = await AccountService.getAccount(accountId);
                setAccountData(account);
            }

        } catch (error) {
            console.error('Error fetching account:', error);
        }
    };

    useEffect(() => {
        updateAccount();
    }, [accountId]);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${magicConsts.AccountInfoHub}`, {
                accessTokenFactory: () => {
                    return getAccessToken();
                }
            })
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => console.log('Connection started'))
                .catch(err => console.log('Error while starting connection: ' + err))

            connection.on("ReceiveAccount", (message) => {
                console.log('Received message:', message);
                setAccountData(message);
            });
        }

        return () => {
            if (connection) {
                connection.stop();
            }
        }
    }, [connection]);


    return {accountData, setAccountData, updateAccount};
};
