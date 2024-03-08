import React, { createContext, useContext, useState } from 'react';

interface AuthContextType {
    handleLogin: (token: string) => void;
}


const AuthContext = createContext<AuthContextType | undefined>(undefined);
interface AuthProviderProps {
    children: React.ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {

    const handleLogin = (token: string) => {
        localStorage.setItem('token',JSON.stringify(token))
        console.log(token);
    };

    return (
        <AuthContext.Provider value = {{ handleLogin }}>
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