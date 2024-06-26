import {useEffect} from "react";
import {AccountData, AccountService} from "../../api/account";
import {AccountItem, AccountItemProps} from "./AccountItem";
import {useAccounts} from "../../contexts/AccountsContext";
import {isAuthenticated} from "../../api/auth";
import {userSettings} from "../../api/userSettings";

export const Accounts = () => {

    const {accountElements, setAccountElements} = useAccounts();

    useEffect(() => {
        const fetchData = async () => {
            try {
                if (!isAuthenticated())
                    return;

                const accounts = await AccountService.getAccounts();
                if (!Array.isArray(accounts)) {
                    throw new Error('Expected accounts to be an array');
                }
                let AccountsElementsData = mapAccountDataToElementProps(accounts);
                setAccountElements(AccountsElementsData);
                console.log('Fetched accounts:', accounts);
            } catch (error) {
                console.error('Error fetching accounts:', error);
            }
        };


        fetchData()

    }, []);


    return (
        <div className={"accounts"}>
            <h3>Accounts</h3>
            <div className={"accounts-items"}>{
                accountElements.map(item => (
                    <AccountItem
                        key={item.AccountId} // Set a unique key
                        AccountId={item.AccountId}
                        Amount={item.Amount}
                        CurrencyValue={item.CurrencyValue}
                        IsHidden={item.IsHidden}
                    />
                ))
            }</div>
        </div>
    );
};

export function mapAccountDataToElementProps(accountDataArray: AccountData[]): AccountItemProps[] {
    if (!Array.isArray(accountDataArray)) {
        console.error('Error: accountDataArray is not an array');
        return [];
    }

    return accountDataArray.map(accountData => {
        const {id, money,isHidden} = accountData;
        const {amount, currency} = money;

        return {
            AccountId: id,
            Amount: amount,
            CurrencyValue: currency,
            IsHidden:isHidden
        };
    });
}