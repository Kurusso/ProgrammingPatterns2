// UsernameDisplay.tsx
import {useUser} from "../contexts/UserContext";
import {UserService} from "../api/user";
import {useEffect} from "react";
import {getAccessToken, isAuthenticated} from "../api/auth";


export const UsernameDisplay = () => {
    const { user, setUser } = useUser();



    useEffect(() => {
        const fetchUser = async () => {

            if (!user && isAuthenticated()) {
                try {
                    let userData = await UserService.getUser();
                    setUser(userData);
                } catch (error) {
                    console.error('An error occurred while getting user data:', error);
                }
            }
        };

        fetchUser();
    }, [user]);

    return (
        <div id={"username"}>
            {user ? `${user.username}` : "You are not logged in."}
        </div>
    );
};
