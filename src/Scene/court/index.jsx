import { Box, useTheme, Button } from "@mui/material";
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import React, { useState, useEffect } from 'react';
import { ToastContainer, toast } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';
import { ConfigProvider, Modal, Form, Input, InputNumber, Radio } from 'antd';

const Court = () => {
    const theme = useTheme();
    const colors = tokens(theme.palette.mode);

    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [modalVisible, setModalVisible] = useState(false);
    const [selectedCourt, setSelectedCourt] = useState(null);
    const [isAddMode, setIsAddMode] = useState(false);
    const [refresh, setRefresh] = useState(false);

    const [form] = Form.useForm();

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
    }, [refresh]);

    const handleViewInfo = (id) => {
        const court = data.find((court) => court.courtId === id);
        setSelectedCourt(court);
        setModalVisible(true);
        setIsAddMode(false);
        form.setFieldsValue({
            courtImg: court.courtImg,
            price: court.price,
            description: court.description,
            courtStatus: court.courtStatus,
            branchName: court.branchName
        });
    };

    const handleAddCourt = () => {
        setSelectedCourt(null);
        setModalVisible(true);
        setIsAddMode(true);
        form.resetFields();
    };

    const handleDelete = async (id) => {
        const court = data.find((court) => court.id === id);
        const url = `http://localhost:5266/Court/GetDeleted?courtId=${court.courtId}`;

        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(`Failed to delete court: ${errorData.message}`);
            }

            // Update the court status in the state
            setData((prevData) =>
                prevData.filter((c) => c.id !== id)
            );
            toast.success("Court Deleted Successfully");
        } catch (error) {
            console.error('Error:', error);
            toast.error("Failed to delete court");
        }
    };

    const handleSubmit = async (values) => {
        const { courtImg, price, description, courtStatus } = values;
        const url = isAddMode 
            ? `http://localhost:5266/Court/Add?courtImg=${courtImg}&branchId=${selectedCourt?.branchId || 'someDefaultBranchId'}&price=${price}&description=${description}`
            : `http://localhost:5266/Court/Update?courtImg=${courtImg}&price=${price}&description=${description}&id=${selectedCourt.courtId}&activeStatus=${courtStatus}`;

        try {
            const response = await fetch(url, {
                method: isAddMode ? 'POST' : 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(`Failed to ${isAddMode ? 'add' : 'update'} court: ${errorData.message}`);
            }

            setModalVisible(false);
            setRefresh(!refresh);
            toast.success(`Court ${isAddMode ? 'Added' : 'Updated'} Successfully`);
        } catch (error) {
            console.error('Error:', error);
            toast.error(`Failed to ${isAddMode ? 'add' : 'update'} court`);
        }
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
                <Button variant="contained" color="primary" onClick={handleAddCourt}>
                    Add Court
                </Button>
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
                    title={<span style={{ fontSize: '32px' }}>{isAddMode ? 'Add Court' : 'Edit Court'}</span>}
                    open={modalVisible}
                    onCancel={() => setModalVisible(false)}
                    footer={null}
                >
                    <Form form={form} onFinish={handleSubmit} layout="vertical">
                        <Form.Item
                            name="courtImg"
                            label="Image"
                            rules={[{ required: true, message: 'Please input the image!' }]}
                        >
                            <Input />
                        </Form.Item>
                        <Form.Item
                            name="courtStatus"
                            label="Status"
                            rules={[{ required: true, message: 'Please select the status!' }]}
                        >
                            <Radio.Group>
                                <Radio value={true}>true</Radio>
                                <Radio value={false}>false</Radio>
                            </Radio.Group>
                        </Form.Item>
                        <Form.Item
                            name="price"
                            label="Price"
                            rules={[{ required: true, message: 'Please input the price!' }]}
                        >
                            <InputNumber style={{ width: '100%' }} />
                        </Form.Item>
                        <Form.Item
                            name="description"
                            label="Description"
                            rules={[{ required: true, message: 'Please input the description!' }]}
                        >
                            <Input />
                        </Form.Item>
                        {!isAddMode && (
                            <Form.Item
                                name="branchName"
                                label="Branch Name"
                            >
                                <Input disabled />
                            </Form.Item>
                        )}
                        <Form.Item>
                            <Button type="primary" htmlType="submit">
                                Submit
                            </Button>
                        </Form.Item>
                    </Form>
                </Modal>
            </Box>
            <ToastContainer />
        </ConfigProvider>
    );
};

export default Court;
