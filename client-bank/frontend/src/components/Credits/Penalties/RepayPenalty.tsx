import {AccountSelect} from "../../Selects/AccountSelect";
import {CurrencySelect} from "../../Selects/CurrencySelect";
import {CurrencyInput} from "../../Input/CurrencyInput";
import {PenaltySelect} from "../../Selects/PenaltySelect";
import {useState} from "react";
import {Currency} from "../../../api/account";
import {Penalty} from "../../../api/creditPenalties";



export const RepayPenalty = ({penalties}: {penalties: Penalty[]}) => {
    const [selectedPenalty, setSelectedPenalty] = useState<string | null>(null);

    const [selectedAccount, setSelectedAccount] = useState<string | null>(null);
    const [selectedCurrency, setSelectedCurrency] = useState<Currency | null>(null);
    const [penaltyMoneyToPay, setPenaltyMoneyToPay] = useState<number>(0);

    return (
        <div>
            <h3>Repay penalty</h3>
            Penalty
            <PenaltySelect />
            <AccountSelect selectedAccount={selectedAccount} setSelectedAccount={setSelectedAccount}/>
            <CurrencySelect selectedCurrency={selectedCurrency} setSelectedCurrency={setSelectedCurrency}/>
            <CurrencyInput amount={penaltyMoneyToPay} setAmount={setPenaltyMoneyToPay}/>
        </div>

    );
};