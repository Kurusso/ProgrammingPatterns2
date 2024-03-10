import {useEffect} from "react";
import {CreditData, getCredits} from "../api/credit";
import {useCredits} from "../contexts/CreditContext";
import {CreditItem, CreditItemProps} from "./CreditItem";

export const Credits = () => {
const {creditItems,setCreditItems}=useCredits();

useEffect(()=>{
    const fetchData=async ()=>{
        try {
            const storedToken = localStorage.getItem('token');
            if (storedToken) {
                const parsedToken = JSON.parse(storedToken).token;
                if (parsedToken) {
                    const credits= await getCredits(parsedToken);
                    let CreditItemsData=mapCreditDataToItemProps(credits);
                    setCreditItems(CreditItemsData);
                    console.log('Fetched credits:', credits);
                }
            } else {
                console.log('No token found');
            }
        } catch (error) {
            console.error('Error fetching credits:', error);
        }
    }
fetchData();

},[])



    return (
        <div id={"credits"}><h3>Credits</h3>
            <div id={"credit-items"}>
                {creditItems.map(item => (
                    <CreditItem key={item.Id} Id={item.Id} Rate={item.Rate} CreditAccountId={item.CreditAccountId}
                                TotalDebt={item.TotalDebt} monthPayment={item.monthPayment}/>))}
            </div>

        </div>
    );
};

export function mapCreditDataToItemProps(creditDataArray: CreditData[]): CreditItemProps[] {
    return creditDataArray.map(creditData => {
        const {id, creditRate, remainingDebt, unpaidDebt,monthPayAmount,payingAccountId}=creditData;

        let totalDebt = 0;
        if (remainingDebt && unpaidDebt) {
            totalDebt = remainingDebt.amount + unpaidDebt.amount;
        }

        return {
            Id: id,
            Rate: creditRate,
            CreditAccountId: payingAccountId,
            monthPayment: monthPayAmount,
            TotalDebt: {
                amount: totalDebt,
                currency: remainingDebt ? remainingDebt.currency : 0
            }
        }
    });
}
