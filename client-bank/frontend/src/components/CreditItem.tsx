import {Currency, Money} from "../api/account";
import {useNavigate} from "react-router-dom";
import {CreditRate} from "../api/credit";

export interface CreditItemProps {
    Id: string,
    Rate: CreditRate,
    AccountId: string,
    TotalDebt: Money,
    monthPayment: Money
}


export const CreditItem: React.FC<CreditItemProps> = ({Id, AccountId, TotalDebt, monthPayment, Rate}) => {
    const navigate = useNavigate();
    return (
        <div>
            <div>Credit:{Id}</div>
            <div>Bound to Account: {AccountId}</div>
            <div>Credit rate: {Rate.name}: {Rate.monthPercent}%</div>
            <div> {TotalDebt ? `Debt to pay: ${TotalDebt.amount} ${Currency[TotalDebt.currency]}` : null}</div>
            <div> {monthPayment ? (`Payment per
                Month: ${monthPayment.amount}  ${Currency[monthPayment.currency]}`) : null}</div>
        </div>
    );
};