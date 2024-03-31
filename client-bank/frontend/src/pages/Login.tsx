import React from 'react';
import "../styles/Login.css";
import { sendOAuthRequest} from "../api/auth";

enum Role {
    Client,
    Staff
}

const Login: React.FC = () => {


    const RedirectToLoginPage = (role: Role) => {
        sendOAuthRequest()
    };

    return (
        <div className="login-container">
            <h2>Login</h2>

            <button type="button" onClick={() => RedirectToLoginPage(Role.Client)}>
                Log In as Client
            </button>
            <button type="button" onClick={() => RedirectToLoginPage(Role.Staff)}>
                Log In as Staff
            </button>
        </div>
    );
};

export default Login;