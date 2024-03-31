import {ChangeEvent, useState} from "react";
import {AccountService, Currency } from "../../api/account";
import {useAccounts} from "../../contexts/AccountsContext";
import {mapAccountDataToElementProps} from "./Accounts";
import {CurrencySelect} from "../Selects/CurrencySelect";

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
    const {setAccountElements} = useAccounts();

    const handleChange =
        (event: ChangeEvent<HTMLSelectElement>) => {
            setSelectedCurrency(event.target.value as unknown as Currency);
        };

    const storedToken = localStorage.getItem('token');
    const parsedToken = storedToken ? JSON.parse(storedToken).token : null;

    const handleCreate = async () => {
        if (!parsedToken || !selectedCurrency) {
            return;
        }

        try {
            await AccountService.createAccount(parsedToken, selectedCurrency!);
            const accounts = await AccountService.getAccounts(parsedToken);
            let AccountsElementsData = mapAccountDataToElementProps(accounts);
            setAccountElements(AccountsElementsData);
        } catch (error) {
            console.error('An error occurred while creating the account:', error);
        }
    };

    return (
        <div className={"new-account-form"}>
            <h3>Create Account</h3>
            <button className={"create-account-btn"} type={"button"} onClick={handleCreate}>Create Account</button>
            <CurrencySelect  selectedCurrency={selectedCurrency}
                            setSelectedCurrency={setSelectedCurrency}/>
        </div>
    );
};