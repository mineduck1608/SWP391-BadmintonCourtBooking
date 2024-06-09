import React, { useState } from 'react';
import './login.css';
import { FaUser, FaLock } from "react-icons/fa";
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import SignIn from '../googleSignin/signIn';

const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();

  const proceedLogin = (e) => {
    e.preventDefault();

    const loginData = { username, password };

    if (validate()) {
      fetch("http://localhost:5266/User/LoginAuth?username=" + username + '&password=' + password, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(loginData)
      }).then((res) => res.json())
        .then((resp) => {
          if (resp.token) {
            sessionStorage.setItem('token', resp.token);
            toast.success("Login successful!");
            navigate('/admin');
          } else {
            toast.error('Invalid username or password.');
          }
        }).catch((err) => {
          toast.error('Login failed. Please try again.');
          console.log(err);
        });
    } else {
      toast.error('Please fill in all fields.');
    }
  };

  const validate = () => {
    return username && password;
  };

  return (
    <div className='wrapper'>
      <h1>Login</h1>
      <form onSubmit={proceedLogin}>
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
        <Link to={'/forget'}>Forgot password?</Link>
      </div>
      <div className="register-link">
        <p>Don't have an account? <Link to={'/signup'}>Register</Link></p>
      </div>
      <div className="line">
        <span>_________________________</span>
      </div>
      <div className="or">
        <span>or</span>
      </div>
      <div className='login-google'>
        <SignIn />
      </div>
    </div>
  );
}

export default Login;


