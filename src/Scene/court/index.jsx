import { Box, Typography, useTheme } from "@mui/material";
import { DataGrid } from '@mui/x-data-grid';
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import React, {useState, useEffect} from 'react';


const Court = () => {
    const theme = useTheme();
    const colors = tokens(theme.palette.mode);

    const[data, setData] = useState([]);
    const[loading, setLoading] = useState(true);
    const[error, setError] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch('http://localhost:5266/Court/GetAll');
                if(!response.ok) {
                    throw new Error('Failed to fetch data');
                }
                const jsonData = await response.json();
            } catch (error) {
                setError(error.message);
                setLoading(false);
            }
        };
        
        fetchData();
    }, []);

    const columns = [
        { field: "courtId", headerName: "Court ID" },

        {
            field: "courtImg",
            headerName: "Img",
            flex: 1,
            cellClassName: "name-column--cell",
        },

        {
            field: "branchId",
            headerName: "Branch ID",
            flex: 1,
        },
        {
            field: "price",
            headerName: "Price",
            flex: 1,
        },
        {
            field: "courtStatus",
            headerName: "Status",
            flex: 1,
            renderCell: (params) => (
                <Typography color={colors.greenAccent[500]}>
                    ${params.row.cost}
                </Typography>
            )
        },
    ];

    return (
        <Box m="20px">
            <Head
                title="INVOICES"
                subtitle="List of Invoice Balances"
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
                }}
            >
                <DataGrid
                    checkboxSelection
                    rows={data}
                    columns={columns}
                />
            </Box>
        </Box>
    );
};

export default Court;
