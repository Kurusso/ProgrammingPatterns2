import {User} from "../contexts/UserContext";
import {getUserEndpoint} from "./magicConst";

export async function getUser(token: string): Promise<User> {
    const response = await fetch(`${getUserEndpoint}/${token}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        }
    });

    if (!response.ok) {
        throw new Error('Network response was not ok');
    }

    return await response.json();
}