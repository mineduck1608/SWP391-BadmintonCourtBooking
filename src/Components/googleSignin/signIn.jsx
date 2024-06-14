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
    const user = result.user;
    e.preventDefault();
    fetch("http://localhost:5266/User/ExternalLogAuth?token=" + result.user.getIdToken(), {
        method: "POST",
        headers: { 'content-type': 'application/json' },
        body: JSON.stringify(result.user.getIdToken()),
      }).then((res) => {
        toast.success('Login successfuly.');
      }).catch((err) => {
        toast.error('Failed: ' + err.message);
      })

    try {
     
      setValue(result.user.getIdToken());
      sessionStorage.setItem("token", result.user.getIdToken());
      navigate('/home'); // Navigate to /home after successful sign-in
    } catch (error) {
    }
  };

  useEffect(() => {
    const storedEmail = sessionStorage.getItem('token');
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