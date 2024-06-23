import React, { useState } from 'react';
import axios from 'axios';
import './forgetpassword.css';

const ForgetPassword = () => {
  const [email, setEmail] = useState('');
  const [otp, setOtp] = useState('');
  const [otpSent, setOtpSent] = useState(false);
  const [otpVerified, setOtpVerified] = useState(false);
  const [newPassword, setNewPassword] = useState('');
  const [message, setMessage] = useState('');

  const handleSendOtp = async () => {
    try {
      const response = await axios.post('http://localhost:3005/send-otp', { email });
      if (response.status === 200) {
        setOtpSent(true);
        setMessage('OTP sent successfully. Check your email.');
      }
    } catch (error) {
      setMessage('Error sending OTP');
    }
  };

  const handleVerifyOtp = async () => {
    try {
      const response = await axios.post('http://localhost:3005/verify-otp', { email, otp });
      if (response.status === 200) {
        setOtpVerified(true);
        setMessage('OTP verified successfully. You can now reset your password.');
      }
    } catch (error) {
      setMessage('Invalid OTP');
    }
  };

  const handleResetPassword = async () => {
    try {
      const response = await axios.post('http://localhost:3005/reset-password', { email, newPassword });
      if (response.status === 200) {
        setMessage('Password updated successfully. You can now log in with your new password.');
      }
    } catch (error) {
      setMessage('Error updating password');
    }
  };

  return (
    <div className='forget-password-wrapper'>
      <h1>Forget Password</h1>
      <div className="input-box">
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder='Enter your email'
          required
        />
      </div>
      <button className='forget-button' onClick={handleSendOtp}>Send OTP</button>
      {otpSent && !otpVerified && (
        <div className="otp-box">
          <input
            type="text"
            value={otp}
            onChange={(e) => setOtp(e.target.value)}
            placeholder='Enter OTP'
            required
          />
          <button className='forget-button' onClick={handleVerifyOtp}>Verify OTP</button>
        </div>
      )}
      {otpVerified && (
        <div className="reset-password-box">
          <input
            type="password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            placeholder='Enter new password'
            required
          />
          <button className='forget-button' onClick={handleResetPassword}>Reset Password</button>
        </div>
      )}
      {message && <p>{message}</p>}
    </div>
  );
}

export default ForgetPassword;
