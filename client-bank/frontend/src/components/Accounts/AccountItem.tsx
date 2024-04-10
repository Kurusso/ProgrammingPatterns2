import {Currency, AccountService} from "../../api/account";
import React, {useCallback} from 'react';
import {useNavigate} from "react-router-dom";
import {isAuthenticated} from "../../api/auth";
import {HideAccountButton} from "../Buttons/HideAccountButton";

export interface AccountItemProps {
    AccountId: string,
    Amount: number,
    CurrencyValue: Currency
    IsHidden:boolean
}

export const AccountItem: React.FC<AccountItemProps> = ({AccountId, Amount, CurrencyValue,IsHidden}) => {
    const navigate = useNavigate();
    const [closed, setClosed] = React.useState(false);

    const handleClickAccountDetails = useCallback(() => {
        navigate(`/account/${AccountId}`);
    }, [navigate, AccountId]);

    const handleClickClose = useCallback(async () => {


        if (!isAuthenticated())
            return;

        try {
            await AccountService.closeAccount(AccountId);
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
            <HideAccountButton accountId={AccountId} IsHidden={IsHidden}/>
            <div>Amount: {Amount}</div>
            <div>Currency: {Currency[CurrencyValue]}</div>
            <button className={"close-account-btn"} onClick={handleClickClose}>Close Account</button>
        </div>
    );
};





