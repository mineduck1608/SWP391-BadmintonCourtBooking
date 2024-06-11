import React, { useState, useEffect } from 'react';
import { Box, Button } from "@mui/material";
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import { useTheme } from "@mui/material";
import { Modal, Spin, ConfigProvider } from 'antd';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { css } from '@ant-design/cssinjs';

const Branch = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [modalVisible, setModalVisible] = useState(false);
  const [selectedBranch, setSelectedBranch] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch('http://localhost:5266/Branch/GetAll');
        if (!response.ok) {
          throw new Error('Failed to fetch data');
        }
        const jsonData = await response.json();

        const newData = jsonData.map((row, index) => ({
          ...row,
          id: index + 1
        }));

        setData(newData);
        setLoading(false);
      } catch (error) {
        setError(error.message);
        setLoading(false);
      }
    };

    fetchData();
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
    { field: "courts", headerName: "Courts", flex: 1, align: "center", headerAlign: "center" },
    { field: "feedbacks", headerName: "Feedbacks", flex: 1, align: "center", headerAlign: "center" },
    { field: "users", headerName: "Users", flex: 1, align: "center", headerAlign: "center" },
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
            View Info
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
          title="Branch Info"
          open={modalVisible}
          onCancel={() => setModalVisible(false)}
          footer={null}
        >
          {selectedBranch ? (
            <Box>
              <p><strong>Branch ID:</strong> {selectedBranch.branchId}</p>
              <p><strong>Location:</strong> {selectedBranch.location}</p>
              <p><strong>Branch Name:</strong> {selectedBranch.branchName}</p>
              <p><strong>Phone:</strong> {selectedBranch.branchPhone}</p>
              <p><strong>Image:</strong> {selectedBranch.branchImg}</p>
              <p><strong>Status:</strong> {selectedBranch.branchStatus ? 'true' : 'false'}</p>
              <p><strong>Courts:</strong> {selectedBranch.courts}</p>
              <p><strong>Feedbacks:</strong> {selectedBranch.feedbacks}</p>
              <p><strong>Users:</strong> {selectedBranch.users}</p>
            </Box>
          ) : (
            <Spin />
          )}
        </Modal>
      </Box>
    </ConfigProvider>
  );
};

export default Branch;
