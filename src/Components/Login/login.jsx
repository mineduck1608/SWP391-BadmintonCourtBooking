import React, { useState } from 'react';
import './login.css';
import { FaUser } from "react-icons/fa";
import { FaLock } from "react-icons/fa";
import SignIn from '../googleSignin/signIn';
import { Link, useNavigate } from 'react-router-dom';



const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const usenavigate = useNavigate();

  const ProceedLogin = (e) => {
    e.preventDefault();
    if (validate()) {
      ///implentation
      ///console.log('proceed');
      fetch("http://localhost:3005/useraccount/" + username).then((res) => {
        return res.json();
      }).then((resp) => {
        if (Object.keys(resp).length === 0) {
          alert('Please Enter valid username')
        }
        else {
          if (resp.password === password) {
            sessionStorage.setItem('username', username);
            alert('Login Success');
            usenavigate('/');
          } else {
            alert('Wrong password');

          }
        }
      }).catch((err) => {
        alert('Login Failed');
      })
    }
  }
  const validate = () => {
    let result = true;
    if (username == '' || username == null) {
      result = false;
    }
    if (password == '' || password == null) {
      result = false;
    }
    return result;
  }
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
        <a href="#">Forgot password?</a>
      </div>
      <div className="register-link">
        <p>Don't hava an account? <Link to={'/signup'}>Register</Link></p>
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

    </div>
  );
}
export default Login
