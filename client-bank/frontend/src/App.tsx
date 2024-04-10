import React from 'react';
import {BrowserRouter, Routes, Route} from "react-router-dom";
import './App.css';
import {Home} from "./pages/Home";
import Login from "./pages/Login";
import {AuthProvider} from "./contexts/AuthContext";
import {Account} from "./pages/Account";
import PrivateRoute from "./other/PrivateRoute";
import {UserProvider} from "./contexts/UserContext";
import {Credit} from "./pages/Credit";
import {AuthProcessing} from "./pages/AuthProcessing";
import {UserPenalties} from "./pages/UserPenalties";

function App() {

    return (
        <div className="App">
            <BrowserRouter>
                <AuthProvider>
                    <UserProvider>
                        <Routes>
                            <Route path="login" element={<Login/>}/>
                            <Route path="auth" element={<AuthProcessing/>}/>
                            <Route path='/' element={<PrivateRoute/>}>
                                <Route path="/" element={<Home/>}/>
                                <Route path="*" element={<Home/>}/>
                                <Route path="/penalties" element={<UserPenalties/>}/>
                                <Route path="/account/:accountId" element={<Account/>}/>
                                <Route path="/credit/:creditId" element={<Credit/>}/>
                            </Route>
                        </Routes>
                    </UserProvider>
                </AuthProvider>
            </BrowserRouter>
        </div>
    );

}

export default App;
