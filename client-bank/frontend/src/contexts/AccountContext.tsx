import React, { createContext, useContext, useState } from 'react';
import {AccountListElementProps} from "../components/AccountItem";

type AccountContextType = {
    accountElements: AccountListElementProps[];
    setAccountElements: React.Dispatch<React.SetStateAction<AccountListElementProps[]>>;
};

const AccountContext = createContext<AccountContextType | undefined>(undefined);

export const useAccounts = () => {
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
    const [accountElements, setAccountElements] = useState<AccountListElementProps[]>([]);
    return (
        <AccountContext.Provider value={{ accountElements, setAccountElements }}>
            {children}
        </AccountContext.Provider>
    );
};
