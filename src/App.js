import React from 'react';
import './App.css';
import Navbar from './Components/Navbar/Navbar';
import Home from './Components/Home/Home';
import Footer from'./Components/Footer/Footer';
import Popular from './Components/Popular/Popular';
import { Route, Routes } from 'react-router-dom';
import SignInSignUp from './Components/SigninSignUp/signinsignup';
import ViewCourtInfo from './Components/ViewCourtInfo/viewCourtInfo';
import ViewInfo from './Components/ViewInfo/ViewInfo';
import EditInfo from './Components/EditInfo/EditInfo';
import FindCourt from './Components/findCourt/findCourt';
import Header from './Components/Header/header';
import 'react-toastify/dist/ReactToastify.css';
import { ToastContainer } from 'react-toastify';
import ForgetPassword from './Components/ForgetPassword/forgetPassword';

 
const App = () => {
  return (
    <>
    <Routes>
        <Route path="/" element={<div><Navbar/><Home/><Popular/><Footer/></div>}></Route>
        <Route path="/home" element={<div><Header/><Home/><Popular/><Footer/></div>}></Route>
        <Route path="/signin" element={<SignInSignUp defaultLoginVisible={true}/>}></Route>
        <Route path="/signup" element={<SignInSignUp defaultLoginVisible={false}/>}></Route>
        <Route path="/viewCourtInfo" element={<ViewCourtInfo/>}></Route>
        <Route path="/editInfo" element={<EditInfo/>}></Route>
        <Route path="/viewInfo" element={<ViewInfo/>}></Route>
        <Route path="/findCourt" element={<FindCourt/>}></Route>
        <Route path="/home" element={<div><Header/><Home/><Popular/><Footer/></div>}></Route>
        <Route path="/forget" element={<ForgetPassword/>}></Route>
    </Routes>
    <ToastContainer theme='colored'/>
    </>
  );
}

export default App;
