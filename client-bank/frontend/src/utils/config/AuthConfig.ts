const authSettings = {
    authority: 'https://localhost:7212',
    client_id: 'ClientApplication',
    client_secret: '901564A5-E7FE-42CB-B10D-61EF6A8F3655',
    redirect_uri: 'http://localhost:7212/auth',
    silent_redirect_uri: 'https://localhost:7212/auth',
    post_logout_redirect_uri: 'http://localhost:7212/',
    response_type: 'code',
    scope: 'api1'
};


export const AuthConfig = {
    settings: authSettings,
    flow: 'client'
};