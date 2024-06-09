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
  const [userDetail] = [
    {
      id: 2,
      firstName: 'duc',
      lastName: 'minh',
      email: '22',
      phone: '222'
    },
    {
      id: 3,
      firstName: 'duc2',
      lastName: 'minh232',
      email: '22323',
      phone: '2223232'
    },
    {
      id: 4,
      firstName: 'duc3',
      lastName: 'minh232322',
      email: '22322323',
      phone: '22223233232'
    },
  ];

  const token = sessionStorage.getItem('token');

  useEffect(() => {
    if (!token) {
      console.error('Token not found. Please log in.');
      return;
    }

    const decodedToken = jwtDecode(token); // Decode the JWT token to get user information
    const userIdToken = decodedToken.UserId; // Extract userId from the decoded token
    console.log(decodedToken)


    fetch(`http://localhost:5266/UserDetail/GetAll`, { // Fetch all user details
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
        // Find user with matching userId
        const matchingUser = data.find(user => user.userId == userIdToken);
        if (matchingUser) {
          matchingUser.id = matchingUser.userId; // Set id to userId
          //setUserDetail(matchingUser);
        }
      })
      .catch(error => console.error('Error fetching user info:', error));
  }, [token]);
  console.log(userDetail)
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const columns = [
    {
      field: "id",
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
        sx={{
          "& .MuiDataGrid-root": {
            border: "none",
          },
          "& .MuiDataGrid-cell": {
            borderBottom: "none",
          },
          "& .name-column--cell": {
            color: colors.greenAccent[300],
          },
          "& .MuiDataGrid-columnHeader": {
            backgroundColor: colors.blueAccent[700],
            borderBottom: "none",
          },
          "& .MuiDataGrid-virtualScroller": {
            backgroundColor: colors.primary[400],
          },
          "& .MuiDataGrid-footerContainer": {
            borderTop: "none",
            backgroundColor: colors.blueAccent[700],
          },
          "& .MuiCheckbox-root": {
            color: `${colors.greenAccent[200]} !important`,
          },
        }}
      >
        <DataGrid checkboxSelection rows={[userDetail]} columns={columns} />
      </Box>
    </Box>
  );
};

export default Team;
