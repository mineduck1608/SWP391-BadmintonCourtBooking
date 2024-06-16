import { Box, useTheme, Button } from "@mui/material";
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import React, { useState, useEffect } from 'react';
import { ConfigProvider, Modal, Descriptions, Spin } from 'antd';

const Court = () => {
    const theme = useTheme();
    const colors = tokens(theme.palette.mode);

    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [modalVisible, setModalVisible] = useState(false);
    const [selectedCourt, setSelectedCourt] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [courtResponse, branchResponse] = await Promise.all([
                    fetch('http://localhost:5266/Court/GetAll'),
                    fetch('http://localhost:5266/Branch/GetAll')
                ]);

                if (!courtResponse.ok || !branchResponse.ok) {
                    throw new Error('Failed to fetch data');
                }

                const courts = await courtResponse.json();
                const branches = await branchResponse.json();

                const branchMap = branches.reduce((acc, branch) => {
                    acc[branch.branchId] = branch.branchName;
                    return acc;
                }, {});

                const newData = courts.map((court, index) => ({
                    ...court,
                    id: index + 1,
                    branchName: branchMap[court.branchId]
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
        const court = data.find((court) => court.courtId === id);
        setSelectedCourt(court);
        setModalVisible(true);
    };

    const handleDelete = (id) => {
        console.log(`Delete row with id: ${id}`);
    };

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
            headerName: "Description",
            flex: 1,
            align: "center",
            headerAlign: "center",
        },
        {
            field: "branchName",
            headerName: "Branch Name",
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
                        onClick={() => handleViewInfo(params.row.courtId)}
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
                <Head title="COURTS" subtitle="List of Badminton Courts" />
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
                    title={<span style={{ fontSize: '32px' }}>Court Info</span>}
                    open={modalVisible}
                    onCancel={() => setModalVisible(false)}
                    footer={null}
                >
                    {selectedCourt ? (
                        <Box>
                            <Descriptions bordered column={1}>
                                <Descriptions.Item label="Court ID">{selectedCourt.courtId}</Descriptions.Item>
                                <Descriptions.Item label="Image">
                                    <img src={selectedCourt.courtImg} alt="court" style={{ width: '100%' }} />
                                </Descriptions.Item>
                                <Descriptions.Item label="Price">{selectedCourt.price}</Descriptions.Item>
                                <Descriptions.Item label="Status">{selectedCourt.courtStatus ? 'true' : 'false'}</Descriptions.Item>
                                <Descriptions.Item label="Description">{selectedCourt.description}</Descriptions.Item>
                                <Descriptions.Item label="Branch">{selectedCourt.branchName}</Descriptions.Item>
                                <Descriptions.Item label="Slots">{selectedCourt.slots.length}</Descriptions.Item>
                            </Descriptions>
                        </Box>
                    ) : (
                        <Spin />
                    )}
                </Modal>
            </Box>
        </ConfigProvider>
    );
};

export default Court;
