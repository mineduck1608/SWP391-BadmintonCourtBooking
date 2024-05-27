import React, { useState, useEffect } from 'react';
import './ViewInfo.css';

export default function ViewInfo() {
  const [userInfo, setUserInfo] = useState({
    firstName: 'Quynh',
    lastName: 'Tran',
    birthDate: '1990-01-01',
    email: 'quynh@123.com',
    phone: '123456789',
    avatar: 'path'
  });

  useEffect(() => {
    // Uncomment and replace the URL below with your actual API endpoint
    // const apiEndpoint = 'https://api.example.com/user-info';

    // Fetch user information from the API
    // fetch(apiEndpoint)
    //   .then(response => response.json())
    //   .then(data => setUserInfo(data))
    //   .catch(error => console.error('Error fetching user info:', error));
  }, []);

  return (
    <div className="profile-container">
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
              <label>Birth Date</label>
              <div>{userInfo.birthDate}</div>
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
            <button className="button">Edit</button>
          </div>
        </div>
        <div className="history-box">
          <div className="black-box">
            <p className="history-text">History</p>
          </div>
        </div>
      </div>
    </div>
  );
}
