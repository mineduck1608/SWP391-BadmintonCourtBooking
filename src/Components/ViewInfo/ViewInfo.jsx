import React, { useState, useEffect } from 'react';
import './ViewInfo.css';
import { Link } from 'react-router-dom';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import { jwtDecode } from 'jwt-decode';

export default function ViewInfo() {
  const [userInfo, setUserInfo] = useState({
    userId: '',
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    user: ''
  });
  const token = sessionStorage.getItem('token');

  useEffect(() => {
    if (!token) {
      console.error('Token not found. Please log in.');
      return;
    }

    const decodedToken = jwtDecode(token); // Decode the JWT token to get user information
    const userIdToken = decodedToken.UserId; // Extract userId from the decoded token


    fetch(`http://localhost:5266/UserDetail/GetAll`, { // Fetch all user details
      method: "GET",
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    })
    .then(response => {
      if (!response.ok) {
        throw new Error('Failed to fetch user info');
      }
      return response.json();
    })
    .then((data) => {
      // Find user with matching userId
      const matchingUser = data.find(user => user.userId == userIdToken);
      if (matchingUser) {
        setUserInfo(matchingUser);
      }
    })
    .catch(error => console.error('Error fetching user info:', error));
  }, [token]);

  return (
    <div className='view-info'>
      <div className='view-info-header'>
        <Header />
      </div>
      <div className='view-info-wrapper'>
        <div className='background'>
          <div className="profile-container1">
            <div className="profile-sidebar">
              <img src={userInfo.avatar} alt="User Avatar" className="profile-avatar" />
              <h2>{userInfo.firstName} {userInfo.lastName}</h2>
              <p>{userInfo.email}</p>
            </div>
            <div className="profile-content">
              <h2>Profile Settings</h2>
              <div className="info-box">
                <div className="info-items">
                  <div className="info-item">
                    <label>First Name</label>
                    <div>{userInfo.firstName}</div>
                  </div>
                  <div className="info-item">
                    <label>Last Name</label>
                    <div>{userInfo.lastName}</div>
                  </div>
                  <div className="info-item">
                    <label>Email</label>
                    <div>{userInfo.email}</div>
                  </div>
                  <div className="info-item">
                    <label>Phone</label>
                    <div>{userInfo.phone}</div>
                  </div>
                </div>
                <div className="button-container">
                  <button className="button"><Link to={'/editInfo'}>Edit</Link></button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className='view-info-footer'>
        <Footer />
      </div>
    </div>
  );
}
