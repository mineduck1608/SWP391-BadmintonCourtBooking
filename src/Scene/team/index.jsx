import { Box, Typography, useTheme } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { tokens } from "../../theme";
import { mockDataTeam } from "../../data/mockData";
import AdminPanelSettingsOutlinedIcon from "@mui/icons-material/AdminPanelSettingsOutlined";
import LockOpenOutlinedIcon from "@mui/icons-material/LockOpenOutlined";
import SecurityOutlinedIcon from "@mui/icons-material/SecurityOutlined";
import Head from "../../Components/Head";
import { jwtDecode } from 'jwt-decode';
import React, { useState, useEffect } from "react";


const Team = () => {
  const [rows, setRows] = useState([]);
  const token = sessionStorage.getItem('token');

  useEffect(() => {
    if (!token) {
      console.error('Token not found. Please log in.');
      return;
    }

    const decodedToken = jwtDecode(token);
    const userIdToken = decodedToken.UserId;

    fetch(`http://localhost:5266/UserDetail/GetAll`, {
      method: "GET",
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    })
    .then(response => {
      if (!response.ok) {
        throw new Error('Failed to fetch user info');
      }
      return response.json();
    })
    .then((data) => {
      // Add a unique id to each row
      const formattedData = data.map((row, index) => ({ id: index + 1, ...row }));
      setRows(formattedData);
    })
    .catch(error => console.error('Error fetching user info:', error));
  }, [token]);

  const columns = [
    {
      field: "userId",
      headerName: "UserID"
    },
    {
      field: "firstName",
      headerName: "First Name",
      flex: 1,
    },
    {
      field: "lastName",
      headerName: "Last Name",
      flex: 1,
    },
    {
      field: "email",
      headerName: "Email",
      flex: 1,
    },
    {
      field: "phone",
      headerName: "Phone",
      flex: 1,
    },
  ];

  return (
    <Box m="20px">
      <Head title="TEAM" subtitle="Managing the Team Members" />
      <Box
        m="40px 0 0 0"
        height="75vh"
      >
        <DataGrid
          checkboxSelection
          rows={rows}
          columns={columns}
          getRowId={(row) => row.id}
        />
      </Box>
    </Box>
  );
};

export default Team;
