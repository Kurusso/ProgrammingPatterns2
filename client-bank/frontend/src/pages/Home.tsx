import {LogoutButton} from "../components/LogoutButton";
import {Accounts} from "../components/Accounts";
export const Home = () => {
    return (
        <div>
           <h1>Home</h1>
            {/*        UserName
            List of Accounts // Deposit Money on particular account ||Close particular account
            List of Credits  // Pay money for particular credit|| Repay Credit

            Take new Credit
            Create new Account*/}
            <LogoutButton/>
            <Accounts/>
        </div>
    );
};