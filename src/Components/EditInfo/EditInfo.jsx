import React, { useState, useEffect } from 'react';
import './EditInfo.css';
import Header from "../Header/header";
import Footer from "../Footer/Footer";

export default function EditInfo() {
  const [userInfo, setUserInfo] = useState({
    firstName: '',
    lastName: '',
    birthDate: '',
    email: '',
    phone: '',
    avatar: ''
  });

  useEffect(() => {
    fetch('/api/user/123')
      .then(response => response.json())
      .then(data => {
        setUserInfo({
          firstName: data.firstName,
          lastName: data.lastName,
          birthDate: data.birthDate,
          email: data.email,
          phone: data.phone,
          avatar: data.avatar
        });
      })
      .catch(error => console.error('Error fetching user data:', error));
  }, []);

  return (
    <div className='edit-info'>
      <div className='edit-info-header'>
        <Header />
      </div>
      <div className="edit-info-wrapper">
        <div className="background"> 
          <div className="profile-container">
            <div className="profile-sidebar">
              <img src={userInfo.avatar} alt="User Avatar" className="profile-avatar" />
              <h2>{userInfo.firstName} {userInfo.lastName}</h2>
              <p>{userInfo.email}</p>
              <button className="button upload">Upload</button>
            </div>
            <div className="profile-content">
              <h2>Profile Settings</h2>
              <div className="info-items">
                <div className="info-item">
                  <label htmlFor="first-name">First Name</label>
                  <input
                    type="text"
                    id="first-name"
                    name="first-name"
                    value={userInfo.firstName}
                    placeholder="Enter First Name"
                    onChange={(e) => setUserInfo({ ...userInfo, firstName: e.target.value })}
                  />
                </div>
                <div className="info-item">
                  <label htmlFor="last-name">Last Name</label>
                  <input
                    type="text"
                    id="last-name"
                    name="last-name"
                    value={userInfo.lastName}
                    placeholder="Enter Last Name"
                    onChange={(e) => setUserInfo({ ...userInfo, lastName: e.target.value })}
                  />
                </div>
                <div className="info-item">
                  <label htmlFor="phone">Phone Number</label>
                  <input
                    type="tel"
                    id="phone"
                    name="phone"
                    value={userInfo.phone}
                    placeholder="Enter Phone Number"
                    onChange={(e) => setUserInfo({ ...userInfo, phone: e.target.value })}
                  />
                </div>
                <div className="info-item">
                  <label htmlFor="email">Email ID</label>
                  <input
                    type="email"
                    id="email"
                    name="email"
                    value={userInfo.email}
                    placeholder="Enter Email"
                    onChange={(e) => setUserInfo({ ...userInfo, email: e.target.value })}
                  />
                </div>
                <div className="info-item">
                  <label htmlFor="address">Address</label>
                  <input
                    type="text"
                    id="address"
                    name="address"
                    value={userInfo.address}
                    placeholder="Enter Address"
                    onChange={(e) => setUserInfo({ ...userInfo, address: e.target.value })}
                  />
                </div>
                <div className="button-container">
                  <button className="button">Save Profile</button>
                </div>
              </div>
            </div>
          </div>
        </div>
        </div>
      <div className='edit-info-footer'>
        <Footer />
      </div>
    </div>
  );
}
