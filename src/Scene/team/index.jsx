import { Box, Button } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import jwtDecode from 'jwt-decode';
import React, { useState, useEffect } from "react";
import Head from "../../Components/Head";

const Team = () => {
  const [rows, setRows] = useState([]);
  const token = sessionStorage.getItem('token');

  useEffect(() => {
    if (!token) {
      console.error('Token not found. Please log in.');
      return;
    }

    const fetchData = async () => {
      try {
        const [userDetailsRes, rolesRes, usersRes] = await Promise.all([
          fetch(`http://localhost:5266/UserDetail/GetAll`, {
            method: "GET",
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          }),
          fetch(`http://localhost:5266/Role/GetAll`, {
            method: "GET",
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          }),
          fetch(`http://localhost:5266/User/GetAll`, {
            method: "GET",
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          })
        ]);

        if (!userDetailsRes.ok || !rolesRes.ok || !usersRes.ok) {
          throw new Error('Failed to fetch data');
        }

        const [userDetails, roles, users] = await Promise.all([
          userDetailsRes.json(),
          rolesRes.json(),
          usersRes.json()
        ]);

        // Check if roleId and roleName exist in roles
        roles.forEach(role => {
          if (!role.roleId || !role.roleName) {
            throw new Error(`Role data is missing roleId or roleName: ${JSON.stringify(role)}`);
          }
        });

        // Merge userDetails with users based on userId
        const mergedData = userDetails.map(userDetail => {
          const user = users.find(u => u.userId === userDetail.userId);
          if (user) {
            return { ...userDetail, ...user };
          }
          return userDetail;
        });

        // Process merged data to add unique id and role names
        const formattedData = mergedData.map((row, index) => {
          const role = roles.find(r => r.roleId === row.roleId);
          return { id: index + 1, ...row, role: role ? role.roleName : 'Unknown' };
        });

        setRows(formattedData);
      } catch (error) {
        console.error('Error fetching data:', error);
      }
    };

    // Initial fetch
    fetchData();

    // Set interval for continuous fetching
    const intervalId = setInterval(fetchData, 1000); // Fetch every 1 seconds

    // Clean up function to clear interval
    return () => clearInterval(intervalId);
  }, [token]);

  const handleDelete = (id) => {
    console.log(`Delete user with id: ${id}`);
    fetch(`http://localhost:5266/User/Delete?id=${id}`, {
        method: "DELETE",
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Failed to delete user');
        }
        return response.json();
    })
    .then(data => {
        console.log('User deleted successfully:', data);
        // Remove deleted user from rows
        setRows(prevRows => prevRows.filter(row => row.id !== id));
    })
    .catch(error => {
        console.error('Error deleting user:', error);
    });
  };

  const columns = [
    { field: "userId", headerName: "UserID", align: "center", headerAlign: "center" },
    { field: "firstName", headerName: "First Name", flex: 1, align: "center", headerAlign: "center" },
    { field: "lastName", headerName: "Last Name", flex: 1, align: "center", headerAlign: "center" },
    { field: "email", headerName: "Email", flex: 1, align: "center", headerAlign: "center" },
    { field: "phone", headerName: "Phone", flex: 1, align: "center", headerAlign: "center" },
    { field: "role", headerName: "Role", flex: 1, align: "center", headerAlign: "center" },
    { field: "activeStatus", headerName: "Active Status", flex: 1, align: "center", headerAlign: "center" },
    {
      field: "actions",
      headerName: "Actions",
      sortable: false,
      flex: 1,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Box>
          <Button
            variant="contained"
            color="primary"
            size="small"
          >
            View Info
          </Button>
          <Button
            variant="contained"
            color="secondary"
            size="small"
            onClick={() => handleDelete(params.id)}
            style={{ marginLeft: 8 }}
          >
            Delete
          </Button>
        </Box>
      )
    }
  ];

  return (
    <Box m="20px">
      <Head title="TEAM" subtitle="Managing the Team Members" />
      <Box m="40px 0 0 0" height="75vh">
        <DataGrid
          rows={rows}
          columns={columns}
          getRowId={(row) => row.userId}
        />
      </Box>
    </Box>
  );
};

export default Team;
