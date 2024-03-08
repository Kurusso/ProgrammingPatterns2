import {ChangeEvent, useEffect, useState} from "react";
import {createAccount, Currency, getAccounts} from "../api/account";
import {useAccounts} from "../contexts/AccountContext";
import {mapAccountDataToElementProps} from "./Accounts";

type OptionType = {
    value: string;
    label: string;
};

const options: OptionType[] = [
    {value: Currency.Ruble.toString(), label: `${Currency[Currency.Ruble]}`},
    {value: Currency.Dollar.toString(), label: `${Currency[Currency.Dollar]}`},
    {value: Currency.Euro.toString(), label: `${Currency[Currency.Euro]}`},
]

export const CreateAccount = () => {
    const [selectedCurrency, setSelectedCurrency] = useState<Currency | null>(null);
    const { setAccountElements } = useAccounts();
    useEffect(() => {
        setSelectedCurrency(Currency.Ruble);
    }, []);
    const handleChange =
        (event: ChangeEvent<HTMLSelectElement>) => {
        setSelectedCurrency(event.target.value as unknown as Currency);
    };

    const storedToken = localStorage.getItem('token');
    const parsedToken = (storedToken ? JSON.parse(storedToken) : null).token;

    const handleCreate = async () => {
        if (!parsedToken) {
            // Inform the user that they need to log in
            return;
        }

        try {
            await createAccount(parsedToken, selectedCurrency!);
            const accounts = await getAccounts(parsedToken);
            let AccountsElementsData = mapAccountDataToElementProps(accounts);
            setAccountElements(AccountsElementsData);
        } catch (error) {
            console.error('An error occurred while creating the account:', error);
        }
    };

    return (
        <div className={"new-account-from"}>
            <h3>Create Account</h3>
            <button type={"button"} onClick={handleCreate}>Create Account</button>
            <select value={selectedCurrency || ''} onChange={handleChange}>
                <option value="">Select...</option>
                {options.map((option) => (
                    <option key={option.value} value={option.value}>
                        {option.label}
                    </option>
                ))}
            </select>
        </div>
    );
};