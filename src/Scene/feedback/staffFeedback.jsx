import React, { useState, useEffect } from "react";
import { Box, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useTheme } from "@mui/material";
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import { ConfigProvider } from 'antd';
import {jwtDecode} from 'jwt-decode';

const StaffFeedback = () => {
    const [rows, setRows] = useState([]);
    const [branchId, setBranchId] = useState(null);
    const [userDetails, setUserDetails] = useState([]);
    const token = sessionStorage.getItem('token');
    const theme = useTheme();
    const colors = tokens(theme.palette.mode);

    useEffect(() => {
        if (!token) {
            console.error('Token not found. Please log in.');
            return;
        }

        const decodedToken = jwtDecode(token);
        const userIdToken = decodedToken.UserId;

        const fetchBranchId = async () => {
            try {
                const response = await fetch(`https://localhost:7233/User/GetAll`, {
                    method: "GET",
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch user data');
                }

                const data = await response.json();
                console.log('Fetched user data:', data);

                const matchingUser = data.find(user => user.userId === userIdToken);
                if (matchingUser) {
                    setBranchId(matchingUser.branchId);
                } else {
                    console.error('No matching user found');
                }
            } catch (error) {
                console.error('Error fetching user data:', error);
            }
        };

        fetchBranchId();
    }, [token]);

    useEffect(() => {
        const fetchUserDetails = async () => {
            if (!token) return;

            try {
                const response = await fetch(`https://localhost:7233/UserDetail/GetAll`, {
                    method: "GET",
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch user details');
                }

                const data = await response.json();
                console.log('Fetched user details:', data);
                setUserDetails(data);
            } catch (error) {
                console.error('Error fetching user details:', error);
            }
        };

        fetchUserDetails();
    }, [token]);

    useEffect(() => {
        const fetchData = async () => {
            if (!token ||!branchId) return;

            try {
                const response = await fetch(`https://localhost:7233/Feedback/GetAll`, {
                    method: "GET",
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch feedback data');
                }

                const data = await response.json();
                console.log('Fetched feedback data:', data);

                const filteredData = data.filter(feedback => feedback.branchId === branchId);
                const modifiedData = filteredData.map((feedback) => {
                    const user = userDetails.find((user) => user.userId === feedback.userId);
                    return {
                       ...feedback,
                        userId: user? `${user.firstName} ${user.lastName}` : 'Unknown User',
                    };
                });
                setRows(modifiedData);
            } catch (error) {
                console.error('Error fetching feedback data:', error);
            }
        };

        if (branchId) {
            fetchData();
        }

        const intervalId = setInterval(fetchData, 100000);

        return () => clearInterval(intervalId);
    }, [token, branchId]);

    const columns = [
        { field: "feedbackId", headerName: "Feedback ID", flex: 1, align: "center", headerAlign: "center" },
        { field: "userId", headerName: "User", flex: 1, align: "center", headerAlign: "center" },
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
                <Head title="Feedback" subtitle="View feedback for your branch" />
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

export default StaffFeedback;