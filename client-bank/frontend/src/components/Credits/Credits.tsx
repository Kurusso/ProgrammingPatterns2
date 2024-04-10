import {useEffect} from "react";
import {CreditData, CreditService} from "../../api/credit";
import {useCredits} from "../../contexts/CreditContext";
import {CreditItem, CreditItemProps} from "./CreditItem";
import {isAuthenticated} from "../../api/auth";
import {Simulate} from "react-dom/test-utils";
import error = Simulate.error;

export const Credits = () => {
const {creditItems,setCreditItems}=useCredits();

useEffect(()=>{
    const fetchData=async ()=>{
        try {

            if (!isAuthenticated()) {
                throw Error('No token found');
            } else {
                const credits = await CreditService.getCredits();
                let CreditItemsData = mapCreditDataToItemProps(credits);
                setCreditItems(CreditItemsData);
                console.log('Fetched credits:', credits);
            }
        } catch (error) {
            console.error('Error fetching credits:', error);
        }
    }
fetchData();

},[])



    return (
        <div className={"credits"}><h3>Credits</h3>
            <div className={"credit-items"}>
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
