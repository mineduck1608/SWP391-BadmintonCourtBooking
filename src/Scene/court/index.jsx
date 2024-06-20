import { Box, useTheme, Button, TextField,Radio ,RadioGroup ,FormControlLabel, FormControl, FormLabel, MenuItem, Select, InputLabel} from "@mui/material";
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import React, { useState, useEffect } from 'react';
<<<<<<< HEAD
import { ToastContainer, toast } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';
import { ConfigProvider, Modal, Form, Input, InputNumber, Radio } from 'antd';
import { v4 } from 'uuid';
import { uploadBytes, getDownloadURL } from 'firebase/storage';
import { ref } from 'firebase/storage';
import { imageDb } from '../../Components/googleSignin/config.js';
=======
import { ConfigProvider, Modal, Spin } from 'antd';
import {toast, ToastContainer} from 'react-toastify';
>>>>>>> 4bff605d8ad0903701080150ec138d5e9d92fdca

const Court = () => {
    const theme = useTheme();
    const colors = tokens(theme.palette.mode);

    const [data, setData] = useState([]);
    const [branches, setBranches] = useState([null]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [modalVisible, setModalVisible] = useState(false);
    const [addModalVisible, setAddModalVisible] = useState(false); //Add court modal state
    const [selectedCourt, setSelectedCourt] = useState(null);
    const [formData, setFormData] = useState({
        courtId: '',
        courtImg: '',
        price: '',
        courStatus: '',
        description: '',
        branchName: '',
    });

<<<<<<< HEAD
    const [form] = Form.useForm();
    const [img, setImg] = useState(null);
=======
    const [newCourtData, setNewCourtData] = useState({
        courtImg: '',
        branchId: '',
        price: '',
        description: '',
    });
>>>>>>> 4bff605d8ad0903701080150ec138d5e9d92fdca

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
                setBranches(branches);
                setLoading(false);
            } catch (error) {
                setError(error.message);
                setLoading(false);
            }
        };

        fetchData();
        const interval = setInterval(fetchData, 1000); //Fetch data every 1000ms
        return () => clearInterval(interval); // Cleanup interval on component unmount
    }, []);

    const handleViewInfo = (id) => {
        const court = data.find((court) => court.courtId === id);
        setSelectedCourt(court);
        setFormData({
            courtId: court.courtId,
            courtImg: court.courtImg,
            price: court.price,
            courtStatus: court.courtStatus,
            description: court.description,
            branchName: court.branchName,
        });
        setModalVisible(true);
    };

    const handleDelete = (id) => {
        console.log(`Delete row with id: ${id}`);
    };

    const handleInputChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData({
            ...formData,
            [name]: type === 'checkbox' ? checked : value,
        });
    };

    const handleAddInputChange = (e) => {
        const{name, value} = e.target;
        setNewCourtData({
            ...newCourtData,
            [name]: value,
        });
    };

    const handleSave = async () => {
        const { courtId, courtImg, price, courtStatus, description, branchName } = formData;
        try {
            const response = await fetch(`http://localhost:5266/Court/Update?courtImg=${courtImg}&price=${price}&description=${description}&id=${courtId}&activeStatus=${courtStatus}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    courtId,
                    courtImg,
                    price: parseFloat(price),
                    description,
                    activeStatus: courtStatus,
                    branchName,
                }),
            });

            if (!response.ok) {
                throw new Error('Failed to update court');
            }

            toast.success('Court updated successfully!');
            setModalVisible(false);
        } catch (error) {
            console.error('Error:', error);
            toast.error('Failed to update court');
        }
    };

<<<<<<< HEAD
    const handleSubmit = async (values) => {
        const { courtImg, price, description, courtStatus } = values;
        const url = isAddMode
            ? `http://localhost:5266/Court/Add?courtImg=${courtImg}&branchId=${selectedCourt?.branchId || 'someDefaultBranchId'}&price=${price}&description=${description}`
            : `http://localhost:5266/Court/Update?courtImg=${courtImg}&price=${price}&description=${description}&id=${selectedCourt.courtId}&activeStatus=${courtStatus}`;

        try {
            const response = await fetch(url, {
                method: isAddMode ? 'POST' : 'PUT',
=======
    const handleAddCourt = async() => {
        const {courtImg, branchId, price, description} = newCourtData;
        try{
            const response = await fetch(`http://localhost:5266/Court/Add?courtImg=${courtImg}&branchId=${branchId}&price=${price}&description=${description}`, {
                method: 'POST',
>>>>>>> 4bff605d8ad0903701080150ec138d5e9d92fdca
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    courtImg,
                    branchId,
                    price: parseFloat(price),
                    description,
                }),
            });

            if(!response.ok) {
                throw new Error('Failed to add court');
            }

            toast.success('Court added successfully!');
            setAddModalVisible(false);
        } catch (error) {
            console.error('Error:', error);
            toast.error('Failed to add court');
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

    const handleClick = async (e) => {
        e.preventDefault();
        if (!img) {
            toast.error('No image selected');
            return;
        }
        const imgRef = ref(imageDb, `files/${v4()}`);
        try {
            await uploadBytes(imgRef, img);
            const url = await getDownloadURL(imgRef);
            const encodedUrl = encodeURIComponent(url);
            form.setFieldsValue({ courtImg: encodedUrl });
            toast.success('Image uploaded successfully');
        } catch (error) {
            console.error('Error uploading image:', error);
            toast.error('Image upload failed');
        }
    };
    const handleImageChange = (e) => {
        const file = e.target.files[0];
        if (file) {
            setImg(file);
        }
    };

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
                <Box display="flex" justifyContent="flex-start" mb={2}>
                    <Button variant="contained" color="primary" onClick={() => setAddModalVisible(true)}>
                        Add Court
                    </Button>
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
                    title={<span style={{ fontSize: '32px' }}>Court Info</span>}
                    open={modalVisible}
                    onCancel={() => setModalVisible(false)}
                    footer={[
                        <Button key="cancel" variant="contained" color="secondary" onClick={() => setModalVisible(false)}>
                            Cancel
                        </Button>,
                        <Button key="submit" variant="contained" color="primary"  onClick={handleSave}>
                            Save
                        </Button>
                    ]}
                >
                    {selectedCourt ? (
                        <Box>
                        <TextField
                            label="Court ID"
                            name="courtId"
                            value={formData.courtId}
                            InputProps={{
                                readOnly: true,
                            }}
                            fullWidth
                            margin="normal"
                        />
                        <TextField
                            label="Image URL"
                            name="courtImg"
<<<<<<< HEAD
                            label="Image"
                            rules={[{ required: true, message: 'Please input the image!' }]}
                        >
                            <div className="uploaded-branchimage-upload">
                                <input className="button-branch-input" type="file" onChange={handleImageChange} />
                                <button className="button upload" onClick={handleClick}>Upload</button>
                            </div>
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
=======
                            value={formData.courtImg}
                            onChange={handleInputChange}
                            fullWidth
                            margin="normal"
                        />
                        <TextField
>>>>>>> 4bff605d8ad0903701080150ec138d5e9d92fdca
                            label="Price"
                            name="price"
                            value={formData.price}
                            onChange={handleInputChange}
                            fullWidth
                            margin="normal"
                            type="number"
                        />
                        <FormControl component="fieldset" margin="normal">
                                <FormLabel component="legend">Status</FormLabel>
                                <RadioGroup
                                    row
                                    name="courtStatus"
                                    value={formData.courtStatus}
                                    onChange={handleInputChange}
                                >
                                    <FormControlLabel value="true" control={<Radio />} label="True" />
                                    <FormControlLabel value="false" control={<Radio />} label="False" />
                                </RadioGroup>
                            </FormControl>
                            <TextField
                                label="Description"
                                name="description"
                                value={formData.description}
                                onChange={handleInputChange}
                                fullWidth
                                margin="normal"
                            />
                            <TextField
                                label="Branch Name"
                                name="branchName"
                                value={formData.branchName}
                                InputProps={{
                                    readOnly: true,
                                }}
                                fullWidth
                                margin="normal"
                            />
                        </Box>
                    ) : (
                        <Spin />
                    )}
                </Modal>

                <Modal
                    title={<span style={{ fontSize: '32px' }}>Add Court</span>}
                    open={addModalVisible}
                    onCancel={() => setAddModalVisible(false)}
                    footer={[
                        <Button key="cancel" variant="contained" color="secondary" onClick={() => setAddModalVisible(false)}>
                            Cancel
                        </Button>,
                        <Button key="submit" variant="contained" color="primary" onClick={handleAddCourt}>
                            Add
                        </Button>
                    ]}
                >
                    <Box>
                        <TextField
                            label="Court Image URL"
                            name="courtImg"
                            value={newCourtData.courtImg}
                            onChange={handleAddInputChange}
                            fullWidth
                            margin="normal"
                        />
                        <FormControl fullWidth margin="normal">
                            <InputLabel id="branchId-label">Branch</InputLabel>
                            <Select
                                labelId="branchId-label"
                                name="branchId"
                                value={newCourtData.branchId}
                                onChange={handleAddInputChange}
                            >
                                {branches.map((branch) => (
                                    <MenuItem key={branch.branchId} value={branch.branchId}>
                                        {branch.branchName}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                        <TextField
                            label="Price"
                            name="price"
                            value={newCourtData.price}
                            onChange={handleAddInputChange}
                            fullWidth
                            margin="normal"
                            type="number"
                        />
                        <TextField
                            label="Description"
                            name="description"
                            value={newCourtData.description}
                            onChange={handleAddInputChange}
                            fullWidth
                            margin="normal"
                        />
                    </Box>
                </Modal>

                <ToastContainer />
            </Box>
        </ConfigProvider>
    );
};

export default Court;
