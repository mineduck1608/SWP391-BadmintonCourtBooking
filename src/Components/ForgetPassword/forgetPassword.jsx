import React, {useState} from 'react';
import { sendPasswordResetEmail } from 'firebase/auth';
import { auth } from '../../firebase.js';
import './forgetpassword.css'

const ForgetPassword = () => {
    const [resetEmail, setResetEmail] = useState('');
    const [message, setMessage] = useState('');

    const handlePasswordReset = () => {
        if(!resetEmail) {
            setMessage ('Please enter your email to reset password');
            return;
        }
        sendPasswordResetEmail(auth, resetEmail)
        .then(() => {
            setMessage('Password reset email sent successfully');
        })
        .catch((error) => {
            setMessage('Error sending password reset email: ' + error.message);
        });
    };

  return (
    <div className='forget-password-wrapper'>
        <h1>Reset Password</h1>
        <div className="input-reset-box">
            <input 
            type="email"
            value={resetEmail}
            onChange={e => setResetEmail(e.target.value)}
            placeholder='Enter your email'
            required
            />
            <button type='button' onClick={handlePasswordReset}>Send Reset Email</button>
        </div>
        {message && <p>{message}</p>}
    </div>
  );
};

export default ForgetPassword;