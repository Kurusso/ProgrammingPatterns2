import {Currency, Money} from "../api/account";
import {useNavigate} from "react-router-dom";
import {CreditRate} from "../api/credit";
import {useCallback} from "react";

export interface CreditItemProps {
    Id: string,
    Rate: CreditRate,
    CreditAccountId: string,
    TotalDebt: Money,
    monthPayment: Money
}


export const CreditItem: React.FC<CreditItemProps> = ({Id, CreditAccountId, TotalDebt, monthPayment, Rate}) => {
    const navigate = useNavigate();

    const handleClickAccountDetails = useCallback(() => {
        navigate(`/credit/${Id}`);
    }, [navigate, Id]);

    return (
        <div className={"credit-item"}>
            <button className={"credit-btn"} onClick={handleClickAccountDetails}> Credit:{Id}</button>
            <div>Bound to Account: {CreditAccountId}</div>
            <div>Credit rate: {Rate.name}: {Rate.monthPercent * 100}%</div>
            <div> {TotalDebt ? `Debt to pay: ${TotalDebt.amount} ${Currency[TotalDebt.currency]}` : null}</div>
            <div> {monthPayment ? (`Payment per
                Month: ${monthPayment.amount}  ${Currency[monthPayment.currency]}`) : null}</div>
        </div>
    );
};