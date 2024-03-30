import {LogoutButton} from "../components/LogoutButton";
import {Accounts} from "../components/Accounts";
import {CreateAccount} from "../components/CreateAccount";
import {AccountsProvider} from "../contexts/AccountsContext";
import {UsernameDisplay} from "../components/UserameDisplay";
import {Credits} from "../components/Credits";
import {CreditProvider} from "../contexts/CreditContext";
import {TakeCredit} from "../components/TakeCredit";
export const Home = () => {


    return (
        <div>
            <h1>Home</h1>
            <UsernameDisplay/>
            <LogoutButton/>
            <AccountsProvider>
                <CreateAccount/>
                <Accounts/>

                <CreditProvider>
                    <TakeCredit/>
                    <Credits/>
                </CreditProvider>
            </AccountsProvider>
        </div>
    );
};