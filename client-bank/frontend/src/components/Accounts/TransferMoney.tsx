import {useState} from "react";
import {AccountSelect} from "../Selects/AccountSelect";
import {CurrencyInput} from "../Input/CurrencyInput";
import {Currency} from "../../api/account";
import {CurrencySelect} from "../Selects/CurrencySelect";

export const TransferMoney = () => {
    const [selectedAccount, setSelectedAccount] = useState<string | null>(null);
    const [transferMoney, setTransferMoney] = useState<number>(0);
    const [selectedCurrency, setSelectedCurrency] = useState<Currency | null>(null);
    const [accountToTransfer, setAccountToTransfer] = useState<string >('')

    const handleAccountToTransferChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setAccountToTransfer(event.target.value);
    };

    function handleTransfer() {

    }

    return (
        <div>
            <h3>Transfer Money</h3>
            <AccountSelect selectedAccount={selectedAccount} setSelectedAccount={setSelectedAccount}/>
            <div>
                <h5>Money</h5>
                <CurrencyInput amount={transferMoney} setAmount={setTransferMoney}/>
            </div>
            <div>
                <h5>Currency</h5>
                <CurrencySelect selectedCurrency={selectedCurrency} setSelectedCurrency={setSelectedCurrency}/>
            </div>
            <div>
                <h5>Account To transfer</h5>
                <input type="text" id="guid" name="guid" value={accountToTransfer as string}
                       onChange={handleAccountToTransferChange}
                       pattern="[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}"
                />
            </div>

            <button className={"transfer-money-btn"} type={"button"} onClick={handleTransfer}>Transfer</button>
        </div>

    );
};