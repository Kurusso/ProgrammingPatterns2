import {UsernameDisplay} from "../components/UserameDisplay";
import {LogoutButton} from "../components/LogoutButton";
import {HomeButton} from "../components/HomeButton";
import React, {useEffect, useState} from "react";
import {useParams} from "react-router-dom";
import {CreditData, getCredit} from "../api/credit";
import {Currency, getAccount, Money} from "../api/account";
import {RepayCredit} from "../components/RepayCredit";

export const Credit = () => {
    const {creditId} = useParams<{ creditId: string }>();
    const [creditData, setCreditData] = useState<CreditData>()
    const [totalDebt,setTotalDebt]=useState<Money>()
    useEffect(() => {

        const fetchData = async () => {
            try {
                console.log("works?")
                const storedToken = localStorage.getItem('token');
                if (!storedToken) {
                    throw new Error('No token found');
                }

                const parsedToken = JSON.parse(storedToken).token;
                if (!parsedToken) {
                    throw new Error('Invalid token');
                }
                if (!creditId) {
                    throw new Error('Invalid creditId');
                }

                const credit = await getCredit(parsedToken, creditId);
                setCreditData(credit);
                console.log('fetched credit:', credit)
            } catch (error) {
                console.error('Error fetching credit:', error);
            }

            if (creditData?.remainingDebt && creditData.unpaidDebt) {
                let debt= creditData?.remainingDebt.amount + creditData.unpaidDebt.amount;

                setTotalDebt({amount:debt,
                    currency:creditData?.remainingDebt.currency})
                console.log("total debt",totalDebt);
            }
        }
        fetchData();





    }, []);

    return (
        <div>
            <h2>Credit</h2>
            <UsernameDisplay/>
            <LogoutButton/>
            <HomeButton/>
            <div>
                <RepayCredit/>

                <div>
                    <h3>CreditInformation</h3>
                    <div>
                        <div>Credit: {creditData?.id}</div>
                        <div>Credit rate: {creditData?.creditRate.name}: {creditData?.creditRate.monthPercent}%
                        </div>
                        <div> {creditData?.monthPayAmount ? (`Payment per
                Month: ${creditData?.monthPayAmount.amount}  ${Currency[creditData?.monthPayAmount.currency]}`) : null}</div>
                    <div>{creditData?.fullMoneyAmount?(`Total Money Amount ${creditData?.fullMoneyAmount}`):null}</div>
                    <div>{creditData?.monthPayAmount?(`Payment per month ${creditData?.fullMoneyAmount}`):null}</div>
                    <div>{totalDebt? `Debt to pay: ${totalDebt.amount} ${Currency[totalDebt.currency]}` : null}</div>
                    <div>{creditData?.remainingDebt? `Remaining debt: ${creditData?.remainingDebt.amount} ${Currency[creditData?.remainingDebt.currency]}` : null}</div>
                        <div>{creditData?.unpaidDebt ? `Unpaid debt: ${creditData?.unpaidDebt.amount} ${Currency[creditData?.unpaidDebt.currency]}` : null}</div>
                    </div>

                </div>
            </div>
        </div>
    );
};