import {Currency, closeAccount} from "../api/account";
import React, {useCallback} from 'react';
import {useNavigate} from "react-router-dom";

export interface AccountListElementProps{
    AccountId:string,
    Amount:number,
    CurrencyValue:Currency
}
export const AccountItem:React.FC<AccountListElementProps> = ({ AccountId, Amount, CurrencyValue }) => {
    const navigate = useNavigate();
    const [closed, setClosed] = React.useState(false); // Add this line

    const handleClickAccountDetails = useCallback(() => {
        navigate(`/account/${AccountId}`);
    }, [navigate, AccountId]);

    const handleClickClose = useCallback(async () => { // Handle async operation
        const storedToken = localStorage.getItem('token');
        if (!storedToken) {
            console.error('Token not found');
            return;
        }
        const parsedToken = JSON.parse(storedToken).token;
        try {
            await closeAccount(parsedToken, AccountId); // Handle promise
            setClosed(true); // Add this line
        } catch (error) {
            console.error('Error closing account:', error);
        }
    }, [AccountId]);

    if (closed) { // Add this block
        return null;
    }

    return (
        <div>
            <button onClick={handleClickAccountDetails}>Account: {AccountId}</button>
            <div>Amount: {Amount}</div>
            <div>Currency: {Currency[CurrencyValue]}</div>
            <button onClick={handleClickClose}>Close Account</button>
        </div>
    );
};





//CloseAccount

//CreateAccount


//Withdraw- Account
//Deposit - Account