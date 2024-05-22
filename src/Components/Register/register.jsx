import './register.css';
import React from 'react';

const Register = () => {
    return (
        <div className='wrapper'>
        <form action="">
          <h1>User Registeration</h1>
          <div  className="input-register">
          <div>
            <input type="text" placeholder="Username" required/>
            <input type="Password" placeholder="Password" id="password" required/>
            <input type="Password" placeholder="Confirm password" required/>
          </div>
          <div>
            <input type="text" placeholder="First name" required/>
            <input type="text" placeholder="Last name" required/>
            <input type="email" placeholder="Email" required/>
            <input type="phone" placeholder="Phone" required/>
          </div>
          </div>
         </form>
    </div>
    );
  }
  
  export default Register