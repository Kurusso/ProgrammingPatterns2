import {LogoutButton} from "../components/Buttons/LogoutButton";
import {Accounts} from "../components/Accounts/Accounts";
import {CreateAccount} from "../components/Accounts/CreateAccount";
import {AccountsProvider} from "../contexts/AccountsContext";
import {UsernameDisplay} from "../components/UsernameDisplay";
import {Credits} from "../components/Credits/Credits";
import {CreditProvider} from "../contexts/CreditContext";
import {TakeCredit} from "../components/Credits/TakeCredit";
import {TransferMoney} from "../components/Accounts/TransferMoney";
export const Home = () => {


    return (
        <div>
            <h1>Home</h1>
            <UsernameDisplay/>
            <LogoutButton/>
            <AccountsProvider>
                <CreateAccount/>
                <TransferMoney/>
                <Accounts/>

                <CreditProvider>
                    <TakeCredit/>
                    <Credits/>
                </CreditProvider>
            </AccountsProvider>
        </div>
    );
}