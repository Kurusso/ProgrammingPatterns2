import {AccountSelect} from "../../Selects/AccountSelect";
import {CurrencySelect} from "../../Selects/CurrencySelect";
import {CurrencyInput} from "../../Input/CurrencyInput";
import {PenaltySelect} from "../../Selects/PenaltySelect";
import {useState} from "react";
import {Currency} from "../../../api/account";
import {creditPenalties, Penalty} from "../../../api/creditPenalties";
import {isAuthenticated} from "../../../api/auth";


export const RepayPenalty = ({penalties}: { penalties: Penalty[] }) => {
    const [selectedPenalty, setSelectedPenalty] = useState<string | null>(null);

    const [selectedAccount, setSelectedAccount] = useState<string | null>(null);
    const [selectedCurrency, setSelectedCurrency] = useState<Currency | null>(null);
    const [penaltyMoneyToPay, setPenaltyMoneyToPay] = useState<number>(0);

    function HandleRepayingPenalty() {
        try {
            if (!isAuthenticated())
                throw new Error('not Authenticated')

            if (!selectedPenalty)
                throw new Error('Penalty not selected')
            if (!selectedAccount)
                throw new Error('Account not selected')
            if(!selectedCurrency)
                throw new Error('Currency not selected')
            if(!penaltyMoneyToPay || penaltyMoneyToPay<=0)
                throw new Error('Incorrect money')

            creditPenalties.RepayPenalty(selectedPenalty, penaltyMoneyToPay, selectedCurrency, selectedAccount);


        } catch (e) {
            console.log(e)
        }
    }

    return (
        <div>
            <h3>Repay penalty</h3>
            Penalty

            <AccountSelect selectedAccount={selectedAccount} setSelectedAccount={setSelectedAccount}/>
            <CurrencySelect selectedCurrency={selectedCurrency} setSelectedCurrency={setSelectedCurrency}/>
            <CurrencyInput amount={penaltyMoneyToPay} setAmount={setPenaltyMoneyToPay}/>
        </div>

    );
};