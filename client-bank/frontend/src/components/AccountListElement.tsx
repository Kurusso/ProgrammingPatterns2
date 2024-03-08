import {Currency} from "../api/account";
import React from 'react';
import {useNavigate} from "react-router-dom";

export interface AccountListElementProps{
    AccountId:string,
    Amount:number,
    CurrencyValue:Currency
}
export const AccountListElement:React.FC<AccountListElementProps> = ({ AccountId, Amount, CurrencyValue }) => {

    const navigate = useNavigate();
    const HandleClick=()=>{
        navigate("/account/"+AccountId);
    }

    return (
        <div>
            <a onClick={HandleClick}>Account: {AccountId}</a>
            <div>Amount: {Amount}</div>
            <div>Currency: {Currency[CurrencyValue]}</div>
        </div>
    );
};