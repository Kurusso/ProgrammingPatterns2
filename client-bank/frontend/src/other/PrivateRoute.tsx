
import {useAuth} from "../contexts/AuthContext";
import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';



export const PrivateRoute = () => {
    const { isAuthenticated } = useAuth();

    return isAuthenticated ? <Outlet /> : <Navigate to="/login" />;
};

export  default PrivateRoute