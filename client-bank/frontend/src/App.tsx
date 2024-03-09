import React from 'react';
import {BrowserRouter, Routes, Route} from "react-router-dom";
import './App.css';
import {Home} from "./pages/Home";
import Login from "./pages/Login";
import {AuthProvider} from "./contexts/AuthContext";
import {Account} from "./pages/Account";
import PrivateRoute from "./other/PrivateRoute";
import {UserProvider} from "./contexts/UserContext";

function App() {

    return (
        <div className="App">
            <BrowserRouter>
                <AuthProvider>
                    <UserProvider>
                        <Routes>
                            <Route path='/' element={<PrivateRoute/>}>
                                <Route path="/" element={<Home/>}/>
                            </Route>
                            <Route path="*" element={<Home/>}/>
                            <Route path="/account/:accountId" element={<Account/>}/>
                            <Route path="login" element={<Login/>}/>
                        </Routes>
                    </UserProvider>
                </AuthProvider>
            </BrowserRouter>
        </div>
    );
}

export default App;
