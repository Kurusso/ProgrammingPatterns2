import React, { createContext, useContext, useState } from 'react';
import {AccountItemProps} from "../components/AccountItem";

type AccountContextType = {
    accountElements: AccountItemProps[];
    setAccountElements: React.Dispatch<React.SetStateAction<AccountItemProps[]>>;
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
    const [accountElements, setAccountElements] = useState<AccountItemProps[]>([]);

    return (
        <AccountContext.Provider value={{ accountElements, setAccountElements }}>
            {children}
        </AccountContext.Provider>
    );
};
