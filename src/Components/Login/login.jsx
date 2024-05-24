import React, { useState } from 'react';
import './login.css';
import { FaUser } from "react-icons/fa";
import { FaLock } from "react-icons/fa";
import SignIn from '../googleSignin/signIn';



const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleUsernameChange = (event) => {
    setUsername(event.target.value);
  };

  const handlePasswordChange = (event) => {
    setPassword(event.target.value);
  };

  return (
    <div className='wrapper'>
      <form  action='' method='get'>
        <h1>Login</h1>
        <div className="input-box">
          <input type="text" id='username' name='username' value={username} onChange={handleUsernameChange} placeholder='Username' required />
          <FaUser className='icon' />
        </div>
        <div className="input-box">
          <input type="password" id="password" name="password" value={password} onChange={handlePasswordChange} placeholder='Password' required />
          <FaLock className='icon' />
        </div>

        <button type="submit">Login</button>
        <div className="remember-forgot">
          <a href="#">Forgot password?</a>
        </div>
        <div className="register-link">
          <p>Don't hava an account? <a href='./register' >Register</a></p>
        </div>
        <div className="line">
          <a>_________________________</a>
        </div>
        <div class="or">
          <a>or</a>
        </div>
        <div className='login-google'>
          <SignIn />
        </div>
      </form>
    </div>
  );
}
export default Login
