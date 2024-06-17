import { Box, Button } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState, useEffect } from "react";
import Head from "../../Components/Head";
import { Modal } from 'antd';
import './managetimeslot.css'; // Import the custom CSS
import { toast } from "react-toastify";
import { Spin, ConfigProvider } from 'antd';
import { useTheme } from "@mui/material";
import { tokens } from "../../theme";


const TimeSlotManagement = () => {
  const [rows, setRows] = useState([]);
  const token = sessionStorage.getItem('token');
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const [selectedRow, setSelectedRow] = useState(null);
  const [branches, setBranches] = useState([]);
  const [courts, setCourts] = useState([]); // State for courts
  const [slots, setSlots] = useState([]); // State for slots
  const [addFormState, setAddFormState] = useState();
  const [addOpen, setAddOpen] = useState(false);
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);

  const [branchesfilter, setBranchesFilter] = [branches, setBranches];
  const [courtsfilter, setCourtsFilter] = [courts, setCourts];
  const [slotsfilter, setSlotsFilter] = [slots, setSlots];

  console.log(branchesfilter)
  console.log(courtsfilter)


  // Define initial state values
  const initialState = {
    branch: '',
    court: '',
    startTime: '',
    endTime: ''
  };

  // Use a single state object to manage form fields
  const [formState, setFormState] = useState(initialState);

  const showModal = (row) => {
    setSelectedRow(row);
    setFormState({
      id: row.slotId,
      branch: row.branchId || '',
      court: row.courtId || '',
      startTime: row.startTime || '',
      endTime: row.endTime || ''
    });
    setOpen(true);
  };

  const handleOk = () => {
    const slotData = formState;
    fetch(`http://localhost:5266/Slot/Update?id=${slotData.id}&branchId=${slotData.branch}&courtId=${slotData.court}&startTime=${slotData.startTime}&endTime=${slotData.endTime}`, {
      method: "PUT",
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(slotData)
    })
      .then(response => response.json())
      .then(data => {
        toast.success('Slot updated successfully');
      })
      .catch(error => {
        console.error('Error updating slot:', error);
        toast.error('Failed to update slot');
      });
    setLoading(true);
    setTimeout(() => {
      setLoading(false);
      setOpen(false);
    }, 1000);
  };

  const handleCancel = () => {
    setOpen(false);
    setFormState(initialState);
  };

  const handleAddOk = () => {
    const newSlot = {
      branch: formState.branch,
      court: formState.court,
      startTime: formState.startTime,
      endTime: formState.endTime
    };
    fetch(`http://localhost:5266/Slot/Add`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newSlot)
    })
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to add slot');
        }
        return response.json();
      })
      .then(data => {
        toast.success('Slot added successfully');
      })
      .catch(error => {
        console.error('Error adding slot:', error);
        toast.error('Failed to add slot');
      });
    setLoading(true);
    setTimeout(() => {
      setLoading(false);
      setAddOpen(false);
    }, 1000);
  };

  const handleAddCancel = () => {
    setAddOpen(false);
    setFormState(initialState);
  };

  const addSlot = () => {
    setAddOpen(true);
  };

  useEffect(() => {
    if (!token) {
      console.error('Token not found. Please log in.');
      return;
    }

    const fetchData = async () => {
      try {
        const [branchesRes, courtsRes, slotsRes] = await Promise.all([
          fetch(`http://localhost:5266/Branch/GetAll`, {
            method: "GET",
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          }),
          fetch(`http://localhost:5266/Court/GetAll`, {
            method: "GET",
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          }),
          fetch(`http://localhost:5266/Slot/GetAll`, {
            method: "GET",
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          })
        ]);

        if (!branchesRes.ok || !courtsRes.ok || !slotsRes.ok) {
          throw new Error('Failed to fetch data');
        }

        const [branchesData, courtsData, slotsData] = await Promise.all([
          branchesRes.json(),
          courtsRes.json(),
          slotsRes.json()
        ]);

        setBranches(branchesData);
        setCourts(courtsData);

        const formattedData = slotsData.map((row, index) => {
          const court = courtsData.find(court => court.courtId === row.courtId);
          const branch = branchesData.find(branch => branch.branchId === court.branchId);

          return {
            id: index + 1,
            ...row,
            branchName: branch ? branch.branchName : 'Unknown',
            courtName: court ? court.courtName : 'Unknown'
          };
        });

        setSlots(slotsData);
        setRows(formattedData);
      } catch (error) {
        console.error('Error fetching data:', error);
      }
    };

    fetchData();
  }, [token]);

  const handleBranchChange = (value) => {
    console.log(value);
    const selectedBranch = branches.find(branch => branch.branchId === value);
    setFormState({ ...formState, branch: value });

    if (selectedBranch) {
      // Filter courts based on selected branch
      const filteredCourts = courts.filter(court => court.branchId === value);
      setCourtsFilter(filteredCourts);

      // Filter slots based on selected branch
      const filteredSlots = slots.filter(slot => {
        const court = filteredCourts.find(court => court.courtId === slot.courtId);
        return court && court.branchId === value;
      });

      // Update rows with filtered slots
      const formattedData = filteredSlots.map((row, index) => ({
        id: index + 1,
        ...row,
        branchName: selectedBranch.branchName,
        courtName: filteredCourts.find(court => court.courtId === row.courtId)?.courtName || 'Unknown'
      }));
      console.log(formattedData)
      setRows(formattedData);
    } else {
      setCourts([]);
      setRows([]);
    }
  };

  const handleCourtChange = (value) => {
    setFormState({ ...formState, court: value });

    if (!value) {
      // If no court selected, show slots for all courts in the selected branch
      const filteredSlots = slots.filter(slot => {
        const court = courts.find(court => court.courtId === slot.courtId);
        return court && court.branchId === formState.branch;
      });

      // Update rows with filtered slots
      const formattedData = filteredSlots.map((row, index) => ({
        id: index + 1,
        ...row,
        branchName: branches.find(branch => branch.branchId === formState.branch)?.branchName || 'Unknown',
        courtName: courts.find(court => court.courtId === row.courtId)?.courtName || 'Unknown'
      }));

      setRows(formattedData);
    } else {
      // Filter slots based on selected court
      const filteredSlots = slots.filter(slot => slot.courtId === value);

      // Update rows with filtered slots
      const formattedData = filteredSlots.map((row, index) => ({
        id: index + 1,
        ...row,
        branchName: branches.find(branch => branch.branchId === formState.branch)?.branchName || 'Unknown',
        courtName: courts.find(court => court.courtId === row.courtId)?.courtName || 'Unknown'
      }));

      setRows(formattedData);
    }
  };

  const handleDelete = (id) => {
    fetch(`http://localhost:5266/Slot/Delete?id=${id}`, {
      method: "DELETE",
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    })
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to delete slot');
        }
        return response.json();
      })
      .then(data => {
        toast.success('Slot deleted successfully.');
        setRows(prevRows => prevRows.filter(row => row.id !== id));
      })
      .catch(error => {
        console.error('Failed to delete slot:', error);
        toast.error('Failed to delete slot');
      });
  };

  const columns = [
    { field: "slotId", headerName: "ID", align: "center", headerAlign: "center" },
    { field: "branchName", headerName: "Branch", flex: 1, align: "center", headerAlign: "center" },
    { field: "courtName", headerName: "Court", flex: 1, align: "center", headerAlign: "center" }, // Add courtName column
    { field: "startTime", headerName: "Start Time", flex: 1, align: "center", headerAlign: "center" },
    { field: "endTime", headerName: "End Time", flex: 1, align: "center", headerAlign: "center" },
    {
      field: "actions",
      headerName: "Actions",
      sortable: false,
      flex: 1,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Box>
          <Button
            type="primary"
            onClick={() => showModal(params.row)}
            variant="contained"
            color="primary"
            size="small"
            className="managetimeslot-action-button"
          >
            Edit
          </Button>
          <Button
            variant="contained"
            size="small"
            onClick={() => handleDelete(params.row.id)}
            style={{ backgroundColor: '#b22222', color: 'white', marginLeft: 8 }}
          >
            Delete
          </Button>
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
        <Head title="Time Slots" subtitle="Managing Time Slots" />

        <div className="timeslotmanage-filter">
          <label htmlFor="" className="timeslotmanage-filter-branch">
            Branch:
          </label>
          <select
            value={formState.branch}
            onChange={(e) => handleBranchChange(e.target.value)}
            className="timeslotmanage-filter-branch-input-box-modal"
          >
            <option disabled selected hidden value="">
              {selectedRow ? selectedRow.branchName : ''}
            </option>
            {branches.map((branch) => (
              <option key={branch.branchId} value={branch.branchId}>
                {branch.branchName}
              </option>
              
            ))}
          </select>

          <label htmlFor="" className="timeslotmanage-filter-court">
            Court:
          </label>
          <select
            value={formState.court}
            onChange={(e) => handleCourtChange(e.target.value)}
            className="timeslotmanage-filter-court-input-box-modal"
          >
            <option disabled selected hidden value="">
              {selectedRow ? selectedRow.courtName : ''}
            </option>
            {courts.map((court) => (
              <option key={court.courtId} value={court.courtId}>
                {court.courtName}
              </option>
            ))}
          </select>
          <label htmlFor="" className="timeslotmanage-filter-date">
            Date:
          </label>
          <input type="date" className="timeslotmanage-filter-date-input" />

          <button
            className="timeslot-flex"
            type="primary"
            onClick={addSlot}
            variant="contained"
            color="primary"
            size="small"
          >
            Add Time Slot
          </button>
          <Modal
            width={800}
            open={open}
            title="Edit Time Slot"
            onOk={handleOk}
            onCancel={handleCancel}
            className="managetimeslot-custom-modal"
            footer={[
              <Button key="back" onClick={handleCancel} className="managetimeslot-button-hover-black">
                Cancel
              </Button>,
              <Button key="submit" type="primary" loading={loading} onClick={handleOk} className="managetimeslot-button-hover-black">
                Update
              </Button>
            ]}
            centered
          >
            <form>
              <div className="managetimeslot-timeslot-modal">
                <div className="managetimeslot-timeslot-modal-item">
                  <div className="managetimeslot-timeslot-modal-item-label">
                    <p>Branch:</p>
                    <p>Court:</p>
                    <p>Start Time:</p>
                    <p>End Time:</p>
                  </div>
                  <div className="managetimeslot-timeslot-modal-item-value">
                    <select
                      value={formState.branch}
                      onChange={(e) => handleBranchChange(e.target.value)}
                      className="managetimeslot-input-box-modal"
                    >
                      <option disabled selected hidden value="">
                        Select branch
                      </option>
                      {branches.map((branch) => (
                        <option key={branch.branchId} value={branch.branchId}>
                          {branch.branchName}
                        </option>
                      ))}
                    </select>

                    <select
                      value={formState.court}
                      onChange={(e) => handleCourtChange(e.target.value)}
                      className="managetimeslot-input-box-modal"
                    >
                      <option disabled selected hidden value="">
                        Select court
                      </option>
                      {courts.map((court) => (
                        <option key={court.courtId} value={court.courtId}>
                          {court.courtName}
                        </option>
                      ))}
                    </select>

                    <input
                      value={formState.startTime}
                      onChange={(e) =>
                        setFormState({ ...formState, startTime: e.target.value })
                      }
                      className="managetimeslot-input-box-modal"
                      type="time"
                    />
                    <input
                      value={formState.endTime}
                      onChange={(e) =>
                        setFormState({ ...formState, endTime: e.target.value })
                      }
                      className="managetimeslot-input-box-modal"
                      type="time"
                    />
                  </div>
                </div>
              </div>
            </form>
          </Modal>

        </div>

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
          <DataGrid rows={rows} columns={columns} getRowId={(row) => row.id} />
        </Box>
      </Box>
    </ConfigProvider>
  );
};

export default TimeSlotManagement;
