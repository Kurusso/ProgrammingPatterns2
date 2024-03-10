
interface CurrencyInputProps{
    amount:number,
    setAmount: React.Dispatch<React.SetStateAction<number>>
}
export const CurrencyInput:React.FC<CurrencyInputProps> = ({amount,setAmount}) => {


    const handleAmountChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newAmount = parseFloat(event.target.value);
        setAmount(newAmount);
    };

    return (
        <input type="number" value={amount} onChange={handleAmountChange} min="0" step="0.01"/>
    );
};