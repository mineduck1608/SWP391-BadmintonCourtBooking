import React, { useState, useEffect } from 'react';
import './ViewInfo.css';
import { Link } from 'react-router-dom';

export default function ViewInfo() {
  const [userInfo, setUserInfo] = useState({
    userId: '',
    firstName: '',
    lastName: '',
    email: '',
    phone: ''
  });

  useEffect(() => {
    fetch("http://localhost:5266/UserDetail/GetAll", {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    })
      .then(response => response.json())
      .then(data => setUserInfo(data))
      .catch(error => console.error('Lỗi khi lấy thông tin người dùng:', error));
  }, []);

  return (
    <div className="profile-container">
      <div className="profile-sidebar">
        <img src={userInfo.user} alt="User Avatar" className="profile-avatar" />
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
        <div className="history-box">
          <div className="black-box">
            <p className="history-text">History</p>
          </div>
        </div>
      </div>
    </div>
  );
}
