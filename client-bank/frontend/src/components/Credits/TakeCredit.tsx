import {useCredits} from "../../contexts/CreditContext";
import {useEffect, useState} from "react";
import {Currency} from "../../api/account";
import {CurrencyInput} from "../Input/CurrencyInput";
import {CurrencySelect} from "../Selects/CurrencySelect";
import {CreditRate,  CreditService} from "../../api/credit";
import {mapCreditDataToItemProps} from "./Credits";
import {AccountSelect} from "../Selects/AccountSelect";
import {isAuthenticated} from "../../api/auth";

export const TakeCredit = () => {
    const {setCreditItems} = useCredits();

    const [selectedCurrency, setSelectedCurrency] = useState<Currency | null>(null);
    const [totalMoney, setTotalMoney] = useState<number>(0);
    const [moneyPerMonth, setMoneyPerMonth] = useState<number>(0);
    const [selectedCreditRate, setSelectedCreditRate] = useState<string | null>(null);
    const [selectedAccount, setSelectedAccount] = useState<string | null>(null);

    const [creditRates, setCreditRates] = useState<CreditRate[]>([])


    useEffect(() => {

        const fetchData = async () => {


            try {

                let creditRatesData = await CreditService.getCreditRates()
                setCreditRates(creditRatesData);
                console.log('Fetched credit rates:', creditRatesData);

            } catch (error) {
                console.error('Error fetching credit rates:', error);
            }


        }

        fetchData();

    }, []);


    const HandleTakingCredit = async () => {
        try {

            if (!isAuthenticated()) {
                throw new Error('Invalid token');
            }

            if (!selectedCreditRate || !selectedAccount || !selectedCurrency) {
                throw new Error('Credit rate, account, or currency not selected');
            }

            if(totalMoney<=0 || moneyPerMonth<=0){
                throw new Error('Incorrect amount of money')
            }

            await CreditService.takeCredit(selectedCreditRate, selectedAccount, selectedCurrency, totalMoney, moneyPerMonth);
            const credits = await CreditService.getCredits();
            const creditItemsData = mapCreditDataToItemProps(credits);
            setCreditItems(creditItemsData);
        } catch (error) {
            console.error('An error occurred while taking credit:', error);
        }
    };


    return (
        <div>
            <h3>Take Credit</h3>
            <div className={"credit-rate "}>
                <h5>Credit rate</h5>
                <select value={selectedCreditRate || ''} onChange={(e) => setSelectedCreditRate(e.target.value)}>
                    <option value="">Select...</option>
                    {Array.isArray(creditRates) &&creditRates.map((creditRate) => (
                        <option key={creditRate.id}
                                value={creditRate.id}>{creditRate.name}: {creditRate.monthPercent * 100}%</option>
                    ))}
                </select>

            </div>
            <div>

                <AccountSelect selectedAccount={selectedAccount} setSelectedAccount={setSelectedAccount}/>
            </div>
            <div>
                <h5>Currency</h5>
                <CurrencySelect selectedCurrency={selectedCurrency} setSelectedCurrency={setSelectedCurrency}/>
            </div>
            <div>
                <h5>Total money</h5>
                <CurrencyInput amount={totalMoney} setAmount={setTotalMoney}/>
            </div>
            <div>
                <h5>Payment per month</h5>
                <CurrencyInput amount={moneyPerMonth} setAmount={setMoneyPerMonth}/>
            </div>
            <button className={"take-credit-btn"} onClick={HandleTakingCredit}>Take Credit</button>
        </div>
    );
};