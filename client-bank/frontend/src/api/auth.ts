import {UserManager, UserManagerSettings, WebStorageStateStore} from 'oidc-client-ts';
import {Role} from "../pages/Login";


const config: UserManagerSettings = {
    authority: "https://localhost:7212",
    client_id: "ClientApplication",
    redirect_uri: "http://localhost:3000",
    client_secret: "901564A5-E7FE-42CB-B10D-61EF6A8F3655",
    response_type: "code",
    scope: "openid profile api1",
    post_logout_redirect_uri: "http://localhost:3000",
    userStore: new WebStorageStateStore({store: window.localStorage}),
};


const userManager = new UserManager(config);


export async function getUser() {
    const user = await userManager.getUser();
    return user;
}

export async function isAuthenticated(role: Role) {
    let token = await getAccessToken();

    return !!token;
}


export function makeOauth2AuthUrl(role: Role): string {
    // depending on role specifying callback url

    let requestUrl = new URL(config.authority + "/auth");
    let queryParams = new URLSearchParams();
    queryParams.set("client_id", config.client_id);
    if (config.response_type != null) {
        queryParams.set("response_type", config.response_type);
    }
    queryParams.set("redirect_uri", config.redirect_uri);
    if (config.scope != null) {
        queryParams.set("scope", config.scope);
    }
    return requestUrl + "?" + queryParams.toString();

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
    console.log("sending OAuth req")
    //return await userManager.signinRedirect();


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
