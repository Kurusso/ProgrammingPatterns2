import {LogoutButton} from "../components/LogoutButton";
import {Accounts} from "../components/Accounts";
import {CreateAccount} from "../components/CreateAccount";
import {AccountProvider} from "../contexts/AccountContext";
import {useEffect} from "react";
import {UsernameDisplay} from "../components/UserameDisplay";
import {Credits} from "../components/Credits";

export const Home = () => {

    console.log("check")


    return (
        <div>
            <h1>Home</h1>
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
                <Credits/>
            </AccountProvider>
        </div>
    );
};