import React, { useState } from 'react';
import './App.css';
import Navbar from './Components/Navbar/Navbar';
import Home from './Components/Home/Home';
import Footer from './Components/Footer/Footer';
import Popular from './Components/Popular/Popular';
import { Route, Routes, Navigate } from 'react-router-dom';
import SignInSignUp from './Components/SigninSignUp/signinsignup';
import ViewCourtInfo from './Components/ViewCourtInfo/viewCourtInfo';
import ViewInfo from './Components/ViewInfo/ViewInfo';
import EditInfo from './Components/EditInfo/EditInfo';
import FindCourt from './Components/findCourt/findCourt';
import Header from './Components/Header/header';
import 'react-toastify/dist/ReactToastify.css';
import { ToastContainer } from 'react-toastify';
import ForgetPassword from './Components/ForgetPassword/forgetPassword';
import { ColorModeContext, useMode } from './theme';
import { CssBaseline, ThemeProvider } from '@mui/material';
import User from "./Scene/user";
import Branch from "./Scene/courtBranch";
import Court from "./Scene/court";
import SlotManagement from "./Scene/timeSlot";
import BadmintonCourtHours from "./Scene/calendar";
import Payment from "./Scene/payment";
import Bar from "./Scene/bar";
import Pie from "./Scene/pie";
import Line from "./Scene/line";
import Geography from './Scene/geography';
import Dashboard from './Scene/dashboard';
import BookCourt from './Components/bookCourt/bookCourt';
import AdminLayout from './Components/AdminLayout';
import BuyTime from './Components/BuyTime/BuyTime';
import BookingHistory from './Components/ViewHistory/ViewHistory';
import PaySuccess from './Components/PaySuccess/PaySuccess';
import BuyFail from './Components/BuyFail/BuyFail';
import GoogleMap from './Components/googleMap/googleMap';

const App = () => {
  const [theme, colorMode] = useMode();
  const [isSidebar, setIsSidebar] = useState(true);
  const [searchCriteria, setSearchCriteria] = useState({ branch: '', location: '' });

  return (
    <ColorModeContext.Provider value={colorMode}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Routes>
          <Route path="/" element={<div><Navbar /><Home setSearchCriteria={setSearchCriteria} /><Popular searchCriteria={searchCriteria} /><Footer /></div>} />
          <Route path="/home" element={<div><Header /><Home setSearchCriteria={setSearchCriteria} /><Popular searchCriteria={searchCriteria} /><Footer /></div>} />
          <Route path="/signin" element={<SignInSignUp defaultLoginVisible={true} />} />
          <Route path="/signup" element={<SignInSignUp defaultLoginVisible={false} />} />
          <Route path="/viewCourtInfo" element={<ViewCourtInfo />} />
          <Route path="/editInfo" element={<EditInfo />} />
          <Route path="/viewInfo" element={<ViewInfo />} />
          <Route path="/findCourt" element={<FindCourt />} />
          <Route path="/forget" element={<ForgetPassword />} />
          <Route path="/bookCourt" element={<div><Header /><BookCourt /></div>} />
          <Route path="/buyTime" element={<div><Header /><BuyTime /><Footer /></div>} />
          <Route path="/paySuccess" element={<div><Header /><PaySuccess /><Footer /></div>} />
          <Route path="/payFail" element={<div><Header /><BuyFail /><Footer /></div>} />
          <Route path="/bookingHistory" element={<BookingHistory />} />
          <Route path="/googleMap" element={<GoogleMap />} />

          <Route path="/admin/*" element={<AdminLayout isSidebar={isSidebar} setIsSidebar={setIsSidebar} />}>
            <Route path="" element={<Navigate to="dashboard" />} /> {/* Default to Dashboard */}
            <Route path="dashboard" element={<Dashboard />} />
            <Route path="user" element={<User />} />
            <Route path="branch" element={<Branch />} />
            <Route path="court" element={<Court />} />
            <Route path="timeSlot" element={<SlotManagement />} />
            <Route path="timeManage" element={<BadmintonCourtHours />} />
            <Route path="payment" element={<Payment />} />
            <Route path="bar" element={<Bar />} />
            <Route path="pie" element={<Pie />} />
            <Route path="line" element={<Line />} />
            <Route path="geography" element={<Geography />} />
          </Route>
        </Routes>
        <ToastContainer theme='colored' />
      </ThemeProvider>
    </ColorModeContext.Provider>
  );
};

export default App;
