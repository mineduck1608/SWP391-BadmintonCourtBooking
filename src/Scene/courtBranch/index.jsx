import React, { useState, useEffect } from 'react';
import { Box, Button, TextField } from "@mui/material";
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import { useTheme } from "@mui/material";
import { Modal, Spin, ConfigProvider, Radio } from 'antd';
import './branch.css';
import { ToastContainer, toast } from 'react-toastify';

const Branch = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [modalVisible, setModalVisible] = useState(false);
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [addModalVisible, setAddModalVisible] = useState(false);
  const [newBranch, setNewBranch] = useState({
    location: '',
    img: '',
    name: '',
    phone: '',
    status: '',
  });

  const fetchData = async () => {
    try {
      const [branchResponse, feedbackResponse] = await Promise.all([
        fetch('http://localhost:5266/Branch/GetAll'),
        fetch('http://localhost:5266/Feedback/GetAll')
      ]);

      if (!branchResponse.ok || !feedbackResponse.ok) {
        throw new Error('Failed to fetch data');
      }

      const branches = await branchResponse.json();
      const feedbacks = await feedbackResponse.json();

      const feedbackMap = feedbacks.reduce((acc, feedback) => {
        if (!acc[feedback.branchId]) {
          acc[feedback.branchId] = [];
        }
        acc[feedback.branchId].push(feedback);
        return acc;
      }, {});

      const newData = branches.map((branch, index) => ({
        ...branch,
        id: index + 1,
        feedbacks: feedbackMap[branch.branchId] || []
      }));

      setData(newData);
      setLoading(false);
    } catch (error) {
      setError(error.message);
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleViewInfo = (id) => {
    const branch = data.find((branch) => branch.branchId === id);
    setSelectedBranch(branch);
    setModalVisible(true);
    setAddModalVisible(false);
  };

  const handleDelete = async (id) => {
    const branch = data.find((branch) => branch.id === id);
    const url = `http://localhost:5266/Branch/Delete?id=${branch.branchId}`;

    try {
      const response = await fetch(url, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
        }
      });

      if (!response.ok) {
        throw new Error('Failed to delete branch');
      }

      // Update the branch data in the state
      setData((prevData) =>
        prevData.filter((b) => b.id !== id)
      );
      toast.success("Branch Deleted Successfully");
    } catch (error) {
      console.error('Error:', error);
      toast.error("Failed to delete branch");
    }
  };

  const handleAddBranch = async () => {
    const { location, img, name, phone, status } = newBranch;
    try {
      const response = await fetch(`http://localhost:5266/Branch/Add?location=${location}&img=${img}&name=${name}&phone=${phone}&status=${status}`, {
        method: 'POST'
      });

      if (!response.ok) {
        throw new Error('Failed to add branch');
      }

      // Fetch the updated data
      fetchData();

      setAddModalVisible(false);
      setNewBranch({
        location: '',
        img: '',
        name: '',
        phone: '',
        status: true
      });
    } catch (error) {
      console.error(error.message);
    }
  };

  const handleEditBranch = async () => {
    const { location, img, name, phone, status, branchId } = selectedBranch;
    try {
      const response = await fetch(`http://localhost:5266/Branch/Update?location=${location}&img=${img}&name=${name}&phone=${phone}&status=${status}&id=${branchId}`, {
        method: 'PUT'
      });

      if (!response.ok) {
        throw new Error('Failed to update branch');
      }

      // Fetch the updated data
      fetchData();

      setModalVisible(false);
      setSelectedBranch(null);
    } catch (error) {
      console.error(error.message);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setSelectedBranch((prevBranch) => ({
      ...prevBranch,
      [name]: value
    }));
  };

  const handleStatusChange = (e) => {
    const { value } = e.target;
    setSelectedBranch((prevBranch) => ({
      ...prevBranch,
      status: value
    }));
  };

  const columns = [
    { field: "branchId", headerName: "Branch ID", flex: 0.5, align: "center", headerAlign: "center" },
    { field: "location", headerName: "Location", align: "center", headerAlign: "center" },
    { field: "branchName", headerName: "Branch Name", flex: 1, cellClassName: "name-column--cell", align: "center", headerAlign: "center" },
    { field: "branchPhone", headerName: "Phone", type: "number", headerAlign: "center", align: "center", flex: 1 },
    { field: "branchImg", headerName: "Image", flex: 1, align: "center", headerAlign: "center" },
    {
      field: "branchStatus",
      headerName: "Status",
      flex: 1,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Box color={params.value ? 'green' : 'red'}>
          {params.value ? 'true' : 'false'}
        </Box>
      )
    },
    {
      field: "feedbacks",
      headerName: "Feedbacks",
      flex: 1,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Box>
          {params.value.map((feedback, index) => (
            <div key={index}>{feedback.content}</div>
          ))}
        </Box>
      )
    },
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
          <Button
            variant="contained"
            color="secondary"
            onClick={() => handleDelete(params.row.id)}
          >
            Delete
          </Button>
        </Box>
      )
    }
  ];

  if (loading) {
    return <Box m="20px">Loading...</Box>
  }

  if (error) {
    return <Box m="20px">Error: {error}</Box>
  }

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
        <Box display="flex" justifyContent="space-between" alignItems="center" m="20px 0">
          <Button variant="contained" color="primary" onClick={() => setAddModalVisible(true)}>
            Add Branch
          </Button>
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
          title={<span style={{ fontSize: '32px' }}>Edit Branch</span>}
          open={modalVisible}
          onCancel={() => setModalVisible(false)}
          footer={(
            <Button type="primary" onClick={handleEditBranch}>
              Submit
            </Button>
          )}
        >
          {selectedBranch ? (
            <Box>
              <TextField
                required
                id="branchId"
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
                required
                id="location"
                label="Location"
                name="location"
                value={selectedBranch.location}
                onChange={handleInputChange}
                fullWidth
                margin="normal"
              />
              <TextField
                required
                id="img"
                label="Image"
                name="img"
                value={selectedBranch.img}
                onChange={handleInputChange}
                fullWidth
                margin="normal"
              />
              <TextField
                required
                id="name"
                label="Branch Name"
                name="name"
                value={selectedBranch.name}
                onChange={handleInputChange}
                fullWidth
                margin="normal"
              />
              <TextField
                required
                id="phone"
                label="Phone"
                name="phone"
                value={selectedBranch.phone}
                onChange={handleInputChange}
                fullWidth
                margin="normal"
              />
              <Box display="flex" flexDirection="column" alignItems="center" m="20px 0">
                <Box>Status</Box>
                <Radio.Group
                  value={selectedBranch.status}
                  onChange={handleStatusChange}
                  style={{ display: 'flex', justifyContent: 'center' }}
                >
                  <Radio value={true}>True</Radio>
                  <Radio value={false}>False</Radio>
                </Radio.Group>
              </Box>
            </Box>
          ) : (
            <Spin />
          )}
        </Modal>

        <Modal
          title={<span style={{ fontSize: '32px' }}>Add Branch</span>}
          open={addModalVisible}
          onCancel={() => setAddModalVisible(false)}
          footer={null}
        >
          <Box
            component="form"
            sx={{
              '& .MuiTextField-root': { m: 1, width: '100%' },
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center'
            }}
            noValidate
            autoComplete="off"
          >
            <div>
              <TextField
                required
                id="location"
                label="Location"
                value={newBranch.location}
                onChange={(e) => setNewBranch({ ...newBranch, location: e.target.value })}
                fullWidth
              />
              <TextField
                required
                id="img"
                label="Image"
                value={newBranch.img}
                onChange={(e) => setNewBranch({ ...newBranch, img: e.target.value })}
                fullWidth
              />
              <TextField
                required
                id="name"
                label="Branch Name"
                value={newBranch.name}
                onChange={(e) => setNewBranch({ ...newBranch, name: e.target.value })}
                fullWidth
              />
              <TextField
                required
                id="phone"
                label="Phone"
                value={newBranch.phone}
                onChange={(e) => setNewBranch({ ...newBranch, phone: e.target.value })}
                fullWidth
              />
              <Box display="flex" flexDirection="column" alignItems="center" m="20px 0">
                <Box>Status</Box>
                <Radio.Group
                  value={newBranch.status}
                  onChange={(e) => setNewBranch({ ...newBranch, status: e.target.value })}
                  style={{ display: 'flex', justifyContent: 'center' }}
                >
                  <Radio value={true}>True</Radio>
                  <Radio value={false}>False</Radio>
                </Radio.Group>
              </Box>
            </div>
            <Box display="flex" justifyContent="flex-end" m="20px 0">
              <Button variant="contained" color="primary" onClick={handleAddBranch}>
                Add
              </Button>
            </Box>
          </Box>
        </Modal>
      </Box>
      <ToastContainer />
    </ConfigProvider>
  );
};

export default Branch;
