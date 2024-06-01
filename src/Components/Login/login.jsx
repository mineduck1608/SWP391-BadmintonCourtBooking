import React, { useState } from 'react';
import './login.css';
import { FaUser, FaLock } from "react-icons/fa";
import SignIn from '../googleSignin/signIn';
import { Link, useNavigate } from 'react-router-dom';
import { sendPasswordResetEmail } from 'firebase/auth';
import { auth } from '../../firebase.js'; // Adjust the import path as necessary

const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [resetEmail, setResetEmail] = useState('');
  const [message, setMessage] = useState('');

  const navigate = useNavigate();

  const ProceedLogin = (e) => {
    e.preventDefault();
    if (validate()) {
      fetch("http://localhost:3005/useraccount/" + username)
        .then((res) => res.json())
        .then((resp) => {
          if (Object.keys(resp).length === 0) {
            alert('Please enter a valid username');
          } else {
            if (resp.password === password) {
              sessionStorage.setItem('username', username);
              alert('Login Success');
              navigate('/');
            } else {
              alert('Wrong password');
            }
          }
        })
        .catch((err) => {
          alert('Login Failed');
        });
    }
  };

  const validate = () => {
    let result = true;
    if (!username) {
      result = false;
    }
    if (!password) {
      result = false;
    }
    return result;
  };

  const handlePasswordReset = () => {
    if (!resetEmail) {
      alert('Please enter your email to reset password');
      return;
    }
    sendPasswordResetEmail(auth, resetEmail)
      .then(() => {
        setMessage('Password reset email sent successfully');
        console.log('Password reset email sent successfully');
      })
      .catch((error) => {
        setMessage('Error sending password reset email: ' + error.message);
        console.error('Error sending password reset email:', error);
      });
  };

  return (
    <div className='wrapper'>
      <h1>Login</h1>
      <form onSubmit={ProceedLogin}>
        <div className="input-login-box">
          <input type="text" value={username} onChange={e => setUsername(e.target.value)} placeholder='Username' required />
          <FaUser className='icon' />
        </div>
        <div className="input-login-box">
          <input type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder='Password' required />
          <FaLock className='icon' />
        </div>
        <button type="submit">Login</button>
      </form>
      <div className="remember-forgot">
        <label style={{ cursor: 'pointer', color: 'blue' }}>
          Forgot password?
        </label>
        <div className="input-reset-box">
          <input type="email" value={resetEmail} onChange={e => setResetEmail(e.target.value)} placeholder='Enter email to reset password' />
          <button type="button" onClick={handlePasswordReset}>Reset Password</button>
        </div>
      </div>
      <div className="register-link">
        <p>Don't have an account? <Link to={'/signup'}>Register</Link></p>
      </div>
      <div className="line">
        <a>_________________________</a>
      </div>
      <div className="or">
        <a>or</a>
      </div>
      <div className='login-google'>
        <SignIn />
      </div>
      {message && <p>{message}</p>}
    </div>
  );
};

export default Login;
