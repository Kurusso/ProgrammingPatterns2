
import {ChangeEvent, useState, useEffect} from "react";
import {Currency} from "../../api/account";

type OptionType = {
    value: string;
    label: string;
};

const options: OptionType[] = [
    {value: Currency.Ruble.toString(), label: `${Currency[Currency.Ruble]}`},
    {value: Currency.Dollar.toString(), label: `${Currency[Currency.Dollar]}`},
    {value: Currency.Euro.toString(), label: `${Currency[Currency.Euro]}`},
]

type Props = {
    selectedCurrency:Currency | null,
    setSelectedCurrency:React.Dispatch<React.SetStateAction<Currency | null>>
};

export const CurrencySelect = ({selectedCurrency,setSelectedCurrency}: Props) => {

    const HandleCurrencyChange = (event: ChangeEvent<HTMLSelectElement>) => {
        setSelectedCurrency(event.target.value as unknown as Currency);
    };

    return (
        <select className={"currency-select"} value={selectedCurrency || ''} onChange={HandleCurrencyChange}>
            <option value="">Select...</option>
            {options.map((option) => (
                <option key={option.value} value={option.value}>
                    {option.label}
                </option>
            ))}
        </select>
    );
};
