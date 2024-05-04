import {useAuth} from "../contexts/AuthContext";
import React from 'react';
import {Navigate, Outlet} from 'react-router-dom';
import  "../other/firebase-notifications"

export const PrivateRoute = () => {
    const {isAuthenticated} = useAuth();

    return isAuthenticated ? <Outlet/> : <Navigate to="/login"/>;
};

export default PrivateRoute