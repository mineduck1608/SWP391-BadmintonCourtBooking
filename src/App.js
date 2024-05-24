import React from 'react';
import logo from './logo.svg';
import './App.css';
import Login from './Components/Login/login';
import Register from './Components/Register/register';
import Navbar from './Components/Navbar/Navbar';
import Home from './Components/Home/Home';
import Footer from'./Components/Footer/Footer';
import Popular from './Components/Popular/Popular';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import SignInSignUp from './Components/SigninSignUp/signinsignup';

const router = createBrowserRouter([
  {
    path: '/',
    element: <div><Navbar/><Home/><Popular/><Footer/></div>
  },
  {
    path: '/signin',
    element: <div><SignInSignUp defaultLoginVisible={true} /></div>
  },
  {
    path: '/signup',
    element: <div><SignInSignUp defaultLoginVisible={false} /></div>
  },
])
 
const App = () => {
  return (
    <div>
      <RouterProvider router={router}/> 
    </div>
  );
}

export default App;
