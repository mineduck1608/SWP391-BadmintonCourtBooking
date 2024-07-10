import React, { useState, useEffect } from "react";
import { Box, Button, Modal, Typography, TextField, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useTheme } from "@mui/material";
import { tokens } from "../../theme";
import Head from "../../Components/Head";
import { ConfigProvider } from 'antd';
import './discount.css';

const Discount = () => {
    const [rows, setRows] = useState([]);
    const [amount, setAmount] = useState('');
    const [proportion, setProportion] = useState('');
    const [open, setOpen] = useState(false);
    const [editRowId, setEditRowId] = useState(null);
    const [deleteRowId, setDeleteRowId] = useState(null);
    const [openConfirmDialog, setOpenConfirmDialog] = useState(false);
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
                const response = await fetch(`https://localhost:7233/Discount/GetAll`, {
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

                setRows(data);
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        fetchData();

        const intervalId = setInterval(fetchData, 100000);

        return () => clearInterval(intervalId);
    }, [token]);

    const handleSubmit = async (e) => {
        e.preventDefault();

        const url = editRowId 
            ? `https://localhost:7233/Discount/Update?id=${editRowId}&amount=${amount}&proportion=${proportion}`
            : `https://localhost:7233/Discount/Add?amount=${amount}&proportion=${proportion}`;
        const method = editRowId ? 'PUT' : 'POST';

        try {
            const response = await fetch(url, {
                method: method,
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error(editRowId ? 'Failed to update discount' : 'Failed to add discount');
            }

            console.log(editRowId ? 'Discount updated successfully' : 'Discount added successfully');

            // Refresh the data after adding/updating a discount
            const updatedResponse = await fetch(`https://localhost:7233/Discount/GetAll`, {
                method: "GET",
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            const updatedData = await updatedResponse.json();
            setRows(updatedData);
            setOpen(false); // Close the modal after submission
            setEditRowId(null); // Clear the edit row ID
            setAmount(''); // Clear the amount field
            setProportion(''); // Clear the proportion field

        } catch (error) {
            console.error('Error adding/updating discount:', error);
        }
    };

    const handleEdit = (row) => {
        setEditRowId(row.discountId);
        setAmount(row.amount);
        setProportion(row.proportion);
        setOpen(true);
    };

    const handleDelete = async () => {
        try {
            const response = await fetch(`https://localhost:7233/Discount/Delete?id=${deleteRowId}`, {
                method: "DELETE",
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Failed to delete discount');
            }

            console.log('Discount deleted successfully');

            // Refresh the data after deleting a discount
            const updatedResponse = await fetch(`https://localhost:7233/Discount/GetAll`, {
                method: "GET",
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            const updatedData = await updatedResponse.json();
            setRows(updatedData);

        } catch (error) {
            console.error('Error deleting discount:', error);
        }

        setOpenConfirmDialog(false); // Close the confirmation dialog
    };

    const handleDeleteClick = (rowId) => {
        setDeleteRowId(rowId);
        setOpenConfirmDialog(true);
    };

    const handleCloseConfirmDialog = () => {
        setOpenConfirmDialog(false);
        setDeleteRowId(null);
    };

    const handleAddClick = () => {
        setEditRowId(null);
        setAmount('');
        setProportion('');
        setOpen(true);
    };

    const columns = [
        { field: "discountId", headerName: "Discount ID", flex: 1, align: "center", headerAlign: "center" },
        { field: "amount", headerName: "Amount", flex: 1, align: "center", headerAlign: "center", type: 'number' },
        { field: "proportion", headerName: "Proportion", flex: 1, align: "center", headerAlign: "center", type: 'number' },
        {
            field: "actions",
            headerName: "Edit|Delete",
            flex: 1,
            align: "center",
            headerAlign: "center",
            renderCell: (params) => (
                <Box className="actions-cell" display="flex" justifyContent="center" alignItems="center" gap={1}>
                    <Button className="discount-page-edit-btn" onClick={() => handleEdit(params.row)} color="primary">Edit</Button>
                    <Button className="discount-page-delete-btn" onClick={() => handleDeleteClick(params.row.discountId)} color="secondary">Delete</Button>
                </Box>
            )
        }
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
                <Head title="Discount" subtitle="View discount" />
                <Button variant="contained" color="primary" onClick={handleAddClick}>
                    Add Discount
                </Button>
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
                        "& .actions-cell": {
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            height: '100%',
                        }
                    }}
                >
                    <DataGrid
                        rows={rows}
                        columns={columns}
                        getRowId={(row) => row.discountId}
                    />
                </Box>
            </Box>

            <Modal
                open={open}
                onClose={() => setOpen(false)}
                BackdropProps={{
                    style: {
                        backgroundColor: 'rgba(0, 0, 0, 0.1)'
                    }
                }}
            >
                <Box
                    component="form"
                    onSubmit={handleSubmit}
                    className="discount-page-form-container"
                >
                    <Typography variant="h6" component="h2">
                        {editRowId ? 'Edit Discount' : 'Add a New Discount'}
                    </Typography>
                    <TextField
                        label="Amount"
                        type="number"
                        value={amount}
                        onChange={(e) => setAmount(e.target.value)}
                        required
                        InputProps={{ inputProps: { min: 0, step: 0.01 } }}
                    />
                    <TextField
                        label="Proportion"
                        type="number"
                        value={proportion}
                        onChange={(e) => setProportion(e.target.value)}
                        required
                        InputProps={{ inputProps: { min: 0, step: 0.01 } }}
                    />
                    <Box display="flex" justifyContent="space-between">
                        <Button onClick={() => setOpen(false)} className="discount-page-button">Cancel</Button>
                        <Button type="submit" className="discount-page-button">{editRowId ? 'Update' : 'Add'}</Button>
                    </Box>
                </Box>
            </Modal>

            <Dialog
                open={openConfirmDialog}
                onClose={handleCloseConfirmDialog}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">{"Confirm Delete"}</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        Are you sure you want to delete this discount?
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseConfirmDialog} color="primary">
                        Cancel
                    </Button>
                    <Button onClick={handleDelete} color="secondary" autoFocus>
                        Delete
                    </Button>
                </DialogActions>
            </Dialog>
        </ConfigProvider>
    );
};

export default Discount;
