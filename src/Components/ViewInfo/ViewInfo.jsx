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
    img: ''
  });
  const token = sessionStorage.getItem('token');

  useEffect(() => {
    if (!token) {
      console.error('Token không tìm thấy. Vui lòng đăng nhập.');
      return;
    }

    const decodedToken = jwtDecode(token); // Giải mã JWT token để lấy thông tin người dùng
    const userIdToken = decodedToken.UserId; // Trích xuất userId từ token đã giải mã

    fetch(`http://localhost:5266/UserDetail/GetAll`, { // Lấy tất cả thông tin người dùng
      method: "GET",
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    })
    .then(response => {
      if (!response.ok) {
        throw new Error('Lấy thông tin người dùng thất bại');
      }
      return response.json();
    })
    .then((data) => {
      // Tìm người dùng với userId khớp
      const matchingUser = data.find(user => user.userId == userIdToken);
      if (matchingUser) {
        setUserInfo(matchingUser);
        console.log('Fetched user info:', matchingUser); // Log fetched user info
        console.log('Image URL:', matchingUser.img); // Log the image URL
      }
    })
    .catch(error => console.error('Lỗi khi lấy thông tin người dùng:', error));
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
                src={userInfo.img || 'default-avatar.png'} // Use a default image if img is empty
                alt="User Avatar" 
                className="profile-avatar" 
                onError={(e) => e.target.src = 'default-avatar.png'} // Fallback if image fails to load
                style={{ maxWidth: '100%', height: 'auto' }} // Ensure the image is responsive
              />
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
