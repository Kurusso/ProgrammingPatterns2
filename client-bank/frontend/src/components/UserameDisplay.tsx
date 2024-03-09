// UsernameDisplay.tsx
import {useUser} from "../contexts/UserContext";
import {getUser} from "../api/user";
import {useEffect} from "react";


export const UsernameDisplay = () => {
    const { user, setUser } = useUser();

    const storedToken = localStorage.getItem('token');
    const parsedToken = storedToken ? JSON.parse(storedToken).token : null;

    useEffect(() => {
        const fetchUser = async () => {
            if (!user && parsedToken) {
                try {
                    let userData = await getUser(parsedToken);
                    setUser(userData);
                } catch (error) {
                    console.error('An error occurred while getting user data:', error);
                }
            }
        };

        fetchUser();
    }, [user, parsedToken, setUser]);

    return (
        <div>
            {user ? `${user.username}` : "You are not logged in."}
        </div>
    );
};
