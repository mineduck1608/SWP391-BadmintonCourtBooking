import React, { useState, useEffect } from "react";
import Login from "../Login/login";
import Register from "../Register/register";
import './signinsignup.css'
import { MdSportsTennis } from "react-icons/md";
import { Link } from 'react-router-dom';
import { ToastContainer } from "react-toastify";
 
const SignInSignUp = ({ defaultLoginVisible }) => {
    const [isLoginVisible, setIsLoginVisible] = useState(defaultLoginVisible);

    useEffect(() => {
        setIsLoginVisible(defaultLoginVisible);
    }, [defaultLoginVisible]);

    const showLogin = () => {
        setIsLoginVisible(true);
    };

    const showRegister = () => {
        setIsLoginVisible(false);
    };
    return (
        <div>
            <div className="body">
                <div className="body-element">
                    <div className="signin-text-element">
                            <Link to={'/'} className="logo">                            
                            <h1 className="signin-h1"><MdSportsTennis className="icon" />
                                BMTC
                            </h1></Link>
                        <h1 className="signin-h1">
                            BMTC Badminton Court Chain System
                        </h1>
                        <h2 className="signin-h2">Description</h2>
                        <p className="signin-p">Welcome to the BMTC badminton court chain system, which brings you top sports experiences and comfortable practice space. With three branches located in convenient locations, we are proud to be the top choice of the badminton-loving community in the area.</p>                        <br />
                        <h3 className="signin-h3">Branch 1: </h3>
                        <p className="signin-p">Address: </p>
                        <p className="signin-p">Phone: </p>
                        <h3 className="signin-h3">Branch 2: </h3>
                        <p className="signin-p">Address: </p>
                        <p>Phone: </p>
                        <h3 className="signin-h3">Branch 3: </h3>
                        <p className="signin-p">Address: </p>
                        <p className="signin-p">Phone: </p>
                        <br />
                        <h3 className="signin-h3">Contact</h3>
                        <p className="signin-p">Hotline:</p>
                        <p className="signin-p">Email:</p>
                        <p className="signin-p">Website:</p>
                    </div>

                    <div className="login">
                        {isLoginVisible ? (
                            <>
                                <a className="signin-a" onClick={showRegister}></a>
                                <Login />
                            </>
                        ) : (
                            <>
                                <a className="signin-a" onClick={showLogin}></a>
                                <Register />
                            </>
                        )}
                    </div>

                </div>
            </div>
            <ToastContainer />
        </div>

    );
}
export default SignInSignUp
