import React, { useState, useEffect } from "react";
import Login from "../Login/login";
import Register from "../Register/register";
import './signinsignup.css'
import { MdSportsTennis } from "react-icons/md";
import { Link } from 'react-router-dom';
 
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
                    <div className="text-element">
                        <a href="" className="logo">
                            <Link to={'/'}>                            
                            <h1><MdSportsTennis className="icon" />
                                BMTC
                            </h1></Link>
                        </a>
                        <h1>
                            BMTC Badminton Court Chain System
                        </h1>
                        <h3>Description</h3>
                        <p>Welcome to the BMTC badminton court chain system, which brings you top sports experiences and comfortable practice space. With three branches located in convenient locations, we are proud to be the top choice of the badminton-loving community in the area.</p>                        <br />
                        <h4>Branch 1: </h4>
                        <p>Address: </p>
                        <p>Phone: </p>
                        <h4>Branch 2: </h4>
                        <p>Address: </p>
                        <p>Phone: </p>
                        <h4>Branch 3: </h4>
                        <p>Address: </p>
                        <p>Phone: </p>
                        <br />
                        <h3>Contact</h3>
                        <p>Hotline:</p>
                        <p>Email:</p>
                        <p>Website:</p>
                    </div>

                    <div className="login">
                        {isLoginVisible ? (
                            <>
                                <a className="change-1" onClick={showRegister}></a>
                                <Login />
                            </>
                        ) : (
                            <>
                                <a className="change-2" onClick={showLogin}></a>
                                <Register />
                            </>
                        )}
                    </div>

                </div>
            </div>
        </div>

    );
}
export default SignInSignUp
