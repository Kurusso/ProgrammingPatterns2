import {LogoutButton} from "../components/LogoutButton";
import {Accounts} from "../components/Accounts";
import {CreateAccount} from "../components/CreateAccount";
import {AccountProvider} from "../contexts/AccountContext";
import {useEffect} from "react";
import {UsernameDisplay} from "../components/UserameDisplay";
import {Credits} from "../components/Credits";
import {CreditProvider} from "../contexts/CreditContext";
import {TakeCredit} from "../components/TakeCredit";
import "../styles/Home.css"
export const Home = () => {


    return (
        <div>
            <h1 id={"title"}>Home</h1>
            {/*        UserName
            List of Accounts // Deposit Money on particular account ||Close particular account
            List of Credits  // Pay money for particular credit|| Repay Credit

            Take new Credit
            Create new Account*/}
            <UsernameDisplay/>
            <LogoutButton/>
            <AccountProvider>
                <CreateAccount/>
                <Accounts/>

                <CreditProvider>
                    <TakeCredit/>
                    <Credits/>
                </CreditProvider>
            </AccountProvider>
        </div>
    );
};