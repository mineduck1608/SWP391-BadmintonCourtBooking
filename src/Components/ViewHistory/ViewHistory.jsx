import React, { useState, useEffect } from 'react';
import {jwtDecode} from 'jwt-decode';
import { Link } from 'react-router-dom';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import './ViewHistory.css';

const FeedbackModal = ({ show, onClose, onSave, bookingId, feedback }) => {
  const [newFeedback, setNewFeedback] = useState(feedback);

  useEffect(() => {
    setNewFeedback(feedback);
  }, [feedback]);

  const handleSave = () => {
    onSave(bookingId, newFeedback);
    onClose();
  };

  if (!show) {
    return null;
  }

  return (
    <div className="view-history-modal-overlay">
      <div className="view-history-modal">
        <div className="view-history-modal-content">
          <span className="view-history-close" onClick={onClose}>&times;</span>
          <h2>Feedback for Booking ID: {bookingId}</h2>
          <textarea
            value={newFeedback}
            onChange={(e) => setNewFeedback(e.target.value)}
            rows="5"
            cols="50"
          />
          <button onClick={handleSave}>Save</button>
        </div>
      </div>
    </div>
  );
};

export default function ViewHistory() {
  const [bookings, setBookings] = useState([]);
  const [slots, setSlots] = useState([]);
  const [courts, setCourts] = useState([]);
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [selectedBooking, setSelectedBooking] = useState(null);
  const token = sessionStorage.getItem('token');

  useEffect(() => {
    const fetchData = async () => {
      if (!token) {
        setError('Token not found. Please log in.');
        setLoading(false);
        return;
      }

      try {
        const decodedToken = jwtDecode(token);
        const userIdToken = decodedToken.UserId;

        const bookingsResponse = await fetch('https://localhost:7233/Booking/GetAll', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!bookingsResponse.ok) {
          throw new Error('Failed to fetch bookings');
        }

        const bookingsData = await bookingsResponse.json();
        const userBookings = bookingsData.filter(booking => booking.userId === userIdToken);
        setBookings(userBookings);

        const slotsResponse = await fetch('https://localhost:7233/Slot/GetAll', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!slotsResponse.ok) {
          throw new Error('Failed to fetch slots');
        }

        const slotsData = await slotsResponse.json();
        setSlots(slotsData);

        const courtsResponse = await fetch('https://localhost:7233/Court/GetAll', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!courtsResponse.ok) {
          throw new Error('Failed to fetch courts');
        }

        const courtsData = await courtsResponse.json();
        setCourts(courtsData);

        const branchesResponse = await fetch('https://localhost:7233/Branch/GetAll', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!branchesResponse.ok) {
          throw new Error('Failed to fetch branches');
        }

        const branchesData = await branchesResponse.json();
        setBranches(branchesData);

        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
      }
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
  const currentDateString = currentDate.toDateString();

  const handleFeedbackClick = (booking) => {
    setSelectedBooking(booking);
    setShowModal(true);
  };

  const handleSaveFeedback = (bookingId, feedback) => {
    setBookings((prevBookings) =>
      prevBookings.map((booking) =>
        booking.bookingId === bookingId ? { ...booking, feedback } : booking
      )
    );
  };

  const renderTableRows = (filterType) => {
    const filteredBookings = bookings.filter((booking) => {
      const slot = slots.find(slot => slot.bookingId === booking.bookingId);
      const slotDate = slot ? new Date(slot.date) : null;

      switch (filterType) {
        case 'today':
          return slotDate && slotDate.toDateString() === currentDateString;
        case 'upcoming':
          return slotDate && slotDate > currentDate;
        case 'past':
          return slotDate && slotDate < currentDate;
        default:
          return false;
      }
    });

    const sortedBookings = filteredBookings.sort((a, b) => {
      return new Date(b.bookingDate) - new Date(a.bookingDate); 
    });

    return sortedBookings.map((booking) => {
      const slot = slots.find(slot => slot.bookingId === booking.bookingId);
      const slotDate = slot ? new Date(slot.date).toLocaleDateString() : 'N/A';
      const startTime = slot ? formatTime(slot.start) : 'N/A';
      const endTime = slot ? formatTime(slot.end) : 'N/A';
      const courtId = slot ? slot.courtId : 'Unknown Court';
      const court = courts.find(court => court.courtId === courtId);
      const courtName = court ? court.courtName : 'Unknown Court';
      const branch = branches.find(branch => branch.branchId === (court ? court.branchId : null));
      const branchName = branch ? branch.branchName : 'Unknown Branch';
      const feedback = booking.feedback ? booking.feedback : 'N/A';

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
          <td>
            <button onClick={() => handleFeedbackClick(booking)}>
              {feedback}
            </button>
          </td>
        </tr>
      );
    });
  };

  const renderNoBookingsMessage = (filterType) => {
    const filteredBookings = bookings.filter((booking) => {
      const slot = slots.find(slot => slot.bookingId === booking.bookingId);
      const slotDate = slot ? new Date(slot.date) : null;

      switch (filterType) {
        case 'today':
          return slotDate && slotDate.toDateString() === currentDateString;
        case 'upcoming':
          return slotDate && slotDate > currentDate;
        case 'past':
          return slotDate && slotDate < currentDate;
        default:
          return false;
      }
    });

    if (filteredBookings.length === 0) {
      switch (filterType) {
        case 'today':
          return <p>Have no booking today, <Link to="../findCourt" className="view-history-book-now">Book now</Link> !!</p>;
        case 'upcoming':
          return <p>Have no upcoming booking, <Link to="../findCourt" className="view-history-book-now">Book now</Link> !!</p>;
        default:
          return null;
      }
    }

    return null;
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
                    <div className="view-history-today-table">
                      <h3>Today's Bookings</h3>
                      <div className="view-history-table-wrapper">
                        {renderNoBookingsMessage('today')}
                        <table className="view-history-today-booking-table">
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
                              <th>FEEDBACK</th>
                            </tr>
                          </thead>
                          <tbody>
                            {renderTableRows('today')}
                          </tbody>
                        </table>
                      </div>
                    </div>
  
                    <div className="view-history-upcoming-table">
                      <h3>Upcoming Bookings</h3>
                      <div className="view-history-table-wrapper">
                        {renderNoBookingsMessage('upcoming')}
                        <table className="view-history-upcoming-booking-table">
                          <thead>
                            <tr>
                              <th className="view-history-upcoming-table">BOOKING ID</th>
                              <th className="view-history-upcoming-table">PRICE</th>
                              <th className="view-history-upcoming-table">BOOKING TYPE</th>
                              <th className="view-history-upcoming-table">BOOKING DATE</th>
                              <th className="view-history-upcoming-table">SLOT DATE</th>
                              <th className="view-history-upcoming-table">START TIME</th>
                              <th className="view-history-upcoming-table">END TIME</th>
                              <th className="view-history-upcoming-table">COURT NAME</th>
                              <th className="view-history-upcoming-table">BRANCH NAME</th>
                              <th className="view-history-upcoming-table">FEEDBACK</th>
                            </tr>
                          </thead>
                          <tbody>
                            {renderTableRows('upcoming')}
                          </tbody>
                        </table>
                      </div>
                    </div>
  
                    <div className="view-history-past-table">
                      <h3>Past Bookings</h3>
                      <div className="view-history-table-wrapper">
                        <table className="view-history-past-booking-table">
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
                              <th>FEEDBACK</th>
                            </tr>
                          </thead>
                          <tbody>
                            {renderTableRows('past')}
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
      {selectedBooking && (
        <FeedbackModal
          show={showModal}
          onClose={() => setShowModal(false)}
          onSave={handleSaveFeedback}
          bookingId={selectedBooking.bookingId}
          feedback={selectedBooking.feedback}
        />
      )}
    </div>
  );
}

