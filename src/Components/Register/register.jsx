import './register.css';
import React from 'react';

const Register = () => {
    return (
        <div className='wrapper-register'>
        <form action="">
          <h1>Register</h1>
          <div  className="input-register">
            <input className="input-box" type="text" placeholder="Username" required/><br></br>
            <input className="input-box" type="Password" placeholder="Password" id="password" required/><br></br>
            <input className="input-box" type="text" placeholder="First name" required/><br></br>
            <input className="input-box" type="text" placeholder="Last name" required/><br></br>
            <input className="input-box" type="email" placeholder="Email" required/><br></br>
            <input className="input-box" type="phone" placeholder="Phone" required/><br></br>
            <button type="submit">Register</button><br></br>
            <p className="login-link">You already have an account?<a href="/login">Login</a></p>
            
          </div>
         </form>
    </div>
    );
  }
  
  export default Register
