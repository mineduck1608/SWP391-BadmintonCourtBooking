import { useState, useEffect } from "react";
import { Box, Button, Select, MenuItem } from "@mui/material";
import { Formik, Form, Field } from "formik";
import * as yup from "yup";
import Head from "../../Components/Head";
import './timeslot.css';

const validationSchema = yup.object().shape({
  branch: yup.string().required("Branch is required"),
  court: yup.string().required("Court is required"),
});

const generateTimeSlots = (startHour, endHour) => {
  const timeSlots = [];
  for (let hour = startHour; hour <= endHour; hour++) {
    const nextHour = hour === 24 ? 1 : hour + 1; // Handle 24h wrapping to 1h
    timeSlots.push({ time: `${hour}:00 - ${nextHour}:00`, status: "Available" });
  }
  return timeSlots;
};

const CourtForm = () => {
  const [branches, setBranches] = useState([]);
  const [courts, setCourts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [timeSlots, setTimeSlots] = useState(generateTimeSlots(1, 24));

  const [date, setDate] = useState('');
  const [bookings, setBookings] = useState([]);

  const handleDateChange = (e) => {
    setDate(e.target.value);
  };

  const fetchBookings = () => {
    if (!date) {
      alert('Vui lòng chọn ngày trước khi tìm kiếm');
      return;
    }

    fetch(`${apiBaseUrl}/api/bookings/date/${date}`)
      .then(response => response.json())
      .then(data => setBookings(data))
      .catch(error => console.error(error));
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        const branchResponse = await fetch("API_URL_FOR_BRANCHES");
        const courtResponse = await fetch("API_URL_FOR_COURTS");

        if (!branchResponse.ok || !courtResponse.ok) {
          throw new Error("Failed to fetch data");
        }

        const branchData = await branchResponse.json();
        const courtData = await courtResponse.json();

        setBranches(branchData);
        setCourts(courtData);
      } catch (error) {
        console.error("Error fetching data", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleFormSubmit = (values) => {
    console.log(values);
  };

  const handleStatusChange = (index, newStatus) => {
    setTimeSlots((prevTimeSlots) => {
      const updatedTimeSlots = [...prevTimeSlots];
      updatedTimeSlots[index].status = newStatus;
      return updatedTimeSlots;
    });
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  // Split timeSlots into four columns
  const columnSize = Math.ceil(timeSlots.length / 4);
  const columns = Array.from({ length: 4 }, (_, i) =>
    timeSlots.slice(i * columnSize, (i + 1) * columnSize)
  );

  return (
    <Box m="20px">
      <Head title="TIME SLOT" subtitle="Manage time slot of court" />
      <Formik
        initialValues={{ branch: "", court: "" }}
        validationSchema={validationSchema}
        onSubmit={handleFormSubmit}
      >
        {({ errors, touched, handleChange, handleBlur, values }) => (
          <Form>
            <div className="slot-form">
              <div>
                <div className="slot-form-row">
                  <label htmlFor="branch" className="slot-form-label">Court branch:</label>
                  <Field
                    as="select"
                    id="branch"
                    name="branch"
                    value={values.branch}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    className="slot-input-box-modal"
                  >
                    <option disabled selected hidden value="">
                      Please select branch
                    </option>
                    {branches.map((branch) => (
                      <option key={branch.branchId} value={branch.branchId}>
                        {branch.branchName}
                      </option>
                    ))}
                  </Field>
                </div>
                {errors.branch && touched.branch && <div className="error-branch">{errors.branch}</div>}
              </div>
              <div>
                <div className="slot-form-row">
                  <label htmlFor="court" className="slot-form-label-court">Court:</label>
                  <Field
                    as="select"
                    id="court"
                    name="court"
                    value={values.court}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    className="slot-input-box-modal"
                  >
                    <option disabled selected hidden value="">
                      Please select court
                    </option>
                    {courts.map((court) => (
                      <option key={court.courtId} value={court.courtId}>
                        {court.courtName}
                      </option>
                    ))}
                  </Field>
                </div>
                {errors.court && touched.court && <div className="error">{errors.court}</div>}
              </div>

              <button
                type="submit"
                variant="contained"
                color="primary"
                className="slot-submit-button"
              >
                Submit
              </button>
              <label>Chọn ngày:</label>
              <input type="date" value={date} onChange={handleDateChange} />
              <button onClick={fetchBookings}>Tìm kiếm</button>
            </div>
          </Form>
        )}
      </Formik>
      <div className="time-slot-container">
        {columns.map((column, colIndex) => (
          <div className="column" key={colIndex}>
            {column.map((slot, index) => (
              <div key={index} className="time-slot">
                <span>{slot.time}</span>
                <Select
                  value={slot.status}
                  onChange={(e) => handleStatusChange(colIndex * columnSize + index, e.target.value)}
                  className="status-select"
                >
                  <MenuItem value="Available">Available</MenuItem>
                  <MenuItem value="Booked">Booked</MenuItem>
                  <MenuItem value="Closed">Closed</MenuItem>
                </Select>
              </div>
            ))}
          </div>
        ))}
      </div>
    </Box>
  );
};

export default CourtForm;
