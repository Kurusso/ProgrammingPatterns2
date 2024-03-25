import {magicConsts} from "./magicConst";

export class AuthService {
    static async getToken(email: string, password: string): Promise<string> {
        try {

            const response = await fetch(magicConsts.loginEndpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    username: email,
                    password: password
                }),
            });

            return await response.json();
        } catch (error) {
            throw error;
        }
    }
}
