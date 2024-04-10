import {useNavigate} from "react-router-dom";
import React from "react";

export const UserPenaltiesButton = () => {
    const navigate = useNavigate();

    return (
        <div>
            <button id={"user-penalties-btn"} type="button" onClick={()=>navigate("/penalties")}>
                User Penalties
            </button>
        </div>
    );
};