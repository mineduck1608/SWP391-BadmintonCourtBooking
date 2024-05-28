import React from "react";
import { MdSportsTennis } from "react-icons/md";
import './header.css';
import user from '../../Assets/user.jpg';

const Header = () => {
    return (
        <div className="header">
            <a href="./">
                <h1 className='logo-header'><MdSportsTennis className="icon" />BMTC</h1>
            </a>
            <a href="./viewInfo" className="user-pic">
                <img src={user} alt="Error" />
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