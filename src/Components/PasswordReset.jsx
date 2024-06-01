import React, { useState } from 'react';
import './PasswordReset.css';

const PasswordReset = () => {
  const [accountName, setAccountName] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [successMessage, setSuccessMessage] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    if (newPassword === confirmPassword) {
      setSuccessMessage('Password has been successfully reset.');
    } else {
      setSuccessMessage('Passwords do not match.');
    }
  };

  return (
    <div className="password-reset-container">
      <form onSubmit={handleSubmit} className="password-reset-form">
        <div className="form-group">
          <label htmlFor="accountName">Account Name:</label>
          <input 
            type="text" 
            id="accountName" 
            value={accountName} 
            onChange={(e) => setAccountName(e.target.value)} 
            required 
          />
        </div>
        <div className="form-group">
          <label htmlFor="newPassword">New Password:</label>
          <input 
            type="password" 
            id="newPassword" 
            value={newPassword} 
            onChange={(e) => setNewPassword(e.target.value)} 
            required 
          />
        </div>
        <div className="form-group">
          <label htmlFor="confirmPassword">Confirm Password:</label>
          <input 
            type="password" 
            id="confirmPassword" 
            value={confirmPassword} 
            onChange={(e) => setConfirmPassword(e.target.value)} 
            required 
          />
        </div>
        <button type="submit" className="save-button">Save</button>
      </form>
      {successMessage && <div className="success-message">{successMessage}</div>}
    </div>
  );
};

export default PasswordReset;
