import React, { useState, useEffect } from 'react';
import { Box, Button, TextField, Radio, RadioGroup, FormControlLabel, FormControl, FormLabel } from "@mui/material";
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import { useTheme } from "@mui/material";
import { Modal, Spin, ConfigProvider } from 'antd';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './discount.css';
import { jwtDecode } from 'jwt-decode';
import { fetchWithAuth } from '../../Components/fetchWithAuth/fetchWithAuth.jsx';

const Discount = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const [userRole, setUserRole] = useState('');

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [modalVisible, setModalVisible] = useState(false);
  const [selectedDiscount, setSelectedDiscount] = useState(null);
  const [addModalVisible, setAddModalVisible] = useState(false);
  const [newDiscount, setNewDiscount] = useState({
    amount: 0,
    proportion: 0,
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const discountResponse = await fetchWithAuth('https://localhost:7233/Discount/GetAll');
        if (!discountResponse.ok) {
          throw new Error('Failed to fetch discount data');
        }
        const discountData = await discountResponse.json();

        let token = sessionStorage.getItem('token');
        let decodedToken = jwtDecode(token);

        setUserRole(decodedToken.Role);

        const combinedData = discountData.map((discount, index) => ({
          ...discount,
          id: index + 1,
        }));

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
    const discount = data.find((discount) => discount.discountId === id);
    setSelectedDiscount(discount);
    setModalVisible(true);
  };

  const handleDelete = async (id) => {
    try {
      const response = await fetchWithAuth(`https://localhost:7233/Discount/Delete?id=${id}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
      });

      if (!response.ok) {
        throw new Error('Failed to delete discount');
      }

      toast.success('Discount deleted successfully!');
      setData(data.filter((item) => item.discountId !== id));
    } catch (error) {
      console.error('Error:', error);
      toast.error('Failed to delete discount');
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setSelectedDiscount({
      ...selectedDiscount,
      [name]: value,
    });
  };

  const handleRadioChange = (e) => {
    const { value } = e.target;
    setSelectedDiscount({
      ...selectedDiscount,
      isDelete: value === 'true' ? true : null,
    });
  };

  const handleAddInputChange = (e) => {
    const { name, value } = e.target;
    setNewDiscount({
      ...newDiscount,
      [name]: value,
    });
  };

  const token = sessionStorage.getItem('token');

  const handleSave = async () => {
    const { amount, proportion, discountId, isDelete } = selectedDiscount;
    console.log(selectedDiscount)
    try {
      const response = await fetchWithAuth(`https://localhost:7233/Discount/Update?amount=${amount}&proportion=${proportion}&id=${discountId}&isDelete=${isDelete}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
      });

      if (!response.ok) {
        throw new Error('Failed to update discount');
      }

      toast.success('Success');
      setModalVisible(false);
    } catch (error) {
      console.error('Error:', error);
      toast.error('Failed to update discount');
    }
  };

  const handleAddDiscount = async () => {
    const { amount, proportion } = newDiscount;
    try {
      const response = await fetchWithAuth(`https://localhost:7233/Discount/Add?amount=${amount}&proportion=${proportion}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify({
          amount,
          proportion,
        }),
      });

      if (!response.ok) {
        throw new Error('Failed to add discount');
      }
      toast.success('Discount added successfully!');
      setAddModalVisible(false);
    } catch (error) {
      console.error('Error:', error);
      toast.error('Failed to add discount');
    }
  };

  const columns = [
    { field: "discountId", headerName: "Discount ID", flex: 0.5, align: "center", headerAlign: "center" },
    { field: "amount", headerName: "Amount", align: "center", headerAlign: "center" },
    { field: "proportion", headerName: "Proportion", flex: 1, cellClassName: "name-column--cell", align: "center", headerAlign: "center" },
    {
      field: "isDelete",
      headerName: "Status",
      flex: 1,
      cellClassName: "name-column--cell",
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Box color={params.value ? 'red' : 'green'}>
            {params.value ? 'Disable' : 'Active'}
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
            onClick={() => handleViewInfo(params.row.discountId)}
          >
            Edit Info
          </Button>
          <Button
            variant="contained"
            style={{ backgroundColor: '#b22222', color: 'white' }}
            onClick={() => handleDelete(params.row.discountId)}
          >
            Delete
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
        <Head title="DISCOUNTS" subtitle="List of Discounts for Future Reference" />
        <Box display="flex" justifyContent="space-between" alignItems="center" marginTop="20px">
          {userRole === 'Admin' && (
            <Button variant="contained" color="primary" onClick={() => setAddModalVisible(true)}>
              Add Discount
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
          title={<span style={{ fontSize: '32px' }}>Discount Info</span>}
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
          {selectedDiscount ? (
            <Box component="form" noValidate autoComplete="off">
              <TextField
                label="Discount ID"
                name="discountId"
                value={selectedDiscount.discountId}
                InputProps={{
                  readOnly: true,
                }}
                fullWidth
                margin="normal"
              />
              <TextField
                label="Amount"
                name="amount"
                value={selectedDiscount.amount}
                onChange={handleInputChange}
                fullWidth
                margin="normal"
              />
              <TextField
                label="Proportion"
                name="proportion"
                value={selectedDiscount.proportion}
                onChange={handleInputChange}
                fullWidth
                margin="normal"
              />
              <FormControl component="fieldset" margin="normal">
                <FormLabel component="legend">Status</FormLabel>
                <RadioGroup
                  name="isDelete"
                  value={selectedDiscount.isDelete ? 'true' : 'null'}
                  onChange={handleRadioChange}
                >
                  <div className='discount-status'>
                  <FormControlLabel value="null" control={<Radio />} label="Active" />
                  <FormControlLabel value="true" control={<Radio style={{ color: '#b22222' }} />} label="Disable" />
                  </div>
                </RadioGroup>
              </FormControl>
            </Box>
          ) : (
            <Spin />
          )}
        </Modal>

        <Modal
          title={<span style={{ fontSize: '32px' }}>Add Discount</span>}
          open={addModalVisible}
          onCancel={() => setAddModalVisible(false)}
          footer={[
            <Button key="cancel" variant="contained" color="primary" onClick={() => setAddModalVisible(false)}>
              Cancel
            </Button>,
            <Button key="submit" variant="contained" color="secondary" onClick={handleAddDiscount} style={{ marginLeft: 8 }}>
              Save
            </Button>,
          ]}
        >
          <Box component="form" noValidate autoComplete="off">
            <TextField
              label="Amount"
              name="amount"
              value={newDiscount.amount}
              onChange={handleAddInputChange}
              fullWidth
              margin="normal"
            />
            <TextField
              label="Proportion"
              name="proportion"
              value={newDiscount.proportion}
              onChange={handleAddInputChange}
              fullWidth
              margin="normal"
            />
          </Box>
        </Modal>

        <ToastContainer theme='colored'/>
      </Box>
    </ConfigProvider>
  );
};

export default Discount;
