import React, { useState, useEffect } from "react";
import { auth, provider } from "./config"; // Ensure this is the correct path to your Firebase config
import { signInWithPopup } from "firebase/auth";
import { FcGoogle } from "react-icons/fc";
import { useNavigate } from 'react-router-dom';
import { toast } from "react-toastify";

const SignIn = () => {
  const [value, setValue] = useState('');
  const navigate = useNavigate();

  const handleClick = async (e) => {
    const result = await signInWithPopup(auth, provider);
    e.preventDefault();
    fetch("http://localhost:5266/User/ExternalLogAuth?email=" + result.user.email, {
        method: "POST",
        headers: { 'content-type': 'application/json' },
        body: JSON.stringify(result.user.email),
      }).then((res) => {
        toast.success('Login successfuly.');
        console.log(result.user.email);
      }).catch((err) => {
        toast.error('Failed: ' + err.message);
      })

    try {
     
      setValue(result.user.email);
      sessionStorage.setItem("email", result.user.email);
      navigate('/home'); // Navigate to /home after successful sign-in
    } catch (error) {
    }
  };

  useEffect(() => {
    const storedEmail = sessionStorage.getItem('email');
    if (storedEmail) {
      setValue(storedEmail);
      navigate('/home');
    } else {
    }
  }, []);

  return (
    <div className='return'>
      {!value && (
        <div onClick={handleClick}>
          <FcGoogle className='icon' />
        </div>
      )}
    </div>
  );
};

export default SignIn;