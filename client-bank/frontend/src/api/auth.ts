import {UserManager} from 'oidc-client-ts';
import {AuthConfig} from "../utils/config/AuthConfig";


const userManager = new UserManager({
    authority: 'https://localhost:7212', // Your authority server
    client_id: 'ClientApplication', // Your client id
    client_secret: "901564A5-E7FE-42CB-B10D-61EF6A8F3655",
    redirect_uri: 'http://localhost:auth/', // Your callback route
    response_type: 'code',
    scope: 'openid profile api1',
});


export async function getUser() {
    const user = await userManager.getUser();
    return user;
}

export async function isAuthenticated() {
    let token = await getAccessToken();

    return !!token;
}

export async function handleOAuthCallback(callbackUrl: string) {
    try {
        const user = await userManager.signinRedirectCallback(callbackUrl);
        return user;
    } catch (e) {
        alert(e);
        console.log(`error while handling oauth callback: ${e}`);
    }
}

export async function sendOAuthRequest() {
    return await userManager.signinRedirect();
}

// renews token using refresh token
export async function renewToken() {
    const user = await userManager.signinSilent();

    return user;
}

export async function getAccessToken() {
    const user = await getUser();
    return user?.access_token;
}

export async function logout() {
    await userManager.clearStaleState()
    await userManager.signoutRedirect();
}


// export class AuthService {
//     static async getToken(email: string, password: string): Promise<string> {
//         try {
//
//             const response = await fetch(magicConsts.loginEndpoint, {
//                 method: 'POST',
//                 headers: {
//                     'Content-Type': 'application/json',
//                 },
//                 body: JSON.stringify({
//                     username: email,
//                     password: password
//                 }),
//             });
//
//             return await response.json();
//         } catch (error) {
//             throw error;
//         }
//     }
// }
