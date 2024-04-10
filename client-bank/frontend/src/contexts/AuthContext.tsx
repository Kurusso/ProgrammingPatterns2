import React, { createContext, useContext, useState } from 'react';
import {setAccessToken} from "../api/auth";

interface AuthProviderProps {
    children: React.ReactNode;
}

interface AuthContextType {
    isAuthenticated: boolean;
    handleLogin: (token: string) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(() => {
        const token = localStorage.getItem('token');
        return !!token;
    });

    const handleLogin = (token: string) => {
        setAccessToken(token)
        setIsAuthenticated(true);
    };

    return (
        <AuthContext.Provider value={{ isAuthenticated, handleLogin }}>
            {children}
        </AuthContext.Provider>
    );
};


export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};