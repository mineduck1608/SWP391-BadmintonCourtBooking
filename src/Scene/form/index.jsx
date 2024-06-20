import { Box, Button } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState, useEffect } from "react";
import Head from "../../Components/Head";
import { Modal } from 'antd';
import './managetimeslot.css'; // Import the custom CSS
import { toast } from "react-toastify";
import { ConfigProvider } from 'antd';
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
  const [courts, setCourts] = useState([]);
  const [slots, setSlots] = useState([]);
  const [addFormState, setAddFormState] = useState({
    branchId: '',
    courtId: '',
    date: '',
    start: '',
    end: '',
    bookingId: '',
    bookedSlotId: ''
  });
  const [addOpen, setAddOpen] = useState(false);
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);

  const [branchesfilter, setBranchesFilter] = [branches, setBranches];
  const [courtsfilter, setCourtsFilter] = useState([]);

  const initialState = {
    branchId: '',
    courtId: '',
    date: '',
    start: '',
    end: '',
    bookingId: '',
    bookedSlotId: '',
    firstName: '',
    lastName: '',
    phone: '',
    email: '',
    img: ''
  };

  const [formState, setFormState] = useState(initialState);

  const showModal = async (row) => {
    try {
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

      setSelectedRow(row);
      setFormState({
        firstName: userDetail.firstName || '',
        lastName: userDetail.lastName || '',
        phone: userDetail?.phone || '',
        email: userDetail?.email || '',
        img: userDetail?.img || '',
        bookingId: booking ? booking.bookingId : 'N/A',
        bookedSlotId: booking ? booking.bookedSlotId : 'N/A'
      });
      setOpen(true);
    } catch (error) {
      console.error('Error fetching booking or user data:', error);
      toast.error('Failed to fetch booking or user data');
    }
  };

  const handleOk = () => {
    const slotData = formState;
    fetch(`http://localhost:5266/Slot/Update?id=${slotData.id}&branchId=${slotData.branchId}&courtId=${slotData.courtId}&date=${slotData.date}&start=${slotData.start}&end=${slotData.end}`, {
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
    if (!addFormState.branchId || !addFormState.courtId || !addFormState.date || !addFormState.start || !addFormState.end) {
      toast.error('All fields are required.');
      return;
    }

    const newSlot = {
      branchId: addFormState.branchId,
      courtId: addFormState.courtId,
      date: addFormState.date,
      start: addFormState.start,
      end: addFormState.end
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
        fetchData();
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
    setAddFormState({
      branchId: '',
      courtId: '',
      date: '',
      start: '',
      end: '',
      bookingId: '',
      bookedSlotId: ''
    });
  };

  const addSlot = () => {
    setAddOpen(true);
  };

  const fetchData = async () => {
    try {
      const [branchesRes, courtsRes, slotsRes, bookingsRes] = await Promise.all([
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
        }),
        fetch(`http://localhost:5266/Booking/GetAll`, {
          method: "GET",
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        })
      ]);

      if (!branchesRes.ok || !courtsRes.ok || !slotsRes.ok || !bookingsRes.ok) {
        throw new Error('Failed to fetch data');
      }

      const [branchesData, courtsData, slotsData, bookingsData] = await Promise.all([
        branchesRes.json(),
        courtsRes.json(),
        slotsRes.json(),
        bookingsRes.json()
      ]);

      setBranches(branchesData);
      setCourts(courtsData);
      setSlots(slotsData);
      console.log(branchesData)

  
      const formattedData = slotsData.map((row, index) => {
        const court = courts.find(court => court.courtId === row.courtId);
        const branch = branches.find(branch => branch.branchId === court.branchId);
        const booking = bookingsData.find(booking => booking.slotId === row.bookedSlotId);

        return {
          id: index + 1,
          ...row,
          branchName: branch ? branch.branchName : 'Unknown',
          courtName: court ? court.courtName : 'Unknown',
          date: dayjs(row.date).format('DD-MM-YYYY'),
          timeRange: `${row.start}:00 - ${row.end}:00`,
          totalPrice: booking ? booking.totalPrice : 'N/A',
          bookingId: booking ? booking.bookingId : 'N/A',
          bookedSlotId: booking ? booking.bookedSlotId : 'N/A'
        };
      });

      setSlots(slotsData);
      setRows(formattedData);
      console.log(formattedData)
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

  const handleBranchChange = (value) => {
    setAddFormState({ ...addFormState, branchId: value, courtId: '' });

    const filteredCourts = courts.filter(court => court.branchId === value);
    setCourtsFilter(filteredCourts);
  };

  const handleCourtChange = (value) => {
    setAddFormState({ ...addFormState, courtId: value });
  };

  const handleDateChange = async (dateValue) => {
    setFormState({ ...formState, date: dateValue });

    try {
      const [branchesRes, courtsRes, slotsRes, bookingsRes] = await Promise.all([
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
        }),
        fetch(`http://localhost:5266/Booking/GetAll`, {
          method: "GET",
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        })
      ]);

      if (!branchesRes.ok || !courtsRes.ok || !slotsRes.ok || !bookingsRes.ok) {
        throw new Error('Failed to fetch data');
      }

      const [branchesData, courtsData, slotsData, bookingsData] = await Promise.all([
        branchesRes.json(),
        courtsRes.json(),
        slotsRes.json(),
        bookingsRes.json()
      ]);

      setBranches(branchesData);
      setCourts(courtsData);
      setSlots(slotsData);

      const selectedBranchId = formState.branchId;
      const selectedCourtId = formState.courtId;

      let filteredSlots = slotsData.filter(slot => dayjs(slot.date).format('YYYY-MM-DD') === dateValue);

      if (selectedBranchId && selectedBranchId !== "all") {
        const filteredCourts = courtsData.filter(court => court.branchId === selectedBranchId);
        setCourtsFilter(filteredCourts);

        if (selectedCourtId && selectedCourtId !== "all") {
          filteredSlots = filteredSlots.filter(slot => slot.courtId === selectedCourtId);
        } else {
          filteredSlots = filteredSlots.filter(slot => {
            const court = filteredCourts.find(court => court.courtId === slot.courtId);
            return court;
          });
        }
      }

      const formattedData = filteredSlots.map((row, index) => {
        const court = courtsData.find(court => court.courtId === row.courtId);
        const branch = branchesData.find(branch => branch.branchId === court.branchId);
        const booking = bookingsData.find(booking => booking.slotId === row.bookedSlotId);
        console.log(court)
        return {
          id: index + 1,
          ...row,
          branchName: branch ? branch.branchName : 'Unknown',
          courtName: court ? court.courtName : 'Unknown',
          date: dayjs(row.date).format('DD-MM-YYYY'),
          timeRange: `${row.start}:00 - ${row.end}:00`,
          totalPrice: booking ? booking.totalPrice : 'N/A',
          bookingId: booking ? booking.bookingId : 'N/A',
          bookedSlotId: booking ? booking.bookedSlotId : 'N/A'
        };
      });
      

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
    { field: "courtName", headerName: "Court", flex: 1, align: "center", headerAlign: "center" },
    { field: "date", headerName: "Date", flex: 1, align: "center", headerAlign: "center" },
    { field: "timeRange", headerName: "Time Range", flex: 1, align: "center", headerAlign: "center" },
    {
      field: "totalPrice", headerName: "Total Price", flex: 1, align: "center", headerAlign: "center"
    },
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

  const hours = [...Array(24).keys()].map(i => i.toString().padStart(2, '0') + ':00');

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
        <Head title="Booking" subtitle="Court Booking Time Slots" />

        <div className="timeslotmanage-filter">
          <label htmlFor="" className="timeslotmanage-filter-branch">
            Branch:
          </label>
          <select
            value={formState.branchId}
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
            value={formState.courtId}
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
                <img
                  src={formState.img || 'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F22701a3e-720e-475d-aa47-c8c4040189e1?alt=media&token=e01180b0-300b-417f-9ef9-82fe648398d8'}
                  alt={`${formState.firstName} ${formState.lastName}`}
                  className="managetimeslot-user-info-image"
                />
                <div className="managetimeslot-user-info-details">
                  <p className="managetimeslot-user-info-text">First Name: {formState.firstName}</p>
                  <p className="managetimeslot-user-info-text">Last Name: {formState.lastName}</p>
                  <p className="managetimeslot-user-info-text">Phone: {formState.phone}</p>
                  <p className="managetimeslot-user-info-text">Email: {formState.email}</p>
                  <p className="managetimeslot-user-info-text">Booking ID: {formState.bookingId}</p>
                  <p className="managetimeslot-user-info-text">Booked Slot ID: {formState.bookedSlotId}</p>
                </div>
              </div>
            </div>
          </Modal>

          <Modal
            width={700}
            open={addOpen}
            title="Add Time Slot"
            onOk={handleAddOk}
            onCancel={handleAddCancel}
            className="managetimeslot-custom-modal"
            footer={[
              <button key="submit" onClick={handleAddOk} className="managetimeslot-button-hover-black-addslot">
                Add
              </button>,
              <button key="back" onClick={handleAddCancel} className="managetimeslot-button-hover-black-addslot">
                Cancel
              </button>
            ]}
            centered
          >
            <div className="managetimeslot-add-slot-modal">
              <div className="managetimeslot-add-slot-fields">
                <div className="managetimeslot-add-slot-label-time-row">
                  <div className="time-input">
                    <label htmlFor="branchId" className="managetimeslot-add-slot-label">Branch:</label>
                    <select
                      id="branchId"
                      value={addFormState.branchId}
                      onChange={(e) => handleBranchChange(e.target.value)}
                      className="managetimeslot-add-slot-input"
                      required
                    >
                      <option disabled selected hidden value="">Select branch</option>
                      {branches.map((branch) => (
                        <option key={branch.branchId} value={branch.branchId}>
                          {branch.branchName}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="time-input">
                    <label htmlFor="courtId" className="managetimeslot-add-slot-label">Court:</label>
                    <select
                      id="courtId"
                      value={addFormState.courtId}
                      onChange={(e) => handleCourtChange(e.target.value)}
                      className="managetimeslot-add-slot-input"
                      required
                      disabled={!addFormState.branchId}
                    >
                      <option disabled selected hidden value="">Select court</option>
                      {courtsfilter.map((court) => (
                        <option key={court.courtId} value={court.courtId}>
                          {court.courtName}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>
                <div className="managetimeslot-add-slot-label-time-row">
                  <div className="time-input">
                    <label htmlFor="date" className="managetimeslot-add-slot-label">Date:</label>
                    <input
                      type="date"
                      id="date"
                      value={addFormState.date}
                      onChange={(e) => setAddFormState({ ...addFormState, date: e.target.value })}
                      className="managetimeslot-add-slot-input"
                      required
                    />
                  </div>
                  <div className="time-input">
                    <label htmlFor="start" className="managetimeslot-add-slot-label">Start Time:</label>
                    <select
                      id="start"
                      value={addFormState.start}
                      onChange={(e) => setAddFormState({ ...addFormState, start: e.target.value })}
                      className="managetimeslot-add-slot-input time-select"
                      required
                    >
                      <option disabled selected hidden value="">Select start time</option>
                      {hours.map(hour => (
                        <option key={hour} value={hour}>{hour}</option>
                      ))}
                    </select>
                  </div>
                  <div className="time-input">
                    <label htmlFor="end" className="managetimeslot-add-slot-label">End Time:</label>
                    <select
                      id="end"
                      value={addFormState.end}
                      onChange={(e) => setAddFormState({ ...addFormState, end: e.target.value })}
                      className="managetimeslot-add-slot-input time-select"
                      required
                    >
                      <option disabled selected hidden value="">Select end time</option>
                      {hours.map(hour => (
                        <option key={hour} value={hour}>{hour}</option>
                      ))}
                    </select>
                  </div>
                </div>
                <div className="managetimeslot-add-slot-label-time-row">
                  <div className="time-input">
                    <label htmlFor="phone" className="managetimeslot-add-slot-label">Phone Number:</label>
                    <input
                      type="text"
                      id="phone"
                      value={addFormState.phone}
                      onChange={(e) => setAddFormState({ ...addFormState, phone: e.target.value })}
                      className="managetimeslot-add-slot-input"
                      required
                    />
                  </div>
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
    </ConfigProvider>
  );
};

export default TimeSlotManagement;
