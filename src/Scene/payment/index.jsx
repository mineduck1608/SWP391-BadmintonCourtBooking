import React, { useState, useEffect } from "react";
import { Box } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useTheme } from "@mui/material";
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import { ConfigProvider } from 'antd';
import { fetchWithAuth } from "../../Components/fetchWithAuth/fetchWithAuth";

const UserTable = () => {
    const [rows, setRows] = useState([]);
    const token = sessionStorage.getItem('token');
    const theme = useTheme();
    const colors = tokens(theme.palette.mode);

    useEffect(() => {
        if (!token) {
            console.error('Token not found. Please log in.');
            return;
        }

        const fetchData = async () => {
            try {
                const response = await fetchWithAuth(`https://localhost:7233/Payment/GetAll`, {
                    method: "GET",
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch data');
                }

                const data = await response.json();

                // Convert dates to readable format and split into date and time
                const formattedData = data.map(item => {
                    const dateObject = new Date(item.date);
                    return {
                        ...item,
                        date: dateObject.toLocaleDateString(),
                        time: dateObject.toLocaleTimeString(),
                        method: item.method === 1 ? 'Momo' : item.method === 2 ? 'VNPay' : 'Unknown'
                    };
                });

                setRows(formattedData);
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        fetchData();

        // Optionally set an interval to refresh data
        const intervalId = setInterval(fetchData, 1000);

        return () => clearInterval(intervalId);
    }, [token]);

    const columns = [
        { field: "paymentId", headerName: "Payment ID", flex: 1, align: "center", headerAlign: "center" },
        { field: "userId", headerName: "User ID", flex: 1, align: "center", headerAlign: "center" },
        { field: "date", headerName: "Date", flex: 1, align: "center", headerAlign: "center" },
        { field: "time", headerName: "Time", flex: 1, align: "center", headerAlign: "center" },
        { field: "bookingId", headerName: "Booking ID", flex: 1, align: "center", headerAlign: "center" },
        { field: "method", headerName: "Method", flex: 1, align: "center", headerAlign: "center" },
        { field: "amount", headerName: "Amount", flex: 1, align: "center", headerAlign: "center", type: 'number' }
    ];

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
                <Head title="Payment" subtitle="View payment" />
                <Box
                    height="75vh"
                    m="40px 0 0 0"
                    sx={{
                        "& .MuiDataGrid-root": {
                            border: "none",
                        },
                        "& .MuiDataGrid-cell": {
                            borderBottom: "none",
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
                        rows={rows}
                        columns={columns}
                        getRowId={(row) => row.paymentId} // Adjust this based on your data
                    />
                </Box>
            </Box>
        </ConfigProvider>
    );
};

export default UserTable;
