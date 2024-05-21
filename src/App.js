import React from 'react'
import logo from './logo.svg';
import './App.css';
import LoginForm from './Components/LoginForm/LoginForm';
import Navbar from './Components/Navbar/Navbar'
import Home from './Components/Home/Home'
import Footer from'./Components/Footer/Footer'
import Popular from './Components/Popular/Popular'
const App = () => {
  return (
    <div>
      <Navbar/>
      <Home/> 
      <Popular/>
      <Footer/>
      {/* <LoginForm/> */}
    </div>
  );
}

export default App;
