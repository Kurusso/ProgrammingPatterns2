import {useEffect, useState} from "react";
import {AccountData, getAccounts} from "../api/account";
import {AccountListElement, AccountListElementProps} from "./AccountListElement";

export const Accounts = () => {

    const [accountElements,setAccountElements] = useState<AccountListElementProps[]>([]);

    useEffect(() => {
        console.log('Component did mount');
        const fetchData =async () => {
            try {
                const storedToken = localStorage.getItem('token');
                const parsedToken = (storedToken ? JSON.parse(storedToken) : null).token;

                const accounts = await getAccounts(parsedToken);
                let AccountsElementsData=mapAccountDataToElementProps(accounts);
                console.log(AccountsElementsData);
                setAccountElements(AccountsElementsData);


                console.log('Fetched accounts:', accounts);
            } catch (error) {
                console.error('Error fetching accounts:', error);
            }
        }

        fetchData()




        }, []);


    return (
        <div><h5>Accounts</h5>
            <div>{
                accountElements.map(item=>(
                    <AccountListElement
                        key={item.AccountId} // Set a unique key
                        AccountId={item.AccountId}
                        Amount={item.Amount}
                        CurrencyValue={item.CurrencyValue}
                    />
                ))
            }</div>
        </div>
    );
};

function mapAccountDataToElementProps(accountDataArray: AccountData[]): AccountListElementProps[]{
    return accountDataArray.map(accountData => {
        const { id, money } = accountData;
        const { amount, currency } = money;

        return {
            AccountId: id,
            Amount: amount,
            CurrencyValue: currency,
        };
    });


}