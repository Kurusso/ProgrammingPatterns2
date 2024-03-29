import {useAccounts} from "../contexts/AccountContext";
import {useEffect} from "react";
import {getAccounts} from "../api/account";
import {mapAccountDataToElementProps} from "./Accounts";

interface AccountSelectProps {
    selectedAccount: string | null,
    setSelectedAccount: React.Dispatch<React.SetStateAction<string | null>>
}

export const AccountSelect: React.FC<AccountSelectProps> = ({selectedAccount, setSelectedAccount}) => {
    const {accountElements, setAccountElements} = useAccounts();

    useEffect(() => {

        const fetchData = async () => {
            try {
                const storedToken = localStorage.getItem('token');
                if (!storedToken) {
                    throw new Error('No token found');
                }

                const parsedToken = JSON.parse(storedToken).token;
                if (!parsedToken) {
                    throw new Error('Invalid token');
                }

                const accounts = await getAccounts(parsedToken);
                let AccountsElementsData = mapAccountDataToElementProps(accounts);
                setAccountElements(AccountsElementsData);
            } catch (e) {

            }
        }
        fetchData()
    }, []);


    return (<div>
            <h4>Account to pay</h4>
            <select className={"account-select"} value={selectedAccount || ''} onChange={(e) => setSelectedAccount(e.target.value)}>
                <option value="">Select...</option>
                {accountElements.map((account) => (
                    <option key={account.AccountId} value={account.AccountId}>{account.AccountId}</option>
                ))}
            </select>
        </div>
    );
};