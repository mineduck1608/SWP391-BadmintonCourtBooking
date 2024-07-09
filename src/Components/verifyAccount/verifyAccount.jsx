import React, { useState } from 'react';
import './verifyAccount.css';
import Navbar from '../Navbar/Navbar';
import { fetchWithAuth } from '../fetchWithAuth/fetchWithAuth';

const VerifyAccount = () => {
  const [email, setEmail] = useState('');
  const [message, setMessage] = useState('');

  const handleSendResetLink = async () => {
    try {
      const response = await fetchWithAuth(`https://localhost:7233/User/VerifyBeforeReset?mail=${email}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email })
      });

      if (response.ok) {
        const responseData = await response.json();
        setMessage(responseData.msg);
      } else {
        const responseData = await response.json();
        setMessage(responseData.msg);
      }
    } catch (error) {
      setMessage('Error sending reset link. Please check your email and try again.');
    }
  };

  return (
    <>
    <div><Navbar/></div>
    <div className='verifyaccount-background'>
    <div className='verifyaccount-wrapper'>
      <h1>Forget Password</h1>
      <div className="verifyaccount-input-box">
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="verifyaccount-input"
          placeholder='Enter your email'
          required
        />
      </div>
      <button className='verifyaccount-button' onClick={handleSendResetLink}>Send Reset Link</button>
      {message && <p className="verifyaccount-message">{message}</p>}
    </div>
    </div>
    </>
  );
}

export default VerifyAccount;
