import React, { useState } from 'react';
import axios from 'axios';
import './forgetpassword.css';

const ForgetPassword = () => {
  const [email, setEmail] = useState('');
  const [otp, setOtp] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [otpSent, setOtpSent] = useState(false);
  const [otpError, setOtpError] = useState('');
  const [resetError, setResetError] = useState('');

  const handleSendOtp = async () => {
    try {
      const response = await axios.post('http://localhost:3005/send-otp', { email });
      setOtpSent(true);
      alert(response.data.message);
    } catch (error) {
      setOtpError('Error sending OTP. Please try again.');
    }
  };

  const handleResetPassword = async () => {
    try {
      const response = await axios.post('http://localhost:3005/reset-password', {
        email,
        otp,
        newPassword,
      });
      alert(response.data.message);
      if (response.data.message === 'Password reset successful') {
        // Redirect or perform any further action after successful password reset
      }
    } catch (error) {
      setResetError('Error resetting password. Please try again.');
    }
  };

  return (
    <div className="forget-password-wrapper">
      <h2>Forget Password</h2>
      {!otpSent ? (
        <>
          <input
            type="email"
            placeholder="Enter your email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <button onClick={handleSendOtp}>Send OTP</button>
          {otpError && <p className="error-message">{otpError}</p>}
        </>
      ) : (
        <>
          <input
            type="text"
            placeholder="Enter OTP"
            value={otp}
            onChange={(e) => setOtp(e.target.value)}
          />
          <input
            type="password"
            placeholder="Enter new password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
          />
          <button onClick={handleResetPassword}>Reset Password</button>
          {resetError && <p className="error-message">{resetError}</p>}
        </>
      )}
    </div>
  );
};

export default ForgetPassword;
