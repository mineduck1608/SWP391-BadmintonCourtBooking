import React, { useEffect } from "react";
import { MdSportsTennis } from "react-icons/md";
import './header.css';
import user from '../../Assets/user.jpg';
import { Link, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { jwtDecode } from 'jwt-decode';

const Header = () => {
    const navigate = useNavigate();

    useEffect(() => {
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

        const { UserId: userid, Username: username, Role: role } = decodedToken;

        if (!username && !userid && !role) {
            navigate('/signin'); // Redirect to sign-in if required fields are missing
            return;
        }

        if (role !== "Customer") {
            sessionStorage.clear();
            navigate('/'); // Redirect to home if the role is not 'Customer'
            return;
        }
    }, [navigate]);

    const handleLogout = () => {
        sessionStorage.clear();
        toast.success('Logout successful.');
        navigate('/'); // Redirect to home on logout
    };
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
                <a href="" onClick={handleLogout} >Logout</a>
            </div>
        </div>
    );
}

export default Header