import { Box, useTheme, Button, TextField, Radio, RadioGroup, FormControlLabel, FormControl, FormLabel, MenuItem, Select, InputLabel } from "@mui/material";
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import React, { useState, useEffect } from 'react';
import 'react-toastify/dist/ReactToastify.css';
import { v4 } from 'uuid';
import { uploadBytes, getDownloadURL } from 'firebase/storage';
import { ref } from 'firebase/storage';
import { imageDb } from '../../Components/googleSignin/config.js';
import { ConfigProvider, Modal, Spin, Form } from 'antd';
import { toast, ToastContainer } from 'react-toastify';
import './court.css';

const Court = () => {
    const theme = useTheme();
    const colors = tokens(theme.palette.mode);

    const [data, setData] = useState([]);
    const [branches, setBranches] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [modalVisible, setModalVisible] = useState(false);
    const [addModalVisible, setAddModalVisible] = useState(false);
    const [selectedCourt, setSelectedCourt] = useState(null);
    const [formData, setFormData] = useState({
        courtId: '',
        courtImg: '',
        price: '',
        courtStatus: '',
        description: '',
        branchName: '',
    });
    const [newCourtData, setNewCourtData] = useState({
        courtImg: '',
        branchId: '',
        price: '',
        description: '',
    });
    const [imgs, setImgs] = useState([]);

    const [form] = Form.useForm();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [courtResponse, branchResponse] = await Promise.all([
                    fetch('https://localhost:7233/Court/GetAll'),
                    fetch('https://localhost:7233/Branch/GetAll')
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
        const interval = setInterval(fetchData, 1000);
        return () => clearInterval(interval);
    }, []);

    const handleViewInfo = (id) => {
        const court = data.find((court) => court.courtId === id);
        setSelectedCourt(court);
        setModalVisible(true);
    };

    const handleDelete = (id) => {
        console.log(`Delete row with id: ${id}`);
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setSelectedCourt((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    const handleAddInputChange = (e) => {
        const { name, value } = e.target;
        setNewCourtData({
            ...newCourtData,
            [name]: value,
        });
    };

    const handleSave = async () => {
        const { courtId, courtImg, price, courtStatus, description, branchName } = selectedCourt;
        try {
            const response = await fetch(`https://localhost:7233/Court/Update?courtImg=${courtImg}&description=${description}&id=${courtId}&activeStatus=${courtStatus}&price=${price}`, {
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

    const handleAddCourt = async () => {
        const { courtImg, branchId, price, description } = newCourtData;
        try {
            const response = await fetch(`https://localhost:7233/Court/Add?courtImg=${courtImg}&branchId=${branchId}&price=${price}&description=${description}`, {
                method: 'POST',
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

            if (!response.ok) {
                throw new Error('Failed to add court');
            }

            toast.success('Court added successfully!');
            setAddModalVisible(false);
        } catch (error) {
            console.error('Error:', error);
            toast.error('Failed to add court');
        }
    };

    const handleImageChange = (e) => {
        const files = Array.from(e.target.files);
        setImgs(files);
    };

    const handleClick = async (e) => {
        e.preventDefault();
        if (imgs.length === 0) {
            toast.error('No images selected');
            return;
        }

        const uploadedUrls = [];
        for (const img of imgs) {
            const imgRef = ref(imageDb, `files/${v4()}`);
            try {
                await uploadBytes(imgRef, img);
                const url = await getDownloadURL(imgRef);
                uploadedUrls.push(encodeURIComponent(url));
            } catch (error) {
                console.error('Error uploading image:', error);
                toast.error('Image upload failed');
                return;
            }
        }

        if (modalVisible) {
            setSelectedCourt((prevState) => ({ ...prevState, courtImg: uploadedUrls.join('|') }));
        } else {
            setNewCourtData((prevState) => ({ ...prevState, courtImg: uploadedUrls.join('|') }));
        }

        toast.success('Images uploaded successfully');
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
                    {params.value ? 'Active' : 'Maintenance'}
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
                        <Button key="cancel" variant="contained" color="primary" onClick={() => setModalVisible(false)}>
                            Cancel
                        </Button>,
                        <Button key="submit" variant="contained" color="secondary" onClick={handleSave} style={{ marginLeft: 8 }}>
                            Save
                        </Button>
                    ]}
                >
                    {selectedCourt ? (
                        <Box component="form" noValidate autoComplete="off">
                            <TextField
                                label="Court ID"
                                name="courtId"
                                value={selectedCourt.courtId}
                                InputProps={{
                                    readOnly: true,
                                }}
                                fullWidth
                                margin="normal"
                            />
                            <div className="uploaded-image-container">
                                <TextField
                                    label="Image URL"
                                    name="courtImg"
                                    value={selectedCourt.courtImg}
                                    InputProps={{
                                        readOnly: true,
                                    }}
                                    fullWidth
                                    margin="normal"
                                />
                            </div>
                            <div className="uploaded-courtimage-upload">
                                <input className="button-court-input" type="file" multiple onChange={handleImageChange} />
                                <button className="button upload" onClick={handleClick}>Upload</button>
                            </div>
                            <TextField
                                label="Price"
                                name="price"
                                value={selectedCourt.price}
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
                                    value={selectedCourt.courtStatus}
                                    onChange={handleInputChange}
                                >
                                    <FormControlLabel value="true" control={<Radio />} label="Active" />
                                    <FormControlLabel value="false" control={<Radio />} label="Maintenance" />
                                </RadioGroup>
                            </FormControl>
                            <TextField
                                label="Description"
                                name="description"
                                value={selectedCourt.description}
                                onChange={handleInputChange}
                                fullWidth
                                margin="normal"
                            />
                            <TextField
                                label="Branch Name"
                                name="branchName"
                                value={selectedCourt.branchName}
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
