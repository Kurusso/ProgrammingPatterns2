import React from "react";
import {useNavigate} from "react-router-dom";

export const LogoutButton = () => {

    const navigate = useNavigate();
    const handleLogout = () => {
        localStorage.removeItem('token');
    navigate("/login")
    }
    return (<div>
        <button id={"logout-btn"} type="button" onClick={handleLogout}>
            Logout
        </button>
    </div>);
};