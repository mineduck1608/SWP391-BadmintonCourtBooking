import React, { useState } from 'react';
import './login.css';
import { FaUser } from "react-icons/fa";
import { FaLock } from "react-icons/fa";
import SignIn from '../googleSignin/signIn';
import { Link } from 'react-router-dom';



const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleUsernameChange = (event) => {
    setUsername(event.target.value);
  };

  const handlePasswordChange = (event) => {
    setPassword(event.target.value);
  };
  const login = () => {
    var username = document.getElementById("username").value;
    var password = document.getElementById("password").value;
    var data = new FormData();
    data.append("username", username);
    data.append("password", password);
    fetch("https://localhost:7011/api/Badminton/Authorize", {
      method: "POST",
      body: data
    }).then(res => res.json())
      .then(data => {
        alert(data["UserName"]);
        // code de redirect
        //data["RoleID"]
      });
  }
  return (
    <div className='wrapper'>
      <h1>Login</h1>
      <div className="input-login-box">
        <input type="text" id='username' name='username' value={username} onChange={handleUsernameChange} placeholder='Username' required />
        <FaUser className='icon' />
      </div>
      <div className="input-login-box">
        <input type="password" id="password" name="password" value={password} onChange={handlePasswordChange} placeholder='Password' required />
        <FaLock className='icon' />
      </div>

      <button type="submit" onClick={login}>Login</button>
      <div className="remember-forgot">
        <a href="#">Forgot password?</a>
      </div>
      <div className="register-link">
        <p>Don't hava an account? <a href='' ><Link to={'/signup'}>Register</Link></a></p>
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
    </div>
  );
}
export default Login
