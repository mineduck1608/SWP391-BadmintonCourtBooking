import React, { useState } from 'react';
import './LoginForm.css';
import { FaUser, FaRegEyeSlash } from "react-icons/fa";
import { FaLock } from "react-icons/fa";


const LoginForm = () => {
  const [profile, setProfile] = useState(null);
  return (
    <div className='wrapper'>
        <form action="">
          <h1>Login</h1>
          <div className="input-box">
            <input type="text" placeholder='Username' required/>
            <FaUser className='icon'/>
          </div>
          <div className="input-box">
            <input type="Password" placeholder='Password' id='password' required/>
            <FaLock className='icon'/>
          </div>
          <div className="remember-forgot">
            <label><input type="checkbox" />Remember me</label>
            <a href="#">Forgot password?</a>
          </div>
          <button type="submit">Login</button>
          <div className="register-link">
            <p>Don't hava an account? <a href="#">Register</a></p>
          </div>
          <div>
          
          </div>
        </form>
    </div>
    
  );
}

export default LoginForm
