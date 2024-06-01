import { isValidDateValue } from '@testing-library/user-event/dist/utils';
import './register.css';
import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';

const Register = () => {
  const [id, idChange] = useState("");
  const [password, passwordChange] = useState("");
  const [firstName, firstNameChange] = useState("");
  const [lastName, lastNameChange] = useState("");
  const [email, emailChange] = useState("");
  const [phone, phoneChange] = useState("");

  const usenavigate = useNavigate();

  const isValidate = () => {
    let isproceed = true;
    if(/^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[A-Za-z]+$/.test(email)){
    }else{
      isproceed =false;
      toast.warning('Please enter the valid email');
    }
    return isproceed;
  }

  const handleSubmit = (e) => {

    e.preventDefault();
    let regobj = { id, password, firstName, lastName, email, phone };
    //console.log(regobj);
    if (isValidate()) {
      fetch("http://localhost:3006/userregister", {
        method: "POST",
        headers: { 'content-type': 'application/json' },
        body: JSON.stringify(regobj),
      }).then((res) => {
        toast.success('Registered successfuly.');
        usenavigate('/signin');
      }).catch((err) => {
        toast.error('Failed: ' + err.message);
      });
    }
  }


  return (
    <div className='wrapper-register'>
      <form onSubmit={handleSubmit}>
        <h1>Register</h1>
        <div className="input-register">
          <input value={id} onChange={e => idChange(e.target.value)} className="input-box" type="text" placeholder="Username" required /><br></br>
          <input value={password} onChange={e => passwordChange(e.target.value)} className="input-box" type="Password" placeholder="Password" required /><br></br>
          <input value={firstName} onChange={e => firstNameChange(e.target.value)} className="input-box" type="text" placeholder="First name" required /><br></br>
          <input value={lastName} onChange={e => lastNameChange(e.target.value)} className="input-box" type="text" placeholder="Last name" required /><br></br>
          <input value={email} onChange={e => emailChange(e.target.value)} className="input-box" type="email" placeholder="Email" required /><br></br>
          <input value={phone} onChange={e => phoneChange(e.target.value)} className="input-box" type="phone" placeholder="Phone" required /><br></br>
          <button type="submit">Register</button><br></br>
          <p className="login-link">You already have an account?<Link to={'/signin'}>Login</Link></p>
        </div>
      </form>
    </div>
  );
}

export default Register
