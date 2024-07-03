import React, { useState } from 'react';
import './login.css';
import { FaUser, FaLock } from "react-icons/fa";
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import SignIn from '../googleSignin/signIn';
import { jwtDecode } from 'jwt-decode';

const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();

  const proceedLogin = (e) => {
    e.preventDefault();

    const loginData = { username, password };

    if (validate()) {
      fetch("https://localhost:7233/User/LoginAuth?username=" + username + '&password=' + password, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: 'include',
        body: JSON.stringify(loginData)
      }).then((res) => res.json())
        .then((resp) => {
          if (resp.token) {
            sessionStorage.setItem('token', resp.token);
            const decodedToken = jwtDecode(resp.token); // Decode the JWT token to get user information
            const roleToken = decodedToken.Role; // Extract userId from the decoded token
            const status = decodedToken.Status
            if (roleToken == "Customer") {
              if (status == 'True') {
                navigate('/home');
                toast.success("Login successful!");
              } else {
                navigate('/signin');
                toast.warning('Banned. Please contact admin.')
              }
            }
            if (roleToken == "Admin") {
              if (status == 'True') {
                navigate('/admin');
                toast.success("Login successful!");
              }
            }
          } else if(resp.msg == 'Temporaly locked'){
            toast.warning("Please contact admin")
          }
           else {
            toast.warning("Wrong username or password!")
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
        <button className='login-submit' type="submit">Login</button>
      </form>
      <div className="register-link">
        <p>Don't have an account? <Link to={'/signup'}>Register</Link></p>
      </div>
      <div className="line">
        <span>_________________________</span>
      </div>
      <div className="or">
        <span>or</span>
      </div>
      <div>
        <SignIn className='login-google' />
      </div>
    </div>
  );
}

export default Login;


