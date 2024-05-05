import {Penalty} from "../../api/creditPenalties";


interface PenaltySelectProps {
    selectedPenalty: string | null,
    setSelectedPenalty: React.Dispatch<React.SetStateAction<string | null>>,
    penalties: Penalty[]
}

export const PenaltySelect: React.FC<PenaltySelectProps> = ({selectedPenalty, setSelectedPenalty, penalties}) => {
    return (
        <div>
            <h5>Select Penalty</h5>
            <select className={"penalty-select"} value={selectedPenalty || ''}
                    onChange={(e) => setSelectedPenalty(e.target.value)}>
                <option value="">Select...</option>
                {penalties.map((penalty) => (
                    <option key={penalty.creditId} value={penalty.creditId}>{penalty.creditId}</option>
                ))}
            </select>
        </div>
    );
};
//{TotalDebt ? `Debt to pay: ${TotalDebt.amount} ${Currency[TotalDebt.currency]}` : null}