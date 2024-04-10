import {User} from "../contexts/UserContext";
import {magicConsts} from "./magicConst";
import {getAccessToken} from "./auth";



export class UserService{
    static async  getUser(): Promise<User> {
        const response = await fetch(`${magicConsts.getUserEndpoint}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': getAccessToken(),
            }
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        console.log(response)
        return await response.json();
    }
}