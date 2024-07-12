import React, { useState, useEffect } from 'react';
import './forgetpassword.css';
import Navbar from '../Navbar/Navbar';
import { fetchWithAuth } from '../fetchWithAuth/fetchWithAuth';


const ResetPassword = () => {
  const [userId, setUserId] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [message, setMessage] = useState('');

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const userId = params.get('id');
    if (userId) {
      setUserId(userId);
    }
  }, []);

  const handleResetPassword = async () => {
    if (newPassword !== confirmPassword) {
      setMessage('Passwords do not match. Please try again.');
      return;
    }

    try {
      const response = await fetch(`https://localhost:7233/User/ForgotPassReset?id=${userId}&password=${newPassword}&confirmPassword=${confirmPassword}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userId, newPassword, confirmPassword })
      });

      if (response.ok) {
        setMessage('Password updated successfully. You can now log in with your new password.');
        setTimeout(() => {
          window.location.href = '/signin';
        }, 2000);
      } else {
        const responseData = await response.json();
        setMessage(responseData.msg);
      }
    } catch (error) {
      setMessage('Error updating password. Please try again.');
    }
  };

  return (
    <>
      <div><Navbar /></div>
      <div className='reset-password-wrapper'>
        <div className="reset-password-container">
          <h1 className='reset-password-title'>Reset Password</h1>
          <div className="reset-password-input-box">
            <i className="reset-password-input-icon fas fa-lock"></i>
            <input
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              className="reset-password-input"
              placeholder='Enter new password'
              required
            />
          </div>
          <div className="reset-password-input-box">
            <i className="reset-password-input-icon fas fa-lock"></i>
            <input
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              className="reset-password-input"
              placeholder='Confirm new password'
              required
            />
          </div>
          <button className='reset-password-button' onClick={handleResetPassword}>Reset Password</button>
          {message && <p className="forget-password-message">{message}</p>}
        </div>
      </div>
    </>
  );
}

export default ResetPassword;
