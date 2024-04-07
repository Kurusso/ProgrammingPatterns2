import React from 'react';
import "../styles/Login.css";
import {makeOauth2AuthUrl} from "../api/auth";
import {useNavigate} from "react-router-dom";

export enum Role {
    Client,
    Staff
}

const Login: React.FC = () => {

    const navigate = useNavigate();
    const RedirectToLoginPage = (role: Role) => {

        const url = makeOauth2AuthUrl(role);
        console.log(url)
        window.location.assign(url);
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