import React, { createContext, useContext, useState } from 'react';

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
        localStorage.setItem('token', JSON.stringify(token));
        setIsAuthenticated(true);
        console.log(token);
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