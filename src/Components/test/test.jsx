// src/components/BookingsByDate.js
import React, { useState } from 'react';

const BookingsByDate = ({ apiBaseUrl }) => {
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

  return (
    <div>
      <h2>Danh sách các slot sân cầu lông đã được đặt trong ngày</h2>
      <label>Chọn ngày:</label>
      <input type="date" value={date} onChange={handleDateChange} />
      <button onClick={fetchBookings}>Tìm kiếm</button>
      
      <ul>
        {bookings.map(booking => (
          <li key={booking.bookingId}>
            <p>Sân: {booking.courtId}</p>
            <p>Thời gian bắt đầu: {new Date(booking.startTime).toLocaleTimeString()}</p>
            <p>Thời gian kết thúc: {new Date(booking.endTime).toLocaleTimeString()}</p>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default BookingsByDate;
