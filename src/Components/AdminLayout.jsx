import React from 'react';
import { Outlet } from 'react-router-dom';
import Sidebar from '../Scene/global/Sidebar';
import Topbar from '../Scene/global/Topbar';

const AdminLayout = ({ isSidebar, setIsSidebar }) => {
  return (
    <div className="app">
      <Sidebar isSidebar={isSidebar} />
      <main className="content">
        <Outlet />
      </main>
    </div>
  );
};

export default AdminLayout;
