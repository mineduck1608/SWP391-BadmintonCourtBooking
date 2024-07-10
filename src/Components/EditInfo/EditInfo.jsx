import React, { useState, useEffect } from 'react';
import './EditInfo.css';
import Header from "../Header/header";
import Footer from "../Footer/Footer";
import { jwtDecode } from 'jwt-decode';
import { toast } from 'react-toastify';
import { imageDb } from '../googleSignin/config';
import { v4 } from 'uuid';
import { uploadBytes, getDownloadURL } from 'firebase/storage';
import { ref } from 'firebase/storage';
import { useNavigate } from 'react-router-dom';
import { fetchWithAuth } from '../fetchWithAuth/fetchWithAuth';

export default function EditInfo() {
  const navigate = useNavigate();
  const [userInfo, setUserInfo] = useState({
    userId: '',
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    img: ''
  });
  const apiUrl = 'https://localhost:7233/'
  const [img, setImg] = useState(null);
  const [imgPreview, setImgPreview] = useState('');

  const token = sessionStorage.getItem('token');
  const decodedToken = jwtDecode(token); // Decode the JWT token to get user information
  const userIdToken = decodedToken.UserId; // Extract userId from the decoded token

  useEffect(() => {
    fetchWithAuth(`https://localhost:7233/UserDetail/GetAll`, { // Fetch all user details
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
        const matchingUser = data.find(user => user.userId === userIdToken);
        if (matchingUser) {
          // Encode the image URL
          const encodedImgUrl = encodeURI(matchingUser.img);
          setUserInfo({
            ...matchingUser,
            img: encodedImgUrl
          });
          setImgPreview(matchingUser.img); // Set initial imgPreview to the current user's encoded image URL
        }
      })
      .catch(error => console.error('Error fetching user info:', error));
  }, [token]);

  const handleSave = () => {
    // Ensure userInfo.img is not empty
    const updatedUserInfo = userInfo;
    fetchWithAuth(`https://localhost:7233/User/Update?userId=${updatedUserInfo.userId}&firstName=${updatedUserInfo.firstName}&lastName=${updatedUserInfo.lastName}&phone=${updatedUserInfo.phone}&email=${updatedUserInfo.email}&img=${updatedUserInfo.img}&actorId=${updatedUserInfo.userId}`, {
      method: "PUT",
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updatedUserInfo) // Send updated userInfo as the body
    })
      .then(response => response.json())
      .then(data => {
        toast.success('Update info success.');
        navigate("/viewInfo");
      })
      .catch(error => toast.warning('Update info failed.'));
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setImg(file);
      setImgPreview(URL.createObjectURL(file));
    }
  };

  const handleClick = () => {
    if (!img) {
      toast.error('No image selected');
      return;
    }
    const imgRef = ref(imageDb, `files/${v4()}`);
    uploadBytes(imgRef, img)
      .then(() => getDownloadURL(imgRef))
      .then(url => {
        const encodedUrl = encodeURIComponent(url);
        setUserInfo(prevState => ({ ...prevState, img: encodedUrl }));
        toast.success('Image uploaded successfully');
      })
      .catch(error => {
        console.error('Error uploading image:', error);
        toast.error('Image upload failed');
      });
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
              <div className="uploaded-image-container">
                <img
                  src={imgPreview || userInfo.img}
                  alt="Uploaded"
                  className="uploaded-image"
                />
              </div>
              <input type="file" onChange={handleImageChange} />
              <button className="button upload" onClick={handleClick}>Upload</button>
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
                    placeholder='Enter First Name'
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
                    placeholder='Enter Last Name'
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
                    placeholder='Enter Email'
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
                    placeholder='Enter Phone Number'
                    onChange={(e) => setUserInfo({ ...userInfo, phone: e.target.value })}
                  />
                </div>
                <div className="button-container">
                  <button className="button-editinfo" onClick={handleSave}>Save Profile</button>
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
