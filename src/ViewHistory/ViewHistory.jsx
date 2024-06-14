import React from 'react';
import './BookingHistory.css';

const BookingHistory = () => {
  const bookings = [
    { date: '1/5/2024', court: 'c1', time: '15:00-18:00', price: 'p1' },
    { date: '8/5/2024', court: 'c2', time: '15:00-16:30', price: 'p2' },
    { date: '28/4/2024', court: 'c3', time: 'Flexible (90/100 hrs left)', price: 'p3' },
  ];

  return (
    <div className="booking-history">
      <header className="header">
        <h1>Booking History of: AAAAAAAAAAAAAAAA</h1>
      </header>
      <table className="booking-table">
        <thead>
          <tr>
            <th>Date</th>
            <th>Court</th>
            <th>Time</th>
            <th>Price</th>
          </tr>
        </thead>
        <tbody>
          {bookings.map((booking, index) => (
            <tr key={index}>
              <td>{booking.date}</td>
              <td>{booking.court}</td>
              <td>{booking.time}</td>
              <td>{booking.price}</td>
            </tr>
          ))}
        </tbody>
      </table>
      <div className="actions">
        <button className="view-button">View</button>
        <button className="view-button">View</button>
        <button className="buy-more-time-button">Buy More Time</button>
        <button className="view-button">View</button>
      </div>
    </div>
  );
};

export default BookingHistory;
