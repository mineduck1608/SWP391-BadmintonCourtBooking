import React, { useState, useEffect } from 'react';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import './ViewHistory.css';

export default function ViewHistory() {
  const [bookings, setBookings] = useState([]);
  const [slots, setSlots] = useState([]);
  const [courts, setCourts] = useState([]);
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const token = sessionStorage.getItem('token');

  useEffect(() => {
    if (!token) {
      console.error('Token not found. Please log in.');
      return;
    }

    const fetchBookings = async () => {
      try {
        const response = await fetch('https://localhost:7233/Booking/GetAll', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!response.ok) {
          throw new Error('Failed to fetch bookings');
        }

        const data = await response.json();
        setBookings(data);
      } catch (err) {
        setError(err.message);
      }
    };

    const fetchSlots = async () => {
      try {
        const response = await fetch('https://localhost:7233/Slot/GetAll', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!response.ok) {
          throw new Error('Failed to fetch slots');
        }

        const data = await response.json();
        setSlots(data);
      } catch (err) {
        setError(err.message);
      }
    };

    const fetchCourts = async () => {
      try {
        const response = await fetch('https://localhost:7233/Court/GetAll', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!response.ok) {
          throw new Error('Failed to fetch courts');
        }

        const data = await response.json();
        setCourts(data);
      } catch (err) {
        setError(err.message);
      }
    };

    const fetchBranches = async () => {
      try {
        const response = await fetch('https://localhost:7233/Branch/GetAll', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!response.ok) {
          throw new Error('Failed to fetch branches');
        }

        const data = await response.json();
        setBranches(data);
      } catch (err) {
        setError(err.message);
      }
    };

    const fetchData = async () => {
      setLoading(true);
      await fetchBookings();
      await fetchSlots();
      await fetchCourts();
      await fetchBranches();
      setLoading(false);
    };

    fetchData();
  }, [token]);

  const formatTime = (time) => {
    const hours = time.toString().padStart(2, '0');
    return `${hours}:00:00`;
  };

  const getBookingTypeLabel = (bookingType) => {
    switch (bookingType) {
      case 1:
        return 'Once';
      case 2:
        return 'Permanent';
      case 3:
        return 'Flexible';
      default:
        return 'unknown';
    }
  };

  const currentDate = new Date();

  const renderTableRows = (isUpcoming) => {
    const filteredBookings = bookings.filter((booking) => {
      const slot = slots.find(slot => slot.bookingId === booking.bookingId);
      const slotDate = slot ? new Date(slot.date) : null;
      return isUpcoming ? slotDate && slotDate >= currentDate : slotDate && slotDate < currentDate;
    });

    return filteredBookings.slice(0, 5).map((booking) => {
      const slot = slots.find(slot => slot.bookingId === booking.bookingId);
      const slotDate = slot ? new Date(slot.date).toLocaleDateString() : 'N/A';
      const startTime = slot ? formatTime(slot.start) : 'N/A';
      const endTime = slot ? formatTime(slot.end) : 'N/A';
      const courtId = slot ? slot.courtId : 'Unknown Court';
      const court = courts.find(court => court.courtId === courtId);
      const courtName = court ? court.courtName : 'Unknown Court';
      const branch = branches.find(branch => branch.branchId === (court ? court.branchId : null));
      const branchName = branch ? branch.branchName : 'Unknown Branch';

      return (
        <tr key={booking.bookingId}>
          <td>{booking.bookingId}</td>
          <td>{booking.amount}</td>
          <td>{getBookingTypeLabel(booking.bookingType)}</td>
          <td>{new Date(booking.bookingDate).toLocaleDateString()}</td>
          <td>{slotDate}</td>
          <td>{startTime}</td>
          <td>{endTime}</td>
          <td>{courtName}</td>
          <td>{branchName}</td>
        </tr>
      );
    });
  };

  return (
    <div className='view-history'>
      <div className='view-history-header'>
        <Header />
      </div>
      <div className='view-history-wrapper'>
        <div className='view-history-background'>
          <div className="view-history-profile-container">
            <div className="view-history-profile-content">
              <h2>Booking History</h2>
              <div className="view-history-booking-history">
                {loading ? (
                  <p>Loading bookings...</p>
                ) : error ? (
                  <p className="view-history-error-message">Error: {error}</p>
                ) : (
                  <div className="view-history-table">
                    <div className="upcoming-table">
                      <h3>Upcoming Bookings</h3>
                      <div className="view-history-table-wrapper">
                        <table className="view-history-booking-table">
                          <thead>
                            <tr>
                              <th>BOOKING ID</th>
                              <th>PRICE</th>
                              <th>BOOKING TYPE</th>
                              <th>BOOKING DATE</th>
                              <th>SLOT DATE</th>
                              <th>START TIME</th>
                              <th>END TIME</th>
                              <th>COURT NAME</th>
                              <th>BRANCH NAME</th>
                            </tr>
                          </thead>
                          <tbody>
                            {renderTableRows(true)}
                          </tbody>
                        </table>
                      </div>
                    </div>

                    <div className="past-table">
                      <h3>Past Bookings</h3>
                      <div className="view-history-table-wrapper">
                        <table className="view-history-booking-table view-history-booking-second-table">
                          <thead>
                            <tr>
                              <th>BOOKING ID</th>
                              <th>PRICE</th>
                              <th>BOOKING TYPE</th>
                              <th>BOOKING DATE</th>
                              <th>SLOT DATE</th>
                              <th>START TIME</th>
                              <th>END TIME</th>
                              <th>COURT NAME</th>
                              <th>BRANCH NAME</th>
                            </tr>
                          </thead>
                          <tbody>
                            {renderTableRows(false)}
                          </tbody>
                        </table>
                      </div>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className='view-history-footer'>
        <Footer />
      </div>
    </div>
  );
}
