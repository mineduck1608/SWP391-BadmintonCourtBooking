import React from 'react';
import './App.css';
import Navbar from './Components/Navbar/Navbar';
import Home from './Components/Home/Home';
import Footer from './Components/Footer/Footer';
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
import { ColorModeContext, useMode } from './theme';
import { CssBaseline, ThemeProvider } from '@mui/material';
import Topbar from "./Scene/global/Topbar"
import Sidebar from './Scene/global/Sidebar';
import Team from "./Scene/team"
import Contacts from "./Scene/contacts"
import Invoices from "./Scene/invoices"
import Form from "./Scene/form"
import Calendar from "./Scene/calendar"
import FAQ from "./Scene/faq"
import Bar from "./Scene/bar"
import Pie from "./Scene/pie"
import Line from "./Scene/line"

const App = () => {
  const [theme, colorMode] = useMode();

  return (
    <ColorModeContext.Provider value={colorMode}>
      <ThemeProvider theme={theme}>
        <CssBaseline/>
        <div className='app'>
          <main className='content'>
        <Routes>
          <Route path="/" element={<div><Navbar /><Home /><Popular /><Footer /></div>}></Route>
          <Route path="/home" element={<div><Header /><Home /><Popular /><Footer /></div>}></Route>
          <Route path="/signin" element={<SignInSignUp defaultLoginVisible={true} />}></Route>
          <Route path="/signup" element={<SignInSignUp defaultLoginVisible={false} />}></Route>
          <Route path="/viewCourtInfo" element={<ViewCourtInfo />}></Route>
          <Route path="/editInfo" element={<EditInfo />}></Route>
          <Route path="/viewInfo" element={<ViewInfo />}></Route>
          <Route path="/findCourt" element={<FindCourt />}></Route>
          <Route path="/home" element={<div><Header /><Home /><Popular /><Footer /></div>}></Route>
          <Route path="/forget" element={<ForgetPassword />}></Route>
          <Route path="/admin" element={<div><Topbar/><Sidebar/></div>}></Route>
          <Route path="/team" element={<Team />}></Route>
          <Route path="/contacts" element={<Contacts />}></Route>
          <Route path="/invoices" element={<Invoices />}></Route>
          <Route path="/form" element={<Form />}></Route>
          <Route path="/calendar" element={<Calendar />}></Route>
          <Route path="/faq" element={<FAQ />}></Route>
          <Route path="/bar" element={<Bar />}></Route>
          <Route path="/pie" element={<Pie />}></Route>
          <Route path="/line" element={<Line />}></Route>




        </Routes>
        <ToastContainer theme='colored' />
        </main>
        </div>
      </ThemeProvider>
    </ColorModeContext.Provider>
  );
}




export default App;
