import {AccountData, AccountService} from "../api/account";
import {useEffect, useState} from "react";
import * as signalR from "@microsoft/signalr";
import {magicConsts} from "../api/magicConst";

export const useAccountData = (accountId: string | undefined) => {
    const [accountData, setAccountData] = useState<AccountData>();
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

    const updateAccount = async () => {
        try {
            const storedToken = localStorage.getItem('token');
            if (!storedToken) {
                throw new Error('No token found');
            }

            const parsedToken = JSON.parse(storedToken).token;
            if (!parsedToken) {
                throw new Error('Invalid token');
            }

            if (accountId != null) {
                const account = await AccountService.getAccount(accountId, parsedToken);
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
                    const storedToken = localStorage.getItem('token');
                    if (!storedToken) {
                        throw new Error('No token found');
                    }

                    const parsedToken = JSON.parse(storedToken).token;
                    if (!parsedToken) {
                        throw new Error('Invalid token');
                    }

                    return parsedToken;
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
