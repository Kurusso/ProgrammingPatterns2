import {AccountData} from "../api/account";
import React, {createContext, useContext, useState} from "react";

type AccountsContextType = {
    accountElement: AccountData|undefined;
    setAccountElement: React.Dispatch<React.SetStateAction<AccountData|undefined>>;
};

const AccountContext = createContext<AccountsContextType | undefined>(undefined);

export const useAccount = () => {
    const context = useContext(AccountContext);
    if (!context) {
        throw new Error('useAccounts must be used within an AccountProvider');
    }
    return context;
};

interface AccountProviderProps{
    children:React.ReactNode
}
export const AccountProvider: React.FC<AccountProviderProps> = ({ children }) => {
    const [accountElement, setAccountElement] = useState<AccountData|undefined>(undefined);

    return (
        <AccountContext.Provider value={{ accountElement, setAccountElement }}>
            {children}
        </AccountContext.Provider>
    );
};