import { Box, Button } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState, useEffect } from "react";
import Head from "../../Components/Head";
import { Modal } from 'antd';
import './team.css'; // Import the custom CSS

const Team = () => {
  const [rows, setRows] = useState([]);
  const token = sessionStorage.getItem('token');
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const [selectedRow, setSelectedRow] = useState(null);

  const [username, usernameChange] = useState("");
  const [password, passwordChange] = useState("");
  const [branch, branchChange] = useState("");
  const [balance, balanceChange] = useState("");
  const [activeStatus, activeStatusChange] = useState("");
  const [firstName, firstNameChange] = useState("");
  const [lastName, lastNameChange] = useState("");
  const [email, emailChange] = useState("");
  const [phone, phoneChange] = useState("");
  const [role, roleChange] = useState("");


  const showModal = (row) => { // Nhận hàng được chọn làm đối số
    setSelectedRow(row); // Cập nhật selectedRow với hàng được chọn
    setOpen(true);
  };

  const handleOk = () => {
    setLoading(true);
    setTimeout(() => {
      setLoading(false);
      setOpen(false);
    }, 3000);
  };

  const handleCancel = () => {
    setOpen(false);
  };

  useEffect(() => {
    if (!token) {
      console.error('Token not found. Please log in.');
      return;
    }

    const fetchData = async () => {
      try {
        const [userDetailsRes, rolesRes, usersRes, branchesRes] = await Promise.all([
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
          }),
          fetch(`http://localhost:5266/Branch/GetAll`, {
            method: "GET",
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          })
        ]);

        if (!userDetailsRes.ok || !rolesRes.ok || !usersRes.ok || !branchesRes.ok) {
          throw new Error('Failed to fetch data');
        }

        const [userDetails, roles, users, branches] = await Promise.all([
          userDetailsRes.json(),
          rolesRes.json(),
          usersRes.json(),
          branchesRes.json()
        ]);

        roles.forEach(role => {
          if (!role.roleId || !role.roleName) {
            throw new Error(`Role data is missing roleId or roleName: ${JSON.stringify(role)}`);
          }
        });

        const mergedData1 = userDetails.map(userDetail => {
          const user = users.find(u => u.userId === userDetail.userId);
          if (user) {
            return { ...userDetail, ...user };
          }
          return userDetail;
        });

        const mergedData2 = mergedData1.map(userDetail => {
          const branch = branches.find(b => b.branchId === userDetail.branchId);
          if (branch) {
            return { ...userDetail, branchName: branch.branchName };
          }
          return userDetail;
        });

        const formattedData = mergedData2.map((row, index) => {
          const role = roles.find(r => r.roleId === row.roleId);
          return { id: index + 1, ...row, role: role ? role.roleName : 'Unknown' };
        });


        setRows(formattedData);
        console.log(formattedData)
      } catch (error) {
        console.error('Error fetching data:', error);
      }
    };

    fetchData();

    const intervalId = setInterval(fetchData, 1000);

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
          <Button type="primary" onClick={() => showModal(params.row)}
            variant="contained"
            color="primary"
            size="small"
          >
            View Info
          </Button>
          <Modal
            width={1000}
            open={open}
            title="User Information"
            onOk={handleOk}
            onCancel={handleCancel}
            className="custom-modal"
            footer={[
              <Button key="back" onClick={handleCancel}>
                Return
              </Button>,
              <Button key="submit" type="primary" loading={loading} onClick={handleOk}>
                Submit
              </Button>
            ]}
            centered
          >
            <form action="">
              <div className="user-modal">
                <div className="user-modal-left">
                  <div className="user-modal-item">
                    <div className="user-modal-item-text1">
                      <p>Username:</p>
                      <p>First Name:</p>
                      <p>Email:</p>
                      <p>Role:</p>
                      <p>Active Status:</p>
                    </div>
                    <div className="user-modal-item-value">
                      <input value={username} placeholder={selectedRow ? selectedRow.userName : ''} onChange={e => usernameChange(e.target.value)} className="input-box-modal" type="text" />
                      <input value={firstName} placeholder={selectedRow ? selectedRow.firstName : ''} onChange={e => firstNameChange(e.target.value)} className="input-box-modal" type="text" />
                      <input value={email} placeholder={selectedRow ? selectedRow.email : ''} onChange={e => emailChange(e.target.value)} className="input-box-modal" type="text" />
                      <input value={role} placeholder={selectedRow ? selectedRow.role : ''} onChange={e => roleChange(e.target.value)} className="input-box-modal" type="text" />
                      <input
                        value={activeStatus}
                        placeholder={selectedRow ? selectedRow.activeStatus.toString() : ''}
                        onChange={e => activeStatusChange(e.target.value)}
                        className="input-box-modal"
                        type="text"
                      />
                    </div>
                  </div>
                </div>
                <div className="user-modal-right">
                  <div className="user-modal-item">
                    <div className="user-modal-item-text2">
                      <p>Password:</p>
                      <p>Last Name:</p>
                      <p> Branch:</p>
                      <p>Phone:</p>
                      <p>Balance:</p>
                    </div>
                    <div className="user-modal-item-value">
                      <input value={password} placeholder={selectedRow ? selectedRow.password : ''} onChange={e => passwordChange(e.target.value)} className="input-box-modal" type="text" />
                      <input value={lastName} placeholder={selectedRow ? selectedRow.lastName : ''} onChange={e => lastNameChange(e.target.value)} className="input-box-modal" type="text" />
                      <input value={branch} placeholder={selectedRow ? selectedRow.branchName : ''} onChange={e => branchChange(e.target.value)} className="input-box-modal" type="text" />
                      <input value={phone} placeholder={selectedRow ? selectedRow.phone : ''} onChange={e => phoneChange(e.target.value)} className="input-box-modal" type="text" />
                      <input value={balance} placeholder={selectedRow ? selectedRow.balance : ''} onChange={e => balanceChange(e.target.value)} className="input-box-modal" type="text" />
                    </div>
                  </div>
                </div>
              </div>
            </form>
          </Modal>
          <Button
            variant="contained"
            size="small"
            onClick={() => handleDelete(params.row.userId)}
            style={{ backgroundColor: '#b22222', color: 'white', marginLeft: 8 }}
          >
            Ban
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
