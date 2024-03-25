import React, { useState } from 'react';
import {useNavigate} from 'react-router-dom';
import  "../styles/Login.css";
import { useAuth } from '../contexts/AuthContext';
import {AuthService} from "../api/auth";
const Login: React.FC = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const navigate = useNavigate();

    const { handleLogin } = useAuth();
    const handleLoginFormSubmit = async () => {

        console.log('Logging in with:', email, password);
        let token = await AuthService.getToken(email, password);
        handleLogin(token)
        navigate('/');
    };

    return (
        <div className="login-container">
            <h2>Login</h2>
            <form>
                <input
                    type="email"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />
                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />
                <button type="button" onClick={handleLoginFormSubmit}>
                    Log In
                </button>
            </form>
        </div>
    );
};

export default Login;