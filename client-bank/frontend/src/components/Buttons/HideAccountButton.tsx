import React from 'react';
import {isAuthenticated} from "../../api/auth";
import {AccountId, userSettings} from "../../api/userSettings";



export const HideAccountButton: React.FC<AccountId> = ({accountId,IsHidden=false}) => {
    const [isHidden, setIsHidden] = React.useState(IsHidden);

    const changeAccountHidden = () => {
        try {
            if (!isAuthenticated())
                throw new Error('not authenticated')

            userSettings.ChangeAccountVisibility(accountId)

            setIsHidden(!isHidden);
        } catch (e) {
            console.log(e)
        }


    };


    return (
        <button onClick={changeAccountHidden}>
            {isHidden ? 'Account Hidden' : 'Hide Account'}
        </button>
    );
};
