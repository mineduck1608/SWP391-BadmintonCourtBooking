
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
    let isProceed = true;
  
    if (!/^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[A-Za-z]+$/.test(email)) {
      isProceed = false;
      toast.warning('Please enter a valid email');
    }
  
    if (!/^\d{10}$/.test(phone)) {
      isProceed = false;
      toast.warning('Please enter a valid phone number');
    }

    if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=<>?])[A-Za-z\d!@#$%^&*()_\-+=<>?]{12,}$/.test(password)) {
      isProceed = false;
      toast.warning('Please enter a secure password. It should be at least 12 characters long and include uppercase, lowercase, digit, and special character.');
    }
  
    return isProceed;
  }

  const handleSubmit = (e) => {

    e.preventDefault();
    let regobj = { id, password, firstName, lastName, email, phone };
    //console.log(regobj);
    if (isValidate()) {
      fetch("https://localhost:7233/User/Register?username=" + id + "&password=" + password + "&firstName=" + firstName + "&lastName=" + lastName + "&email=" + email + "&phone=" + phone, {
        method: "POST",
        headers: { 'content-type': 'application/json' },
        body: JSON.stringify(regobj),
      }).then(response => response.json())
      .then((res) => {
        toast(res.msg);
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
          <button className='register-submit' type="submit">Register</button><br></br>
          <p className="login-link">You already have an account?<Link to={'/signin'}>Login</Link></p>
        </div>
      </form>
    </div>
  );
}

export default Register
