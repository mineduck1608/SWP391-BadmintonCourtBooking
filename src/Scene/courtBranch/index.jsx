import { Box } from "@mui/material";
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import { useTheme } from "@mui/material";
import React, {useState, useEffect} from 'react';

const Branch = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);

  const[data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch('http://localhost:5266/Branch/GetAll');
        if (!response.ok) {
          throw new Error('Failed to fetch data');
        }
        const jsonData = await response.json();

        //Add a unique id to each row
        const newData = jsonData.map((row,index) => ({
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

  const columns = [
    { field: "branchId", 
      headerName: "Branch ID", 
      flex: 0.5,
      align: "center",
      headerAlign: "center",
    },
    { field: "location", 
      headerName: "Location",
      align: "center",
      headerAlign: "center",
    },
    {
      field: "branchName",
      headerName: "Branch Name",
      flex: 1,
      cellClassName: "name-column--cell",
      align: "center",
      headerAlign: "center",
    },
    {
      field: "branchPhone",
      headerName: "Phone",
      type: "number",
      headerAlign: "center",
      align: "center",
      flex: 1,
    },
    {
      field: "branchImg",
      headerName: "Image",
      flex: 1,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "branchStatus",
      headerName: "Status",
      flex: 1,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "courts",
      headerName: "Courts",
      flex: 1,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "feedbacks",
      headerName: "Feedbacks",
      flex: 1,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "users",
      headerName: "Users",
      flex: 1,
      align: "center",
      headerAlign: "center",
    },
  ];

  if(loading) {
    return <Box m="20px">Loading...</Box>
  }

  if (error) {
    return <Box m="20px">Error: {error}</Box>
  }

  return (
    <Box m="20px">
      <Head
        title="CONTACTS"
        subtitle="List of Contacts for Future Reference"
      />
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
          rowsPerPageOptions={[5,10,20]}
        />
      </Box>
    </Box>
  );
};

export default Branch;
