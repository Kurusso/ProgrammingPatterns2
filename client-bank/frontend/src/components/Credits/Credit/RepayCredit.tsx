import {CreditService} from "../../../api/credit";
import {CurrencyInput} from "../../Input/CurrencyInput";
import {CurrencySelect} from "../../Selects/CurrencySelect";
import {useState} from "react";
import {Currency} from "../../../api/account";
import {AccountSelect} from "../../Selects/AccountSelect";
import {isAuthenticated} from "../../../api/auth";

interface RepayCreditProps{
    creditId:string,
    accountId:string
}

export const RepayCredit:React.FC<RepayCreditProps> = ({creditId,accountId}) => {
    const [selectedAccount, setSelectedAccount] = useState<string | null>(null);

    const [selectedCurrency, setSelectedCurrency] = useState<Currency | null>(null);
    const [amount, setAmount] = useState<number>(0);
    const HandleCreditPayment =async () => {
        try {

            if (!isAuthenticated())
                throw new Error('Invalid token');


            if(!selectedCurrency||!amount||amount<=0)
                throw new Error('Invalid money input');

            if(!selectedAccount)
                throw new Error('Account wasnt selected')

            console.log("paying for credit")
            await CreditService.repayCredit(creditId,amount,selectedCurrency,selectedAccount);
        }
        catch (e) {

        }

    };


    return (
        <div className={"repay-credit"}>
            <h3>Repay Credit</h3>
            <AccountSelect selectedAccount={selectedAccount} setSelectedAccount={setSelectedAccount}/>
            <h4>Currency</h4>
            <CurrencySelect selectedCurrency={selectedCurrency} setSelectedCurrency={setSelectedCurrency}/>
            <CurrencyInput amount={amount} setAmount={setAmount}/>
            <button className={"repay-credit-btn"} onClick={HandleCreditPayment}>Repay</button>
        </div>
    );
};