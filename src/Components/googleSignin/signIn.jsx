import React, { useState, useEffect } from "react";
import { auth, provider } from "./config"; // Ensure this is the correct path to your Firebase config
import { signInWithPopup } from "firebase/auth";
import { FcGoogle } from "react-icons/fc";
import { useNavigate } from 'react-router-dom';

const SignIn = () => {
  const [value, setValue] = useState('');
  const navigate = useNavigate();

  const handleClick = async () => {
    try {
      const result = await signInWithPopup(auth, provider);
      setValue(result.user.email);
      sessionStorage.setItem("email", result.user.email);
      navigate('/'); // Navigate to /home after successful sign-in
    } catch (error) {
    }
  };

  useEffect(() => {
    const storedEmail = sessionStorage.getItem('email');
    if (storedEmail) {
      setValue(storedEmail);
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