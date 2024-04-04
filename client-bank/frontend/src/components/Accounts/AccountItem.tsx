import {Currency, AccountService} from "../../api/account";
import React, {useCallback} from 'react';
import {useNavigate} from "react-router-dom";

export interface AccountItemProps {
    AccountId:string,
    Amount:number,
    CurrencyValue:Currency
}
export const AccountItem:React.FC<AccountItemProps> = ({ AccountId, Amount, CurrencyValue }) => {
    const navigate = useNavigate();
    const [closed, setClosed] = React.useState(false);

    const handleClickAccountDetails = useCallback(() => {
        navigate(`/account/${AccountId}`);
    }, [navigate, AccountId]);

    const handleClickClose = useCallback(async () => {
        const storedToken = localStorage.getItem('token');
        if (!storedToken) {
            console.error('Token not found');
            return;
        }
        const parsedToken = JSON.parse(storedToken).token;
        try {
            await AccountService.closeAccount(parsedToken, AccountId);
            setClosed(true);
        } catch (error) {
            console.error('Error closing account:', error);
        }
    }, [AccountId]);

    if (closed) {
        return null;
    }

    return (
        <div>
            <button className={"account-btn"} onClick={handleClickAccountDetails}>Account: {AccountId}</button>
            <div>Amount: {Amount}</div>
            <div>Currency: {Currency[CurrencyValue]}</div>
            <button className={"close-account-btn"} onClick={handleClickClose}>Close Account</button>
        </div>
    );
};





