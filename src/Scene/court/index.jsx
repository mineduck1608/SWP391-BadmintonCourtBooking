import { Box, useTheme, Button } from "@mui/material";
import { DataGrid } from '@mui/x-data-grid';
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import React, { useState, useEffect } from 'react';


const Court = () => {
    const theme = useTheme();
    const colors = tokens(theme.palette.mode);

    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch('http://localhost:5266/Court/GetAll');
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
        console.log('View info for row with id: ${id}');
    }

    const handleDelete = (id) => {
        console.log('Delete row with id: ${id}');
    }

    const columns = [
        {
            field: "courtId",
            headerName: "Court ID",
            align: "center",
            headerAlign: "center",
        },

        {
            field: "courtImg",
            headerName: "Img",
            flex: 1,
            align: "center",
            headerAlign: "center",
        },

        {
            field: "branchId",
            headerName: "Branch ID",
            flex: 1,
            align: "center",
            headerAlign: "center",
        },
        {
            field: "price",
            headerName: "Price",
            flex: 1,
            align: "center",
            headerAlign: "center",
        },
        {
            field: "courtStatus",
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
            field: "description",
            headerName: "description",
            flex: 1,
            align: "center",
            headerAlign: "center",
        },
        {
            field: "Branch",
            headerName: "Branch",
            flex: 1,
            align: "center",
            headerAlign: "center",
        },
        {
            field: "slots",
            headerName: "Slots",
            flex: 1,
            align: "center",
            headerAlign: "center",
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
                        onClick={() => handleViewInfo(params.row.id)}
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
        <Box m="20px">
            <Head
                title="COURTS"
                subtitle="List of Badminton Courts"
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
                />
            </Box>
        </Box>
    );
};

export default Court;
