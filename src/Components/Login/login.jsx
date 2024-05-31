import React, { useState } from 'react';
import './login.css';
import { FaUser } from "react-icons/fa";
import { FaLock } from "react-icons/fa";
import SignIn from '../googleSignin/signIn';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';



const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const usenavigate = useNavigate();

  const ProceedLogin = (e) => {
    e.preventDefault();
    if (validate()) {
      ///implentation
      ///console.log('proceed');
      fetch("http://localhost:3004/useraccount/" + username).then((res) => {
        return res.json();
      }).then((resp) => {
        console.log(resp)
        if (Object.keys(resp).length == 0) {
          console.log('1');
        }
        else {
          if (resp.password === password) {
            console.log('2');
            usenavigate('/');
          } else {
                    toast.warning('error');

          }
        }
      }).catch((err) => {
        console.log('4');

        toast.warning('error');
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
