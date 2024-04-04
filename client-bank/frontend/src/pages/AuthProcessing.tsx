import {config, userManager} from "../api/auth";
import {useEffect} from "react";

export const AuthProcessing = () => {


    useEffect(() => {
        const RetrieveOauth2Token = async (code: string): Promise<string> => {
            const requestUrl = `${config.authority}/auth/token`;

            const headers = {
                'Content-Type': 'application/x-www-form-urlencoded',
                'Accept': 'application/x-www-form-urlencoded, application/json',
                'Authorization': 'Basic Q2xpZW50QXBwbGljYXRpb246OTAxNTY0QTUtRTdGRS00MkNCLUIxMEQtNjFFRjZBOEYzNjU1',

            };

            const data = new URLSearchParams();
            data.append('grant_type', 'authorization_code');
            data.append('code', code);
            data.append('redirect_uri', config.redirect_uri);

            console.log(data.toString());
            try {
                const response = await fetch(requestUrl, {
                    method: 'POST',
                    headers,
                    body: data.toString()
                });

                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }

                const responseData = await response.json();
                const {access_token} = responseData;

                if (!access_token) {
                    throw new Error('Failed to get access_token');
                }
                console.log(access_token)
                return access_token;
            } catch (error) {
                console.error(error);
                return '';
            }
        };


        const params = new URLSearchParams(window.location.search);
        const code = params.get('code');
        console.log(code);

        if (code != null) {
            RetrieveOauth2Token(code);
        }

    }, []);

    console.log(1)

    return <div>sssss...</div>;
}
