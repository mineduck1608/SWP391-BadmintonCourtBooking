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
import dayjs from 'dayjs';

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

  // Define initial state values
  const initialState = {
    branch: '',
    court: '',
    startTime: '',
    endTime: '',
    date: ''
  };

  // Use a single state object to manage form fields
  const [formState, setFormState] = useState(initialState);

  const showModal = async (row) => {
    try {
      // Fetch all bookings
      const bookingsRes = await fetch(`http://localhost:5266/Booking/GetAll`, {
        method: "GET",
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      if (!bookingsRes.ok) {
        throw new Error('Failed to fetch bookings');
      }

      const bookingsData = await bookingsRes.json();
      const booking = bookingsData.find(booking => booking.bookingId === row.bookingId);

      // Fetch user data using userId from the booking data
      const userRes = await fetch(`http://localhost:5266/User/GetById?id=${booking.userId}`, {
        method: "GET",
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      if (!userRes.ok) {
        throw new Error('Failed to fetch user details');
      }

      const userData = await userRes.json();

      // Fetch all user details
      const userDetailsRes = await fetch(`http://localhost:5266/UserDetail/GetAll`, {
        method: "GET",
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      if (!userDetailsRes.ok) {
        throw new Error('Failed to fetch user details');
      }

      const userDetailsData = await userDetailsRes.json();
      const userDetail = userDetailsData.find(detail => detail.userId === userData.userId);

      // Set form state with slot, user, and user detail information
      setSelectedRow(row);
      setFormState({
        firstName: userDetail.firstName || '',
        lastName: userDetail.lastName || '',
        phone: userDetail?.phone || '',
        email: userDetail?.email || '',
        img: userDetail?.img || ''
      });
      setOpen(true);
    } catch (error) {
      console.error('Error fetching booking or user data:', error);
      toast.error('Failed to fetch booking or user data');
    }
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
      setSlots(slotsData);

      const formattedData = slotsData.map((row, index) => {
        const court = courtsData.find(court => court.courtId === row.courtId);
        const branch = branchesData.find(branch => branch.branchId === court.branchId);

        return {
          id: index + 1,
          ...row,
          branchName: branch ? branch.branchName : 'Unknown',
          courtName: court ? court.courtName : 'Unknown',
          date: dayjs(row.startTime).format('DD-MM-YYYY'),
          timeRange: `${dayjs(row.startTime).format('HH:mm')} - ${dayjs(row.endTime).format('HH:mm')}`
        };
      });

      setSlots(slotsData);
      setRows(formattedData);
    } catch (error) {
      console.error('Error fetching data:', error);
    }
  };

  useEffect(() => {
    if (!token) {
      console.error('Token not found. Please log in.');
      return;
    }
    fetchData();
  }, [token]);

  const handleBranchChange = async (value) => {
    setFormState({ ...formState, branch: value, court: '' });

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
      setSlots(slotsData);

      const filteredCourts = courtsData.filter(court => court.branchId === value);
      setCourtsFilter(filteredCourts);

      if (value === "all") {
        const formattedData = slotsData.map((row, index) => {
          const court = courtsData.find(court => court.courtId === row.courtId);
          const branch = branchesData.find(branch => branch.branchId === court.branchId);

          return {
            id: index + 1,
            ...row,
            branchName: branch ? branch.branchName : 'Unknown',
            courtName: court ? court.courtName : 'Unknown',
            date: dayjs(row.startTime).format('DD-MM-YYYY'),
            timeRange: `${dayjs(row.startTime).format('HH:mm')} - ${dayjs(row.endTime).format('HH:mm')}`
          };
        });
        setRows(formattedData);
      } else {
        const selectedBranch = branchesData.find(branch => branch.branchId === value);
        const filteredSlots = slotsData.filter(slot => {
          const court = filteredCourts.find(court => court.courtId === slot.courtId);
          return court && court.branchId === value;
        });

        const formattedData = filteredSlots.map((row, index) => ({
          id: index + 1,
          ...row,
          branchName: selectedBranch ? selectedBranch.branchName : 'Unknown',
          courtName: filteredCourts.find(court => court.courtId === row.courtId)?.courtName || 'Unknown',
          date: dayjs(row.startTime).format('DD-MM-YYYY'),
          timeRange: `${dayjs(row.startTime).format('HH:mm')} - ${dayjs(row.endTime).format('HH:mm')}`
        }));

        setRows(formattedData);
      }
    } catch (error) {
      console.error('Error fetching data:', error);
    }
  };

  const handleCourtChange = async (value) => {
    try {
      // Set the new court value in formState
      setFormState({ ...formState, court: value });

      // Fetch fresh data for branches, courts, and slots
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

      // Check if all responses are ok
      if (!branchesRes.ok || !courtsRes.ok || !slotsRes.ok) {
        throw new Error('Failed to fetch data');
      }

      // Parse fetched data
      const [branchesData, courtsData, slotsData] = await Promise.all([
        branchesRes.json(),
        courtsRes.json(),
        slotsRes.json()
      ]);

      // Update state with fetched data
      setBranches(branchesData);
      setCourts(courtsData);
      setSlots(slotsData);

      // Filter courts based on the selected branch
      const selectedBranchId = formState.branch;
      const filteredCourts = courtsData.filter(court => court.branchId === selectedBranchId);
      setCourtsFilter(filteredCourts);

      // Handle displaying slots based on selected court
      if (value === "all") {
        const filteredSlots = slotsData.filter(slot => {
          const court = filteredCourts.find(court => court.courtId === slot.courtId);
          return court;
        });

        const formattedData = filteredSlots.map((row, index) => {
          const court = courtsData.find(court => court.courtId === row.courtId);
          const branch = branchesData.find(branch => branch.branchId === court.branchId);

          return {
            id: index + 1,
            ...row,
            branchName: branch ? branch.branchName : 'Unknown',
            courtName: court ? court.courtName : 'Unknown',
            date: dayjs(row.startTime).format('DD-MM-YYYY'),
            timeRange: `${dayjs(row.startTime).format('HH:mm')} - ${dayjs(row.endTime).format('HH:mm')}`
          };
        });
        setRows(formattedData);
      } else {
        // Filter slots based on selected court
        const filteredSlots = slotsData.filter(slot => slot.courtId === value);

        // Update rows with filtered slots
        const formattedData = filteredSlots.map((row, index) => ({
          id: index + 1,
          ...row,
          branchName: branchesData.find(branch => branch.branchId === formState.branch)?.branchName || 'Unknown',
          courtName: courtsData.find(court => court.courtId === row.courtId)?.courtName || 'Unknown',
          date: dayjs(row.startTime).format('DD-MM-YYYY'),
          timeRange: `${dayjs(row.startTime).format('HH:mm')} - ${dayjs(row.endTime).format('HH:mm')}`
        }));

        setRows(formattedData);
      }
    } catch (error) {
      console.error('Error fetching data:', error);
    }
  };

  const handleDateChange = async (dateValue) => {
    setFormState({ ...formState, date: dateValue });

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
      setSlots(slotsData);

      let filteredSlots;
      if (formState.branch === "all") {
        filteredSlots = slotsData.filter(slot => dayjs(slot.startTime).format('YYYY-MM-DD') === dateValue);
      } else {
        const selectedBranchId = formState.branch;
        const filteredCourts = courtsData.filter(court => court.branchId === selectedBranchId);
        setCourtsFilter(filteredCourts);

        if (formState.court === "all") {
          filteredSlots = slotsData.filter(slot => {
            const court = filteredCourts.find(court => court.courtId === slot.courtId);
            return court && dayjs(slot.startTime).format('YYYY-MM-DD') === dateValue;
          });
        } else {
          filteredSlots = slotsData.filter(slot => {
            const court = filteredCourts.find(court => court.courtId === slot.courtId);
            const slotDate = dayjs(slot.startTime).format('YYYY-MM-DD');
            return court && slot.courtId === formState.court && slotDate === dateValue;
          });
        }
      }

      const formattedData = filteredSlots.map((row, index) => ({
        id: index + 1,
        ...row,
        branchName: branchesData.find(branch => branch.branchId === row.branchId)?.branchName || 'Unknown',
        courtName: courtsData.find(court => court.courtId === row.courtId)?.courtName || 'Unknown',
        date: dayjs(row.startTime).format('DD-MM-YYYY'),
        timeRange: `${dayjs(row.startTime).format('HH:mm')} - ${dayjs(row.endTime).format('HH:mm')}`
      }));

      setRows(formattedData);
    } catch (error) {
      console.error('Error fetching data:', error);
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
    { field: "date", headerName: "Date", flex: 1, align: "center", headerAlign: "center" },
    { field: "timeRange", headerName: "Time Range", flex: 1, align: "center", headerAlign: "center" },
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
            User Info
          </Button>
          <Button
            variant="contained"
            size="small"
            onClick={() => handleDelete(params.row.id)}
            style={{ backgroundColor: '#b22222', color: 'white', marginLeft: 8 }}
          >
            Update
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
            <option disabled selected hidden value="Select branch">
              {selectedRow ? selectedRow.branchName : ''}
            </option>

            <option value="all">All</option>
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
            <option disabled selected hidden value="Select court">
              {selectedRow ? selectedRow.courtName : ''}
            </option>
            <option value="all">All</option>
            {courtsfilter.map((court) => (
              <option key={court.courtId} value={court.courtId}>
                {court.courtName}
              </option>
            ))}
          </select>
          <label htmlFor="" className="timeslotmanage-filter-date">
            Date:
          </label>
          <input
            type="date"
            value={formState.date}
            onChange={(e) => handleDateChange(e.target.value)}
            className="timeslotmanage-filter-date-input"
          />

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
            width={700}
            open={open}
            title="User Infomation"
            onOk={handleOk}
            onCancel={handleCancel}
            className="managetimeslot-custom-modal"
            footer={[
              <button key="back" onClick={handleCancel} className="managetimeslot-button-hover-black">
                Close
              </button>
            ]}
            centered
          >
            <div className="managetimeslot-timeslot-modal">
              <div className="managetimeslot-user-info">
                <img src={'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F22701a3e-720e-475d-aa47-c8c4040189e1?alt=media&token=e01180b0-300b-417f-9ef9-82fe648398d8'} alt={`${formState.firstName} ${formState.lastName}`} className="managetimeslot-user-info-image" />
                <div className="managetimeslot-user-info-details">
                  <p className="managetimeslot-user-info-text">First Name: {formState.firstName}</p>
                  <p className="managetimeslot-user-info-text">Last Name: {formState.lastName}</p>
                  <p className="managetimeslot-user-info-text">Phone: {formState.phone}</p>
                  <p className="managetimeslot-user-info-text">Email: {formState.email}</p>
                </div>
              </div>
            </div>
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
    </ConfigProvider >
  );
};

export default TimeSlotManagement;
