import React, { createContext, useContext, useState } from 'react';
import {AccountItemProps} from "../components/Accounts/AccountItem";

type AccountsContextType = {
    accountElements: AccountItemProps[];
    setAccountElements: React.Dispatch<React.SetStateAction<AccountItemProps[]>>;
};

const AccountsContext = createContext<AccountsContextType | undefined>(undefined);

export const useAccounts = () => {
    const context = useContext(AccountsContext);
    if (!context) {
        throw new Error('useAccounts must be used within an AccountProvider');
    }
    return context;
};

interface AccountsProviderProps{
    children:React.ReactNode
}
export const AccountsProvider: React.FC<AccountsProviderProps> = ({ children }) => {
    const [accountElements, setAccountElements] = useState<AccountItemProps[]>([]);

    return (
        <AccountsContext.Provider value={{ accountElements, setAccountElements }}>
            {children}
        </AccountsContext.Provider>
    );
};
