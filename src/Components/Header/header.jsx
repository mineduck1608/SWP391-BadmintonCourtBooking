import React, { useEffect, useState } from "react";
import { MdSportsTennis, MdMenu } from "react-icons/md";
import './header.css';
import userImg from '../../Assets/user.jpg';
import { Link, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { jwtDecode } from 'jwt-decode'; // Import jwt-decode without curly braces

const Header = () => {
    const navigate = useNavigate();
    const [dropdownOpen, setDropdownOpen] = useState(false);
    const [userImage, setUserImage] = useState(userImg);
    const [username, setUsername] = useState("");

    useEffect(() => {
        let path = window.location.pathname;
        if (path === '/paySuccess') return;

        let token = sessionStorage.getItem('token');
        if (!token) {
            navigate('/signin'); // Redirect to home if token is not present
            return;
        }

        let decodedToken;
        try {
            decodedToken = jwtDecode(token); // Decode the JWT token to get user information
        } catch (error) {
            sessionStorage.clear();
            navigate('/'); // Redirect to home if token is invalid
            return;
        }

        const { UserId: userId, Username: userName, Role: role } = decodedToken;

        if (!userName && !userId && !role) {
            navigate('/signin'); // Redirect to sign-in if required fields are missing
            return;
        }

        if (role !== "Customer") {
            sessionStorage.clear();
            navigate('/'); // Redirect to home if the role is not 'Customer'
            return;
        }

        setUsername(userName);
        fetchUserImage(userId);

    }, [navigate]);

    const fetchUserImage = async (userId) => {
        const userDetailsUrl = `https://localhost:7233/UserDetail/GetById?id=${userId}`;
        
        try {
            const response = await fetch(userDetailsUrl);
            if (response.ok) {
                const userDetails = await response.json();
                if (userDetails.img) {
                    setUserImage(userDetails.img);
                }
            }
        } catch (error) {
            console.error('Failed to fetch user details', error);
        }
    };

    const handleLogout = () => {
        sessionStorage.clear();
        toast.success('Logout successful.');
        navigate('/'); // Redirect to home on logout
    };

    const toggleDropdown = () => {
        setDropdownOpen(!dropdownOpen);
    };

    return (
        <div className="header-component">
            <Link to={'/home'}>
                <h1 className='logo-header'><MdSportsTennis className="icon" />BMTC</h1>
            </Link>
            <a href="" className="user-pic">
                <Link to={'/viewInfo'}>
                    <img src={userImage || userImg} alt="User" />
                </Link>
            </a>
            <div className="dropdown">
                <button className="dropdown-toggle" onClick={toggleDropdown}>
                    <MdMenu className="menu-icon" />
                </button>
                <div className={`text-header ${dropdownOpen ? 'show' : ''}`}>
                    <Link to={'/home'} className="header-link">Home</Link>
                    <Link to={'/bookingHistory'} className="header-link">Booking</Link>
                    <Link to={'/buyTime'} className="header-link">Time Balance</Link>
                    <a className="header-link time-balance" onClick={handleLogout}>Logout</a>
                </div>
            </div>
        </div>
    );
}

export default Header;
