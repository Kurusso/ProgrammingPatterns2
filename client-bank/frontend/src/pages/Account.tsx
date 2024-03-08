import React, {useEffect, useState} from "react";
import {useParams} from 'react-router-dom';
import {AccountData, Currency} from "../api/account";

export const Account = () => {
    const {accountId} = useParams<{ accountId: string }>();


    useEffect(()=>{console.log("all good")},[]);




    return (
        <div>
            <h2>Account</h2>
            <div>{accountId}</div>

        </div>
    );
};