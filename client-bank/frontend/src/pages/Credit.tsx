import {UsernameDisplay} from "../components/UsernameDisplay";
import {LogoutButton} from "../components/Buttons/LogoutButton";
import {HomeButton} from "../components/Buttons/HomeButton";
import React, {useEffect, useState} from "react";
import {useParams} from "react-router-dom";
import {CreditData, CreditService} from "../api/credit";
import {Currency, Money} from "../api/account";
import {RepayCredit} from "../components/Credits/Credit/RepayCredit";
import {AccountsProvider} from "../contexts/AccountsContext";
import {isAuthenticated} from "../api/auth";

export const Credit = () => {
    const {creditId} = useParams<{ creditId: string }>();
    const [creditData, setCreditData] = useState<CreditData>()
    const [totalDebt, setTotalDebt] = useState<Money>()
    useEffect(() => {

        const fetchData = async () => {
            try {
                console.log("works?")

                if (!isAuthenticated()) {
                    throw new Error('Invalid token');
                }
                if (!creditId) {
                    throw new Error('Invalid creditId');
                }

                const credit = await CreditService.getCredit( creditId);
                setCreditData(credit);
                console.log('fetched credit:', credit)
            } catch (error) {
                console.error('Error fetching credit:', error);
            }

            if (creditData?.remainingDebt && creditData.unpaidDebt) {
                let debt = creditData?.remainingDebt.amount + creditData.unpaidDebt.amount;

                setTotalDebt({
                    amount: debt,
                    currency: creditData?.remainingDebt.currency
                })
                console.log("total debt", totalDebt);
            }
        }
        fetchData();


    }, []);

    return (
        <div>
            <AccountsProvider>
                <h2>Credit</h2>
                <UsernameDisplay/>
                <LogoutButton/>
                <HomeButton/>
                <div>
                    <RepayCredit creditId={creditData?.id!} accountId={creditData?.payingAccountId!}/>

                    <div className={"credit-info"}>
                        <h3>Credit Information</h3>
                        <div className={"credit-data"}>
                            <div>Credit: {creditData?.id}</div>
                            <div>Credit
                                rate: {creditData?.creditRate.name}: {creditData?.creditRate.monthPercent! * 100}%
                            </div>
                            <div>{creditData?.fullMoneyAmount ? (`Total Money Amount: ${creditData?.fullMoneyAmount.amount} ${Currency[creditData?.fullMoneyAmount.currency]}`) : null}</div>
                            <div>{creditData?.monthPayAmount ? (`Payment per month: ${creditData?.monthPayAmount.amount} ${Currency[creditData?.monthPayAmount.currency]}`) : null}</div>
                            <div>{totalDebt ? `Debt to pay: ${totalDebt.amount} ${Currency[totalDebt.currency]}` : null}</div>
                            <div>{creditData?.remainingDebt ? `Remaining debt: ${creditData?.remainingDebt.amount} ${Currency[creditData?.remainingDebt.currency]}` : null}</div>
                            <div>{creditData?.unpaidDebt ? `Unpaid debt: ${creditData?.unpaidDebt.amount} ${Currency[creditData?.unpaidDebt.currency]}` : null}</div>
                        </div>

                    </div>
                </div>
            </AccountsProvider>
        </div>
    );
}