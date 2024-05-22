import React from 'react';
import './login.css';
import { FaUser, FaRegEyeSlash } from "react-icons/fa";
import { FaLock } from "react-icons/fa";
import SignIn from '../googleSignin/signIn';
import { Link } from 'react-router-dom';



const Login = () => {
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
          
          <button type="submit">Login</button>
          <div className="remember-forgot">
            <a href="#">Forgot password?</a>
          </div>
          <div className="register-link">
            <p>Don't hava an account?</p>
            
          </div>
          <div className="line">
            <a>_________________________________________</a>
          </div>
          <div class="or">
          <a>or</a>
          </div>
          <div className='login-google'>
             <SignIn/>
          </div>
        </form>
    </div>
    
  );
}

export default Login
