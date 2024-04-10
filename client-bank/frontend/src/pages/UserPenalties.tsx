import {UsernameDisplay} from "../components/UsernameDisplay";
import {LogoutButton} from "../components/Buttons/LogoutButton";
import ThemeButton from "../components/Buttons/ThemeButton";
import {RepayPenalty} from "../components/Credits/Penalties/RepayPenalty";
import {useEffect, useState} from "react";
import {creditPenalties, Penalty} from "../api/creditPenalties";
import {AccountsProvider} from "../contexts/AccountsContext";
import {isAuthenticated} from "../api/auth";
import {HomeButton} from "../components/Buttons/HomeButton";
import {CreditScore} from "../components/Credits/CreditScore";

export const UserPenalties = () => {
    const [userPenalties, setUserPenalties] = useState<Penalty[]>([])

    useEffect(() => {

        const fetchPenalties =async () => {
            try {
                if (!isAuthenticated())
                    throw new Error('not authenticated');
                const penalties = await creditPenalties.GetUserPenalties();
                console.log(penalties)
                setUserPenalties(penalties)
            } catch (e) {
                console.log(e)
            }
        }
        //Load Penalties
        fetchPenalties();
    }, []);

    return (
        <div>
            <h1>User Penalties</h1>
            <UsernameDisplay/>
            <HomeButton/>
            <LogoutButton/>
            <ThemeButton/>
            <CreditScore/>
            <AccountsProvider>
                <RepayPenalty penalties={userPenalties}/>
                <div className={"user-penalties-items"}>

                </div>
            </AccountsProvider>
        </div>
    );
};