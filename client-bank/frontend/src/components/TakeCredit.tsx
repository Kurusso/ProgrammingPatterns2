import {useCredits} from "../contexts/CreditContext";
import {useEffect, useState} from "react";
import {Currency} from "../api/account";
import {CurrencyInput} from "./CurrencyInput";
import {CurrencySelect} from "./CurrencySelect";
import {CreditRate, getCreditRates, getCredits, takeCredit} from "../api/credit";
import {mapCreditDataToItemProps} from "./Credits";
import {AccountSelect} from "./AccountSelect";

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

                let creditRatesData = await getCreditRates()
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
            const storedToken = localStorage.getItem('token');
            if (!storedToken) {
                throw new Error('No token found');
            }

            const parsedToken = JSON.parse(storedToken).token;
            if (!parsedToken) {
                throw new Error('Invalid token');
            }

            if (!selectedCreditRate || !selectedAccount || !selectedCurrency) {
                throw new Error('Credit rate, account, or currency not selected');
            }

            if(totalMoney<=0 || moneyPerMonth<=0){
                throw new Error('Incorrect amount of money')
            }

            await takeCredit(selectedCreditRate, parsedToken, selectedAccount, selectedCurrency, totalMoney, moneyPerMonth);
            const credits = await getCredits(parsedToken);
            const creditItemsData = mapCreditDataToItemProps(credits);
            setCreditItems(creditItemsData);
        } catch (error) {
            console.error('An error occurred while taking credit:', error);
        }
    };


    return (
        <div>
            <div>
                <h5>Credit rate</h5>
                <select value={selectedCreditRate || ''} onChange={(e) => setSelectedCreditRate(e.target.value)}>
                    <option value="">Select...</option>
                    {creditRates.map((creditRate) => (
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
            <button onClick={HandleTakingCredit}>Take Credit</button>
        </div>
    );
};