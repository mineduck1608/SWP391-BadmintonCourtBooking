import React, { useState, useEffect } from "react";
import { Box, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useTheme } from "@mui/material";
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import { ConfigProvider } from 'antd';

const Feedback = () => {
    const [rows, setRows] = useState([]);
    const [userDetails, setUserDetails] = useState([]); 
    const token = sessionStorage.getItem('token');
    const theme = useTheme();
    const colors = tokens(theme.palette.mode);
    const [branches, setBranches] = useState([]);

    useEffect(() => {
        if (!token) {
            console.error('Token not found. Please log in.');
            return;
        }

        const fetchData = async () => {
            try {
                const userDetailsResponse = await fetch(`https://localhost:7233/UserDetail/GetAll`, {
                    method: "GET",
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (!userDetailsResponse.ok) {
                    throw new Error('Failed to fetch user details');
                }

                const userDetailsData = await userDetailsResponse.json();
                setUserDetails(userDetailsData);

                const response = await fetch(`https://localhost:7233/Feedback/GetAll`, {
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
                console.log('Fetched data:', data);

                const branchResponse = await fetch(`https://localhost:7233/Branch/GetAll`, {
                    method: "GET",
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (!branchResponse.ok) {
                    throw new Error('Failed to fetch branch data');
                }

                const branchData = await branchResponse.json();
                setBranches(branchData);

                const modifiedData = data.map((feedback) => {
                    const user = userDetailsData.find((user) => user.userId === feedback.userId);
                    const branch = branchData.find((branch) => branch.branchId === feedback.branchId);
                    return {
                        ...feedback,
                        userId: user ? `${user.firstName} ${user.lastName}` : 'Unknown User',
                        branchName: branch ? branch.branchName : 'Unknown Branch'
                    };
                });

                setRows(modifiedData);
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        fetchData();

        const intervalId = setInterval(fetchData, 100000);

        return () => clearInterval(intervalId);
    }, [token]);

    const columns = [
        { field: "feedbackId", headerName: "Feedback ID", flex: 1, align: "center", headerAlign: "center" },
        { field: "userId", headerName: "User", flex: 1, align: "center", headerAlign: "center" },
        { field: "branchName", headerName: "Branch Name", flex: 1, align: "center", headerAlign: "center" },
        { field: "rating", headerName: "Rating", flex: 1, align: "center", headerAlign: "center", type: 'number' },
        { field: "content", headerName: "Content", flex: 2, align: "center", headerAlign: "center" },
        { field: "period", headerName: "Period", flex: 1, align: "center", headerAlign: "center" },   
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
                <Head title="Feedback" subtitle="View feedback" />
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
                        }
                    }}
                >
                    <DataGrid
                        rows={rows}
                        columns={columns}
                        getRowId={(row) => row.feedbackId}
                    />
                </Box>
            </Box>
        </ConfigProvider>
    );
};

export default Feedback;