import {useAccounts} from "../../contexts/AccountsContext";
import {useEffect} from "react";
import {AccountService} from "../../api/account";
import {mapAccountDataToElementProps} from "../Accounts/Accounts";
import {isAuthenticated} from "../../api/auth";

interface AccountSelectProps {
    selectedAccount: string | null,
    setSelectedAccount: React.Dispatch<React.SetStateAction<string | null>>
}

export const AccountSelect: React.FC<AccountSelectProps> = ({selectedAccount, setSelectedAccount}) => {
    const {accountElements, setAccountElements} = useAccounts();

    useEffect(() => {

        const fetchData = async () => {
            try {
                if (!isAuthenticated()) {
                    throw new Error('Invalid token');
                }

                const accounts = await AccountService.getAccounts();//getAccounts
                let AccountsElementsData = mapAccountDataToElementProps(accounts);
                setAccountElements(AccountsElementsData);
            } catch (e) {

            }
        }
        fetchData()
    }, []);


    return (<div>
            <h5>Account to pay</h5>
            <select className={"account-select"} value={selectedAccount || ''}
                    onChange={(e) => setSelectedAccount(e.target.value)}>
                <option value="">Select...</option>
                {accountElements.map((account) => (
                    <option key={account.AccountId} value={account.AccountId}>{account.AccountId}</option>
                ))}
            </select>
        </div>
    );
};