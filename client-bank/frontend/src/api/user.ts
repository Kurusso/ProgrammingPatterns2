import {User} from "../contexts/UserContext";
import {magicConsts} from "./magicConst";



export class UserService{
    static async  getUser(token: string): Promise<User> {
        const response = await fetch(`${magicConsts.getUserEndpoint}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            }
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        console.log(response)
        return await response.json();
    }
}