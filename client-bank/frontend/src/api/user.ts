import {User} from "../contexts/UserContext";
import {magicConsts} from "./magicConst";



export class UserService{
    static async  getUser(token: string): Promise<User> {
        const response = await fetch(`${magicConsts.getUserEndpoint}/${token}`, {
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
}