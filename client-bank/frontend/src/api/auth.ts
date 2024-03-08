import {loginEndpoint} from "./magicConst";

export async function getToken(email: string, password: string): Promise<string> {
    try {

        const response = await fetch(loginEndpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
            }),
        });

        return await response.json();
    } catch (error) {
        throw error;
    }
}