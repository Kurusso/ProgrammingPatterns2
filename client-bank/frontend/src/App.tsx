import React, {useState} from 'react';
import {BrowserRouter, Routes, Route} from "react-router-dom";
import './App.css';
import {Home} from "./pages/Home";
import Login from "./pages/Login";
import {AuthProvider} from "./contexts/AuthContext";
import {Account} from "./pages/Account";

function App() {

    return (
        <div className="App">
            <BrowserRouter>
                <AuthProvider>
                    <Routes>
                        <Route path="/" element={<Home/>}/>
                        <Route path="login" element=<Login/>/>
                        <Route path="*" element={<Home/>}/>
                        <Route path="/account/:accountId" element=<Account/> />
                    </Routes>
                </AuthProvider>
            </BrowserRouter>
        </div>
    );
}

export default App;
