import React, { useEffect, useState } from 'react';
import './login.css';
import { FaUser, FaLock } from "react-icons/fa";
import SignIn from '../googleSignin/signIn';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';


const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  
  const usenavigate = useNavigate();

  useEffect(() => {
    sessionStorage.clear();
  }, []);


  const ProceedLogin = (e) => {
    e.preventDefault();

    const loginData = {
      username: username,
      password: password
    };

    if (validate()) {
      ///implentation
      fetch("http://localhost:5266/User/LoginAuth?id=" + username + '&password=' + password, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(loginData)
      }).then((res) => {
        return res.json();
      }).then((resp) => {
        if (Object.keys(resp).length === 0) {
          toast.error('Please enter valid username.');
        }
        else {
          if (resp.password === password) {
            sessionStorage.setItem('username', username);
            toast.success("Success");
            usenavigate('/home');
          } else {
            toast.error("Wrong username or password.");
            console.log('2');
            console.log(resp.password);
          }
        }
      })
      .catch((err) => {
        toast.error('Wrong username or password.');
        console.log('1');

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
         <a>  <Link to={'/forget'}>Forgot password?</Link></a>
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
    </div>
  );
}
export default Login
