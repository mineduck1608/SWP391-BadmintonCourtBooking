import React from 'react';
import './App.css';
import Navbar from './Components/Navbar/Navbar';
import Home from './Components/Home/Home';
import Footer from'./Components/Footer/Footer';
import Popular from './Components/Popular/Popular';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import SignInSignUp from './Components/SigninSignUp/signinsignup';
import ViewCourtInfo from './Components/ViewCourtInfo/viewCourtInfo';
import ViewInfo from './Components/ViewInfo/ViewInfo';
import EditInfo from './Components/EditInfo/EditInfo';
import FindCourt from './Components/findCourt/findCourt';

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
  {
    path: '/viewCourtInfo',
    element: <div><ViewCourtInfo/></div>
  },
  {
    path: '/editInfo',
    element: <div><EditInfo/></div>
  },
  {
    path: '/viewInfo',
    element: <div><ViewInfo/></div>
  },
  {
    path: '/findCourt',
    element: <div><Navbar/><FindCourt/></div>
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
