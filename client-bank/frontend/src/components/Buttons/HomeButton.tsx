import {useNavigate} from "react-router-dom";
import React from "react";

export const HomeButton = () => {

    const navigate = useNavigate();

    return (
        <div>
            <button id={"home-btn"} type="button" onClick={()=>navigate("/")}>
                Home
            </button>
        </div>
    );
};