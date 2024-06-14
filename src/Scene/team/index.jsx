import { Box, Button } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState, useEffect } from "react";
import Head from "../../Components/Head";
import { Modal } from 'antd';
import './team.css'; // Import the custom CSS
import { toast } from "react-toastify";

const Team = () => {
  const [rows, setRows] = useState([]);
  const token = sessionStorage.getItem('token');
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const [selectedRow, setSelectedRow] = useState(null);
  const [roles, setRoles] = useState([]); // State to store roles
  const [branches, setBranches] = useState([]);
  const [addFormState, setAddFormState] = useState();
  const [addOpen, setAddOpen] = useState(false);


  // Define initial state values
  const initialState = {
    username: '',
    password: '',
    branch: '',
    balance: '',
    activeStatus: '',
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    role: ''
  };

  // Use a single state object to manage form fields
  const [formState, setFormState] = useState(initialState);


  const showModal = (row) => { // Nhận hàng được chọn làm đối số
    setSelectedRow(row); // Cập nhật selectedRow với hàng được chọn
    // Set form state based on selectedRow values
    setFormState({
      id: row.userId,
      username: row.userName || '',
      password: row.password || '',
      branch: row.branchName || '',
      balance: row.balance || '',
      activeStatus: row.activeStatus !== null ? row.activeStatus.toString() : '',
      firstName: row.firstName || '',
      lastName: row.lastName || '',
      email: row.email || '',
      phone: row.phone || '',
      role: row.role || ''
    });
    setOpen(true);
  };

  const handleOk = () => {
    const userData = formState;

    // Gửi yêu cầu cập nhật cho bảng User
    fetch(`http://localhost:5266/User/Update?id=` + userData.id + "&username=" + userData.username + "&password=" + userData.password + "&branchId=" + userData.branch + "&roleId=" + userData.role + "&firstName=" + userData.firstName + "&lastName=" + userData.lastName + "&phone=" + userData.phone + "&email=" + userData.email + "&activeStatus=" + userData.activeStatus + "&balance=" + userData.balance, {
      method: "PUT",
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(userData)

    })
      .then(response => {
        // Xử lý kết quả trả về từ bảng User
        toast.success("Update success.");
      })
      .catch(error => {
        console.error('Error updating user:', error);
      });
    setLoading(true);
    setTimeout(() => {
      setLoading(false);
      setOpen(false);
    }, 1000);
  };

  const handleCancel = () => {
    setOpen(false);
    setFormState(initialState); // Reset form state to initial values
  };

  const handleAddOk = () => {
    // Construct the data for the new user
    const newUser = {
      username: formState.username,
      password: formState.password,
      branch: formState.branch,
      firstName: formState.firstName,
      lastName: formState.lastName,
      email: formState.email,
      phone: formState.phone,
      role: formState.role
    };
  
    // Send a POST request to the API endpoint to add the new user
    fetch(`http://localhost:5266/User/RegisterAdmin?username=` + newUser.username + "&password=" + newUser.password + "&firstName=" + newUser.firstName + "&lastName=" + newUser.lastName + "&branch=" + newUser.branch + "&role=" + newUser.role + "&email=" + newUser.email + "&phone=" + newUser.phone, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newUser)
    })
    .then(response => {
      if (!response.ok) {
        throw new Error('Failed to add user');
      }
      return response.json();
    })
    .then(data => {
      toast.success(data);
    })
    .catch(error => {
      toast.warning('Error adding user:', error);
      // Handle errors, such as displaying an error message to the user
    });
  
    setLoading(true);
    setTimeout(() => {
      setLoading(false);
      setAddOpen(false);
    }, 1000);
  };

  const handleAddCancel = () => {
    setAddOpen(false);
    setAddFormState(initialState); // Reset form state to initial values
  };

  const addUser = () => {
    setAddOpen(true);
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

        setRoles(roles); // Store roles in state

        setBranches(branches); // Store branches in state

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
        toast.success('User deleted successfully.');
        setRows(prevRows => prevRows.filter(row => row.id !== id));
      })
      .catch(error => {
        toast.warning('User deleted unsuccess.')
      });
  };

  const columns = [
    { field: "userId", headerName: "UserID", align: "center", headerAlign: "center" },
    { field: "firstName", headerName: "First Name", flex: 1, align: "center", headerAlign: "center" },
    { field: "lastName", headerName: "Last Name", flex: 1, align: "center", headerAlign: "center" },
    { field: "email", headerName: "Email", flex: 1, align: "center", headerAlign: "center" },
    { field: "phone", headerName: "Phone", flex: 1, align: "center", headerAlign: "center" },
    { field: "role", headerName: "Role", flex: 1, align: "center", headerAlign: "center" },
    {
      field: "activeStatus", headerName: "Active Status", flex: 1, align: "center", headerAlign: "center", renderCell: (params) => (
        <Box color={params.value ? 'lightgreen' : 'red'}>
          {params.value ? 'true' : 'false'}
        </Box>
      )
    },
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
            Edit Info
          </Button>
          <Modal
            width={1000}
            open={open}
            title="User Information"
            onOk={handleOk}
            onCancel={handleCancel}
            className="custom-modal"
            footer={[
              <Button key="back" onClick={handleCancel} className="button-hover-black">
                Return
              </Button>,
              <Button key="submit" type="primary" loading={loading} onClick={handleOk} className="button-hover-black">
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
                      <input value={formState.username} placeholder={selectedRow ? selectedRow.userName : ''} onChange={e => setFormState({ ...formState, username: e.target.value })} className="input-box-modal" type="text" />
                      <input value={formState.firstName} placeholder={selectedRow ? selectedRow.firstName : ''} onChange={e => setFormState({ ...formState, firstName: e.target.value })} className="input-box-modal" type="text" />
                      <input value={formState.email} placeholder={selectedRow ? selectedRow.email : ''} onChange={e => setFormState({ ...formState, email: e.target.value })} className="input-box-modal" type="text" />
                      <select value={formState.role} onChange={(e) => setFormState({ ...formState, role: e.target.value })} className="input-box-modal">
                        <option disabled selected hidden value={selectedRow ? selectedRow.role : ''}>{selectedRow ? selectedRow.role : ''}</option>
                        {roles.map(role => (
                          <option key={role.roleId} value={role.roleId}>{role.roleName}</option>
                        ))}
                      </select>
                      <select value={formState.activeStatus} onChange={e => setFormState({ ...formState, activeStatus: e.target.value })} className="input-box-modal" type="text" >
                        <option value="0" hidden>{selectedRow ? selectedRow.activeStatus.toString() : ''}</option>
                        <option value="true">true</option>
                        <option value="false">false</option>
                      </select>
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
                      <input value={formState.password} placeholder={selectedRow ? selectedRow.password : ''} onChange={e => setFormState({ ...formState, password: e.target.value })} className="input-box-modal" type="text" />
                      <input value={formState.lastName} placeholder={selectedRow ? selectedRow.lastName : ''} onChange={e => setFormState({ ...formState, lastName: e.target.value })} className="input-box-modal" type="text" />
                      <select value={formState.branch} onChange={(e) => setFormState({ ...formState, branch: e.target.value })} className="input-box-modal">
                        <option disabled selected hidden value={selectedRow ? selectedRow.branchName : ''}>{selectedRow ? selectedRow.branchName : ''}</option>
                        {branches.map(branch => (
                          <option key={branch.branchId} value={branch.branchId}>{branch.branchName}</option>
                        ))}
                      </select>
                      <input value={formState.phone} placeholder={selectedRow ? selectedRow.phone : ''} onChange={e => setFormState({ ...formState, phone: e.target.value })} className="input-box-modal" type="text" />
                      <input value={formState.balance} placeholder={selectedRow ? selectedRow.balance : ''} onChange={e => setFormState({ ...formState, balance: e.target.value })} className="input-box-modal" type="number" min="0" />
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
      <Head title="User" subtitle="Managing the User Accounts" />
      <Box>
        <Button type="primary" onClick={addUser} variant="contained" color="primary" size="small">
          Add User
        </Button>
        <Modal
          width={1000}
          open={addOpen}
          title="Register New User"
          onOk={handleAddOk}
          onCancel={handleAddCancel}
          className="custom-modal"
          footer={[
            <Button key="back" onClick={handleAddCancel} className="button-hover-black">
              Return
            </Button>,
            <Button key="submit" type="primary" loading={loading} onClick={handleAddOk} className="button-hover-black">
              Submit
            </Button>
          ]}
          centered
        >
          <form>
            <div className="user-modal">
              <div className="user-modal-left">
                <div className="user-modal-item">
                  <div className="user-modal-item-text1">
                    <p>Username:</p>
                    <p>Password:</p>
                    <p>First Name:</p>
                    <p>Last Name:</p>
                    <p>Role:</p>
                    <p>Branch:</p>
                    <p>Email:</p>
                    <p>Phone:</p>
                    
                  </div>
                  <div className="user-modal-item-valu1">
                    <input value={formState.username} onChange={(e) => setFormState({ ...formState, username: e.target.value })} className="input-box-modal" type="text" required/>
                    <input value={formState.password} onChange={(e) => setFormState({ ...formState, password: e.target.value })} className="input-box-modal" type="password" required />
                    <input value={formState.firstName} onChange={(e) => setFormState({ ...formState, firstName: e.target.value })} className="input-box-modal" type="text" required />
                    <input value={formState.lastName} onChange={(e) => setFormState({ ...formState, lastName: e.target.value })} className="input-box-modal" type="text"  required/>
                    <select value={formState.role} onChange={(e) => setFormState({ ...formState, role: e.target.value })} className="input-box-modal" required>
                      <option value="" hidden>Select role</option>
                      {roles.map(role => (
                          <option key={role.roleId} value={role.roleId}>{role.roleName}</option>
                        ))}
                    </select>
                    <select value={formState.branch} onChange={(e) => setFormState({ ...formState, branch: e.target.value })} className="input-box-modal">
                      <option disabled selected hidden value="">Please select branch</option>
                      {branches.map(branch => (
                        <option key={branch.branchId} value={branch.branchId}>{branch.branchName}</option>
                      ))}
                    </select>                         
                    <input value={formState.email} onChange={(e) => setFormState({ ...formState, email: e.target.value })} className="input-box-modal" type="email" />
                    <input value={formState.phone} onChange={(e) => setFormState({ ...formState, phone: e.target.value })} className="input-box-modal" type="text" />
                    
                  </div>
                </div>
              </div>
            </div>
          </form>
        </Modal>
      </Box>
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
