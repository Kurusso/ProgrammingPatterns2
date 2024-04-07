import {UserManagerSettings, WebStorageStateStore} from 'oidc-client-ts';
import {Role} from "../pages/Login";


export const config: UserManagerSettings = {
    authority: "https://localhost:7212",
    client_id: "ClientApplication",
    redirect_uri: "https://localhost:3000/auth",
    client_secret: "901564A5-E7FE-42CB-B10D-61EF6A8F3655",
    response_type: "code",
    scope: "openid profile api1",
    post_logout_redirect_uri: "https://localhost:3000/auth",
    userStore: new WebStorageStateStore({store: window.localStorage}),
};


export function makeOauth2AuthUrl(role: Role): string {
    // depending on role specifying callback url


    let requestUrl = new URL(config.authority + "/auth");
    let queryParams = new URLSearchParams();
    queryParams.set("client_id", config.client_id);
    if (config.response_type != null) {
        queryParams.set("response_type", config.response_type);
    }
    queryParams.set("redirect_uri", config.redirect_uri);
    // if (config.scope != null) {
    //     queryParams.set("scope", config.scope);
    // }
    return requestUrl + "?" + queryParams.toString();

}


export function getAccessToken(): string {
    let token = localStorage.getItem('token');

    if (!token) {
        console.error('Token not found');
        throw new Error('Failed to get access_token');
    }

    // Remove the double quotes from the token
    token = token.replace(/"/g, '');

    token = `Bearer ${token}`
    console.log("token get")
    return token;
}


export function setAccessToken(token: string) {
    localStorage.setItem('token', JSON.stringify(token));
    console.log(`token set`)
}

export function delAccessToken() {
    localStorage.removeItem('token')
}

export function isAuthenticated(): boolean {
    const token = getAccessToken();
    return !!token;
}

