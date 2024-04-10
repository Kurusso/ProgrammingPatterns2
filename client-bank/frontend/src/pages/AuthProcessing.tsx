import {config, setAccessToken} from "../api/auth";
import {useEffect} from "react";
import {useNavigate} from "react-router-dom";
import {useAuth} from "../contexts/AuthContext";

export const AuthProcessing = () => {

    const navigate=useNavigate();
    const {handleLogin}=useAuth()

    useEffect(() => {
        const RetrieveOauth2Token = async (code: string) => {
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
                handleLogin(access_token)
                navigate("/")
            } catch (error) {
                console.error(error);
                throw  error;
            }
        };


        const params = new URLSearchParams(window.location.search);
        const code = params.get('code');
        console.log(code);

        if (code != null) {
            RetrieveOauth2Token(code);
        }

    }, []);



    return <div>Signing in</div>;
}
