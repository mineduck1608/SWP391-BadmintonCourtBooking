import React, { useEffect } from "react";
import { MdSportsTennis } from "react-icons/md";
import './header.css';
import user from '../../Assets/user.jpg';
import { Link, useNavigate } from "react-router-dom";

const Header = () => {
    const usenavigate = useNavigate();
    useEffect(() => {
        let username = sessionStorage.getItem('username');
        if(username === '' || username === null){
            usenavigate('/signin');
        }
    }, []);
    return (
        <div className="header">
            <Link to={'/'}>
                <h1 className='logo-header'><MdSportsTennis className="icon" />BMTC</h1>
            </Link>
            <a href="" className="user-pic">
                <Link to={'/viewInfo'}>
                <img src={user} alt="" />
                </Link>
            </a>
            <div className="text-header">
                <a href="./">Home</a>
                <a href="">Booking</a>
                <a className="long" href="">Time Balance</a>
                <a href="">Logout</a>
            </div>
        </div>
    );
}

export default Header