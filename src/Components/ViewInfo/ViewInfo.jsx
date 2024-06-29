import React, { useState, useEffect } from 'react';
import './ViewInfo.css';
import { Link } from 'react-router-dom';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import { jwtDecode } from 'jwt-decode'; 
import userImg from '../../Assets/user.jpg';

export default function ViewInfo() {
  const [userInfo, setUserInfo] = useState({
    userId: '',
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    img: ''
  });
  const token = sessionStorage.getItem('token');

  useEffect(() => {
    if (!token) {
      return;
    }

    const decodedToken = jwtDecode(token); 
    const userIdToken = decodedToken.UserId; 

    fetch(`https://localhost:7233/UserDetail/GetAll`, { 
      method: "GET",
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    })
    .then(response => {
      if (!response.ok) {
        throw new Error('Error fetch user');
      }
      return response.json();
    })
    .then((data) => {
      const matchingUser = data.find(user => user.userId === userIdToken);
      if (matchingUser) {
        setUserInfo(matchingUser);
      }
    })
    .catch(error => console.error('Error fetch user:', error));
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
              <img 
                src={userInfo.img || userImg} // Use a default image if img is empty
                alt="User Avatar" 
                className="profile-avatar" 
                onError={(e) => e.target.src = userImg} // Fallback if image fails to load
              />
            </div>
            <div className="profile-content">
              <h2>Profile Settings</h2>
              <div className="info-box">
                <div className="info-items">
                  <div className="info-item">
                    <label>First Name</label>
                    <div>{userInfo.firstName || 'Please enter first name'}</div>
                  </div>
                  <div className="info-item">
                    <label>Last Name</label>
                    <div>{userInfo.lastName || 'Please enter last name'}</div>
                  </div>
                  <div className="info-item">
                    <label>Email</label>
                    <div>{userInfo.email || 'Please enter email'}</div>
                  </div>
                  <div className="info-item">
                    <label>Phone</label>
                    <div>{userInfo.phone || 'Please enter phone number'}</div>
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
