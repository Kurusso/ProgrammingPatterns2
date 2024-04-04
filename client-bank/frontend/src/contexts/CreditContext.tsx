import React, {createContext, useContext, useState} from "react";
import {CreditItemProps} from "../components/Credits/CreditItem";

type CreditContextType={
    creditItems:CreditItemProps[];
    setCreditItems:React.Dispatch<React.SetStateAction<CreditItemProps[]>>
}


const CreditContext = createContext<CreditContextType | undefined>(undefined);

export const useCredits=()=>{
    const context = useContext(CreditContext);
    if (!context) {
        throw new Error('useCredits must be used within an CreditProvider');
    }
    return context;
}



interface CreditProviderProps{
    children:React.ReactNode
}
export const CreditProvider:React.FC<CreditProviderProps> = ({children}) => {
const [creditItems,setCreditItems]=useState<CreditItemProps[]>([])
    return (
        <CreditContext.Provider value={{creditItems,setCreditItems}}>
            {children}
        </CreditContext.Provider>
    );
};

