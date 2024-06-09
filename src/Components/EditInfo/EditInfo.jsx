import React, { useState, useEffect } from 'react';
import './EditInfo.css';
import Header from "../Header/header";
import Footer from "../Footer/Footer";
import { jwtDecode } from 'jwt-decode';
import { toast } from 'react-toastify';

export default function EditInfo() {
  const [userInfo, setUserInfo] = useState({
    userId: '',
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    avatar: ''
  });

  const [isLoaded, setIsLoaded] = useState(false);
  const token = sessionStorage.getItem('token');
  const decodedToken = jwtDecode(token); // Decode the JWT token to get user information
  const userIdToken = decodedToken.UserId; // Extract userId from the decoded token

  useEffect(() => {
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

  const handleSave = () => {
    fetch(`http://localhost:5266/User/Update?id=${userInfo.userId}&firstName=${userInfo.firstName}&lastName=${userInfo.lastName}&phone=${userInfo.phone}&email=${userInfo.email}`, {
      method: "PUT",
      headers: { 
        'Content-Type': 'application/json' 
      },
      body: JSON.stringify(userInfo) // Send updated userInfo as the body
    })
      .then(response => response.json())
      .then(data => {
        toast.success('Update info success.');
      })
      .catch(error => toast.waring('Update info failed,'));
  };

  
  return (
    <div className='edit-info'>
      <div className='edit-info-header'>
        <Header />
      </div>
      <div className="edit-info-wrapper">
        <div className="background"> 
          <div className="profile-container">
            <div className="profile-sidebar">
              {isLoaded && <img src={userInfo.avatar} alt="User Avatar" className="profile-avatar" />}
              <h2>{userInfo.firstName} {userInfo.lastName}</h2>
              <p>{userInfo.email}</p>
              <button className="button upload">Upload</button>
            </div>
            <div className="profile-content">
              <h2>Profile Settings</h2>
              <div className="info-items">
                <div className="info-item">
                  <label htmlFor="first-name">Enter New First Name</label>
                  <input
                    type="text"
                    id="first-name"
                    name="first-name"
                    value={userInfo.firstName}
                    placeholder={isLoaded ? userInfo.firstName : 'Enter First Name'}
                    onChange={(e) => setUserInfo({ ...userInfo, firstName: e.target.value })}
                  />
                </div>
                <div className="info-item">
                  <label htmlFor="last-name">Enter New Last Name</label>
                  <input
                    type="text"
                    id="last-name"
                    name="last-name"
                    value={userInfo.lastName}
                    placeholder={isLoaded ? userInfo.lastName : 'Enter Last Name'}
                    onChange={(e) => setUserInfo({ ...userInfo, lastName: e.target.value })}
                  />
                </div>
                <div className="info-item">
                  <label htmlFor="email">Enter New Email</label>
                  <input
                    type="email"
                    id="email"
                    name="email"
                    value={userInfo.email}
                    placeholder={isLoaded ? userInfo.email : 'Enter Email'}
                    onChange={(e) => setUserInfo({ ...userInfo, email: e.target.value })}
                  />
                </div>
                <div className="info-item">
                  <label htmlFor="phone">Enter New Phone Number</label>
                  <input
                    type="tel"
                    id="phone"
                    name="phone"
                    value={userInfo.phone}
                    placeholder={isLoaded ? userInfo.phone : 'Enter Phone Number'}
                    onChange={(e) => setUserInfo({ ...userInfo, phone: e.target.value })}
                  />
                </div>
                <div className="button-container">
                  <button className="button" onClick={handleSave}>Save Profile</button>
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
