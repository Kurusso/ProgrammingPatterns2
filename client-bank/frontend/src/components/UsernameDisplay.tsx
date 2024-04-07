// UsernameDisplay.tsx
import {useUser} from "../contexts/UserContext";
import {UserService} from "../api/user";
import {useEffect} from "react";
import {getAccessToken} from "../api/auth";


export const UsernameDisplay = () => {
    const { user, setUser } = useUser();



    useEffect(() => {
        const fetchUser = async () => {

            let token = getAccessToken()

            if (!user && token) {
                try {
                    console.log(`user ${user} token:${token}`)
                    let userData = await UserService.getUser(token);
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
