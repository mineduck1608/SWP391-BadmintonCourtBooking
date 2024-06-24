import React, { useState, useEffect } from "react";
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
  Select,
  Typography,
} from "@mui/material";
import Head from "../../Components/Head";
import './BadmintonCourtHours.css';
import { toast } from "react-toastify";


const BadmintonCourtHours = () => {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [workingHours, setWorkingHours] = useState({ start: '', end: '' });
  const [start, setStart] = useState('');
  const [end, setEnd] = useState('');

  const fetchWorkingHours = () => {
    fetch('https://localhost:7233/Slot/GetAll')
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to fetch slot data');
        }
        return response.json();
      })
      .then(data => {
        // Find the slot with bookedSlotId 'S1'
        const slotS1 = data.find(slot => slot.bookedSlotId === 'S1');
        if (!slotS1) {
          throw new Error('Slot with bookedSlotId "S1" not found');
        }

        // Extract start and end times from slotS1
        const { start, end } = slotS1;

        // Update state with the retrieved working hours
        setWorkingHours({ start: formatTime(start), end: formatTime(end) });
      })
      .catch(error => {
        console.error('Error fetching slot data:', error);
      });
  };
    fetchWorkingHours();



  const handleEditClick = () => {
    setStart(workingHours.start);
    setEnd(workingHours.end);
    setDialogOpen(true);
  };

  const handleCancel = () => {
    setStart(workingHours.start);
    setEnd(workingHours.end);
    setDialogOpen(false);
  };

  const handleSave = () => {
    const startHour = convertToHour(start);
    const endHour = convertToHour(end);
  
    // Prepare the data to send to the API
    const data = {
      start: startHour,
      end: endHour
    };
  
    fetch(`https://localhost:7233/Slot/UpdateOfficeHours?start=${data.start}&end=${data.end}`, {
      method: 'PUT', // or 'PUT' depending on your API
      headers: {
        'Content-Type': 'application/json',
        // Add any additional headers needed
      },
      body: JSON.stringify(data),
    })
    .then(response => {
      if (!response.ok) {
        throw new Error('Failed to update office hours');
      }
      return response.json();
    })
    .then(result => {
      toast.success(result.msg);
      setWorkingHours({ start, end });
      setDialogOpen(false); 
    })
    .catch(error => {
      toast.error(`Error: ${error.message}`);
    });
  };

  const convertToHour = (time) => {
    return parseInt(time.split(':')[0], 10);
  };

  const formatTime = (hour) => {
    return `${hour.toString().padStart(2, '0')}:00`;
  };

  const generateHourOptions = () => {
    const options = [];
    for (let i = 0; i < 24; i++) {
      const hour = i.toString().padStart(2, '0') + ':00';
      options.push(
        <MenuItem key={i} value={hour} style={{ textAlign: 'center' }}>
          {hour}
        </MenuItem>
      );
    }
    return options;
  };

  return (
    <Box m="20px">
      <Head title="BADMINTON COURT HOURS" subtitle="Edit the working hours of the court" />

      <Box className="timeslotwork-root">
        <Typography variant="h5" className="timeslotwork-heading">Working Hours</Typography>
        <Typography variant="h6" className="timeslotwork-hoursText">
          {`From: ${workingHours.start} To: ${workingHours.end}`}
        </Typography>
        <Button 
          variant="contained" 
          color="primary" 
          onClick={handleEditClick} 
          className="timeslotwork-editButton"
        >
          Edit Working Hours
        </Button>

        <Dialog open={dialogOpen} onClose={handleCancel}>
          <DialogTitle style={{ color: 'black' }}>Edit Working Hours</DialogTitle>
          <DialogContent className="timeslotwork-dialogContent">
            <Typography style={{ color: 'black', marginBottom: '8px' }}>Start Time</Typography>
            <Select
              fullWidth
              value={start}
              onChange={(e) => setStart(e.target.value)}
              style={{ color: 'black', marginBottom: '16px' }} // Ensure text is black
            >
              {generateHourOptions()}
            </Select>
            <Typography style={{ color: 'black', marginBottom: '8px' }}>End Time</Typography>
            <Select
              fullWidth
              value={end}
              onChange={(e) => setEnd(e.target.value)}
              style={{ color: 'black' }} // Ensure text is black
            >
              {generateHourOptions()}
            </Select>
          </DialogContent>
          <DialogActions className="timeslotwork-dialogActions">
            <Button onClick={handleCancel} color="primary">
              Cancel
            </Button>
            <Button onClick={handleSave} color="primary">
              Save
            </Button>
          </DialogActions>
        </Dialog>
      </Box>
    </Box>
  );
};

export default BadmintonCourtHours;
