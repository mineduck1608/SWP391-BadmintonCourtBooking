import React, { useState, useEffect } from "react";
import { auth, provider } from "./config"; // Ensure this path is correct to your Firebase config
import { signInWithPopup } from "firebase/auth";
import { FcGoogle } from "react-icons/fc";
import { useNavigate } from 'react-router-dom';
import { toast } from "react-toastify";

const SignIn = () => {
  const [value, setValue] = useState('');
  const navigate = useNavigate();

  const handleClick = async (e) => {
    e.preventDefault();
    try {
      const result = await signInWithPopup(auth, provider);
      const token = await result.user.getIdToken();
      const response = await fetch("https://localhost:7233/User/ExternalLogAuth?token=" + token, {
        method: "POST",
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ token: token }), // Convert token to object
      });

      if (response.ok) {
        const data = await response.json(); // Assuming the response contains JSON data
        const returnedToken = data.token; // Adjust based on your API response structure
        sessionStorage.setItem("token", returnedToken);
        console.log(returnedToken)
        setValue(returnedToken);
        toast.success('Login successfully.');
        navigate('/home'); // Navigate to /home after successful login
      } else {
        toast.error('Failed to log in.');
      }
    } catch (error) {
      toast.error('Failed: ' + error.message);
    }
  };

  useEffect(() => {
    const storedToken = sessionStorage.getItem('token');
    if (storedToken) {
      setValue(storedToken);
      navigate('/home');
    }
  }, [navigate]);

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
