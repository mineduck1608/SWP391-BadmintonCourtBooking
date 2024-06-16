import React from 'react';
//import './ViewHistory.css';


const BookingHistory = () => {
  const bookings = [
    { date: '1/5/2024', court: 'c1', courtType: 'Indoor', time: '15:00-18:00', price: 'p1' },
    { date: '8/5/2024', court: 'c2', courtType: 'Outdoor', time: '15:00-16:30', price: 'p2' },
    { date: '28/4/2024', court: 'c3', courtType: 'Synthetic', time: 'Flexible (90/100 hrs left)', price: 'p3' },
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
            <th>Court Type</th>
            <th>Time</th>
            <th>Price</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {bookings.map((booking, index) => (
            <tr key={index}>
              <td>{booking.date}</td>
              <td>{booking.court}</td>
              <td>{booking.courtType}</td>
              <td>{booking.time}</td>
              <td>{booking.price}</td>
              <td className="actions">
                <button className="view-button">View</button>
                <button className="view-button">View</button>
                <button className="buy-more-time-button">Buy More Time</button>
                <button className="view-button">View</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default BookingHistory;
