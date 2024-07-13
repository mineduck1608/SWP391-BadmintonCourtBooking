import React, { useState, useEffect } from 'react';
import { Box, Button, TextField, Radio, RadioGroup, FormControlLabel, FormControl, FormLabel } from "@mui/material";
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import { useTheme } from "@mui/material";
import { Modal, Spin, ConfigProvider } from 'antd';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './branch.css';
import { v4 } from 'uuid';
import { uploadBytes, getDownloadURL } from 'firebase/storage';
import { ref } from 'firebase/storage';
import { imageDb } from '../../Components/googleSignin/config.js';
import { jwtDecode } from 'jwt-decode';
import { fetchWithAuth } from '../../Components/fetchWithAuth/fetchWithAuth.jsx';

const Branch = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const [userRole, setUserRole] = useState('');

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [modalVisible, setModalVisible] = useState(false);
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [addModalVisible, setAddModalVisible] = useState(false);
  const [newBranch, setNewBranch] = useState({
    location: '',
    branchName: '',
    branchPhone: '',
    branchImg: '',
    mapUrl: '',  // Added mapUrl
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const branchResponse = await fetchWithAuth('https://localhost:7233/Branch/GetAll');
        if (!branchResponse.ok) {
          throw new Error('Failed to fetch branch data');
        }
        const branchData = await branchResponse.json();

        const feedbackResponse = await fetchWithAuth('https://localhost:7233/Feedback/GetAll');
        if (!feedbackResponse.ok) {
          throw new Error('Failed to fetch feedback data');
        }

        const feedbackData = await feedbackResponse.json();

        const feedbackMap = feedbackData.reduce((acc, feedback) => {
          acc[feedback.branchId] = acc[feedback.branchId] || [];
          acc[feedback.branchId].push(feedback.content);
          return acc;
        }, {});

        const combinedData = branchData.map((branch, index) => ({
          ...branch,
          id: index + 1,
          feedbacks: feedbackMap[branch.branchId] ? feedbackMap[branch.branchId].join(', ') : 'No feedbacks'
        }));

        let token;
        token = sessionStorage.getItem('token');
        let decodedToken;
        decodedToken = jwtDecode(token);

        setUserRole(decodedToken.Role);

        setData(combinedData);
        setLoading(false);
      } catch (error) {
        setError(error.message);
        setLoading(false);
      }
    };

    fetchData();
    const interval = setInterval(fetchData, 1000); // Fetch data every 1000ms
    return () => clearInterval(interval); // Cleanup interval on component unmount
  }, []);

  const handleViewInfo = (id) => {
    const branch = data.find((branch) => branch.branchId === id);
    setSelectedBranch(branch);
    setModalVisible(true);
  };

  const handleDelete = (id) => {
    console.log(`Delete row with id: ${id}`);
    // Implement the logic for deleting a row here
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setSelectedBranch({
      ...selectedBranch,
      [name]: name === 'branchStatus' ? value === 'true' : value,
    });
  };

  const handleAddInputChange = (e) => {
    const { name, value } = e.target;
    setNewBranch({
      ...newBranch,
      [name]: value,
    });
  };
  const token = sessionStorage.getItem('token');

  const handleSave = async () => {
    const { location, branchImg, branchName, branchPhone, branchStatus, branchId, mapUrl } = selectedBranch;
    const statusValue = branchStatus ? 1 : 0;
    const encodedUrl = encodeURIComponent(branchImg);
    console.log(branchImg)
  
    try {
      const response = await fetchWithAuth(`https://localhost:7233/Branch/Update?location=${location}&img=${encodedUrl}&name=${branchName}&phone=${branchPhone}&status=${statusValue}&id=${branchId}&mapUrl=${mapUrl}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
      });
  
      if (!response.ok) {
        throw new Error('Failed to update branch');
      }
  
      toast.success('Branch updated successfully!');
      setModalVisible(false);
    } catch (error) {
      console.error('Error:', error);
      toast.error('Failed to update branch');
    }
  };
  

  const [img, setImg] = useState(null);


  const handleAddBranch = async () => {
    const { location, branchImg, branchName, branchPhone, mapUrl } = newBranch;
    const encodedUrl = encodeURIComponent(branchImg);
  
    try {
      const response = await fetchWithAuth(`https://localhost:7233/Branch/Add?location=${location}&img=${encodedUrl}&name=${branchName}&phone=${branchPhone}&mapUrl=${mapUrl}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify({
          location,
          branchImg: encodedUrl,
          branchName,
          branchPhone,
          mapUrl,
        }),
      });
  
      if (!response.ok) {
        throw new Error('Failed to add branch');
      }
      toast.success('Branch added successfully!');
      setAddModalVisible(false);
    } catch (error) {
      console.error('Error:', error);
      toast.error('Failed to add branch');
    }
  };
  

  const columns = [
    { field: "branchId", headerName: "Branch ID", flex: 0.5, align: "center", headerAlign: "center" },
    { field: "location", headerName: "Location", align: "center", headerAlign: "center" },
    { field: "branchName", headerName: "Branch Name", flex: 1, cellClassName: "name-column--cell", align: "center", headerAlign: "center" },
    { field: "branchPhone", headerName: "Phone", type: "number", headerAlign: "center", align: "center", flex: 1 },
    { field: "branchImg", headerName: "Image", flex: 1, align: "center", headerAlign: "center" },
    { field: "mapUrl", headerName: "Map URL", flex: 1, align: "center", headerAlign: "center" },  // Added mapUrl column
    {
      field: "branchStatus",
      headerName: "Status",
      flex: 1,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Box color={params.value ? 'green' : 'red'}>
          {params.value ? 'Active' : 'Closed'}
        </Box>
      )
    },
    { field: "feedbacks", headerName: "Feedbacks", flex: 1, align: "center", headerAlign: "center" },
    {
      field: "actions",
      headerName: "Actions",
      flex: 1,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Box display="flex" justifyContent="center" alignItems="center" gap="10px" height="100%">
          <Button
            variant="contained"
            color="primary"
            onClick={() => handleViewInfo(params.row.branchId)}
          >
            Edit Info
          </Button>
        </Box>
      )
    }
  ];

  if (loading) {
    return <Box m="20px">Loading...</Box>;
  }

  if (error) {
    return <Box m="20px">Error: {error}</Box>;
  }
  const handleClick = (e) => {
    e.preventDefault();
    if (!img) {
      toast.error('No image selected');
      return;
    }
    const imgRef = ref(imageDb, `files/${v4()}`);
    uploadBytes(imgRef, img)
      .then(() => getDownloadURL(imgRef))
      .then(url => {
        if (modalVisible) {
          setSelectedBranch(branch => ({ ...branch, branchImg: url }));
        } else {
          setNewBranch(branch => ({ ...branch, branchImg: url }));
        }
        toast.success('Image uploaded successfully');
      })
      .catch(error => {
        console.error('Error uploading image:', error);
        toast.error('Image upload failed');
      });
  };
  
  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setImg(file);
    }
  };

  return (
    <ConfigProvider theme={{
      token: {
        colorPrimary: theme.palette.primary.main,
        colorSuccess: theme.palette.success.main,
        colorWarning: theme.palette.warning.main,
        colorError: theme.palette.error.main,
        colorInfo: theme.palette.info.main,
      },
    }}>
      <Box m="20px">
        <Head title="BRANCHES" subtitle="List of Branches for Future Reference" />
        <Box display="flex" justifyContent="space-between" alignItems="center" marginTop="20px">
        {userRole === 'Admin' && (
          <Button variant="contained" color="primary" onClick={() => setAddModalVisible(true)}>
            Add Branch
          </Button>
          )}
        </Box>
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
            "& .MuiDataGrid-toolbarContainer .MuiButton-text": {
              color: `${colors.grey[100]} !important`,
            },
          }}
        >
          <DataGrid
            rows={data}
            columns={columns}
            components={{ Toolbar: GridToolbar }}
            pagination
            pageSize={10}
            rowsPerPageOptions={[5, 10, 20]}
          />
        </Box>

        <Modal
          title={<span style={{ fontSize: '32px' }}>Branch Info</span>}
          open={modalVisible}
          onCancel={() => setModalVisible(false)}
          footer={[
            <Button key="cancel" variant="contained" color="primary" onClick={() => setModalVisible(false)}>
              Cancel
            </Button>,
            <Button key="submit" variant="contained" color="secondary" onClick={handleSave} style={{ marginLeft: 8 }}>
              Save
            </Button>
          ]}
        >
          {selectedBranch ? (
            <Box component="form" noValidate autoComplete="off">
              <TextField
                label="Branch ID"
                name="branchId"
                value={selectedBranch.branchId}
                InputProps={{
                  readOnly: true,
                }}
                fullWidth
                margin="normal"
              />
              <TextField
                label="Location"
                name="location"
                value={selectedBranch.location}
                onChange={handleInputChange}
                fullWidth
                margin="normal"
              />
              <TextField
                label="Branch Name"
                name="branchName"
                value={selectedBranch.branchName}
                onChange={handleInputChange}
                fullWidth
                margin="normal"
              />
              <TextField
                label="Phone"
                name="branchPhone"
                value={selectedBranch.branchPhone}
                onChange={handleInputChange}
                fullWidth
                margin="normal"
              />
              <TextField
                label="Map URL"
                name="mapUrl"
                value={selectedBranch.mapUrl}  // Added mapUrl
                onChange={handleInputChange}
                fullWidth
                margin="normal"
              />
              <div className="uploaded-image-container">
                <TextField
                  label="Image URL"
                  name="branchImg"
                  value={selectedBranch.branchImg}
                  InputProps={{
                    readOnly: true,
                  }}
                  fullWidth
                  margin="normal"
                />
              </div>
              <div className="uploaded-branchimage-upload">
                <input className="button-branch-input" type="file" onChange={handleImageChange} />
                <button className="button upload" onClick={handleClick}>Upload</button>
              </div>
              <FormControl component="fieldset" margin="normal">
                <FormLabel component="legend">Branch Status</FormLabel>
                <RadioGroup
                  row
                  name="branchStatus"
                  value={selectedBranch.branchStatus ? 'true' : 'false'}
                  onChange={handleInputChange}
                >
                  <FormControlLabel value="true" control={<Radio />} label="Active" />
                  <FormControlLabel value="false" control={<Radio />} label="Closed" />
                </RadioGroup>
              </FormControl>
            </Box>
          ) : (
            <Spin />
          )}
        </Modal>

        <Modal
          title={<span style={{ fontSize: '32px' }}>Add Branch</span>}
          open={addModalVisible}
          onCancel={() => setAddModalVisible(false)}
          footer={[
            <Button key="cancel" variant="contained" color="primary" onClick={() => setAddModalVisible(false)}>
              Cancel
            </Button>,
            <Button key="submit" variant="contained" color="secondary" onClick={handleAddBranch} style={{ marginLeft: 8 }}>
              Save
            </Button>,
          ]}
        >
          <Box component="form" noValidate autoComplete="off">
            <TextField
              label="Location"
              name="location"
              value={newBranch.location}
              onChange={handleAddInputChange}
              fullWidth
              margin="normal"
            />
            <TextField
              label="Branch Name"
              name="branchName"
              value={newBranch.branchName}
              onChange={handleAddInputChange}
              fullWidth
              margin="normal"
            />
            <TextField
              label="Phone"
              name="branchPhone"
              value={newBranch.branchPhone}
              onChange={handleAddInputChange}
              fullWidth
              margin="normal"
            />
            <TextField
              label="Image"
              name="branchImg"
              value={newBranch.branchImg}
              onChange={handleAddInputChange}
              fullWidth
              margin="normal"
            />
            <TextField
              label="Map URL"
              name="mapUrl"  // Added mapUrl
              value={newBranch.mapUrl}
              onChange={handleAddInputChange}
              fullWidth
              margin="normal"
            />
          </Box>
        </Modal>

        <ToastContainer theme='colored' />
      </Box>
    </ConfigProvider>
  );
};

export default Branch;
