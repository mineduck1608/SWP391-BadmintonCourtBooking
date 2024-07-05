import React, { useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import './ViewHistory.css';
import { Modal, Button, DatePicker } from 'antd'
import { Link } from 'react-router-dom';
import { HttpStatusCode } from 'axios';
import { getHours } from 'date-fns';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import CreateFeedbackModal from '../CreateFeedbackModal/CreateFeedbackModal';

export default function ViewHistory() {
  const [bookings, setBookings] = useState([]);
  const [slots, setSlots] = useState([]);
  const [courts, setCourts] = useState([]);
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [open, setOpen] = useState(false);
  const [openCancel, setOpenCancel] = useState(false);
  const [timeBound, setTimeBound] = useState([]) //0:00 to 23:00
  const [selectedBooking, setSelectedBooking] = useState({})
  const [showModal, setShowModal] = useState(false)
  const [validSchedule, setValidSchedule] = useState(false)
  const token = sessionStorage.getItem('token');
  const apiUrl = 'https://localhost:7233'
  const initialState = {
    start: '',
    end: '',
    date: '',
    courtId: '',
    slotId: '',
    bookingId: '',
    branchId: '',
    userId: '',
    paymentType: '',
  };
  const [formState, setFormState] = useState(initialState)
  const [payment, setPayment] = useState({})
  const navigate = useNavigate();

  const loadTimeFrame = async () => {
    const fetchTime = async () => {
      var res = await fetch(`${apiUrl}/Slot/GetAll`)
      var data = await res.json()
      return data
    }

    try {
      var data = await fetchTime()
      setTimeBound([])
      var primitive = data.find(d => d['bookedSlotId'] === 'S1')
      var start = primitive.start
      var end = primitive.end
      for (let i = start; i <= end; i++) {
        setTimeBound(t => [...t, i])
      }
    }
    catch (err) {
      toast.error('Server error')
    }
  }
  const formatNumber = (n) => {
    function formatTo3Digits(n, stop) {
      var rs = ''
      if (!stop)
        for (var i = 1; i <= 3; i++) {
          rs = (n % 10) + rs
          n = Math.floor(n / 10)
        }
      else rs = n + rs
      return rs
    }
    n = Math.floor(n)
    var rs = ''
    do {
      rs = formatTo3Digits(n % 1000, Math.floor(n / 1000) === 0) + rs
      n = Math.floor(n / 1000)
      if (n > 0) rs = '.' + rs
    }
    while (n > 0)
    return rs
  }

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

        const bookingsResponse = await fetch(`https://localhost:7233/Booking/GetByUser?id=${userIdToken}`, {
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

        const slotsResponse = await fetch(`${apiUrl}/Slot/GetAll`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!slotsResponse.ok) {
          throw new Error('Failed to fetch slots');
        }

        const slotsData = await slotsResponse.json();
        setSlots(slotsData);

        const courtsResponse = await fetch(`${apiUrl}/Court/GetAll`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!courtsResponse.ok) {
          throw new Error('Failed to fetch courts');
        }

        const courtsData = await courtsResponse.json();
        setCourts(courtsData);

        const branchesResponse = await fetch(`${apiUrl}/Branch/GetAll`, {
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
    loadTimeFrame()
  }, [token]);

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


      return (
        !booking.isDeleted &&
        <tr key={booking.bookingId}>
          <td>
            {booking.bookingId}

          </td>
          <td>{formatNumber(booking.amount)}</td>
          <td>{getBookingTypeLabel(booking.bookingType)}</td>
          <td>{new Date(booking.bookingDate).toLocaleDateString()}</td>
          <td>{slotDate}</td>
          <td>{startTime}</td>
          <td>{endTime}</td>
          <td>{courtName}</td>
          <td>{branchName}</td>
          {
            filterType == 'past' && (
              <td>
                <button className="vh-feedback-btn" onClick={() => handleFeedbackClick(booking.bookingId, branch.branchId, booking.userId)}>Feedback</button>
              </td>
            )
          }

          {
            filterType !== 'past' && (
              <td>
                <button className='view-history-button' onClick={() => onClickEdit(slot)}>Edit</button>
                <button className='view-history-button' onClick={() => onClickCancelSlot(slot)}>Cancel</button>
              </td>
            )
          }
        </tr>
      );
    });
  };

  const onClickEdit = (slot) => {
    setOpen(true)
    loadTimeFrame()
    let court = courts.find(c => c.courtId === slot.courtId)
    let branchId = court.branchId

    setFormState({
      date: slot.date,
      start: slot.start,
      end: slot.end,
      courtId: slot.courtId,
      bookingId: slot.bookingId,
      branchId: branchId,
      slotId: slot.bookedSlotId,
    })
  }
  const onClickCancelSlot = (slot) => {
    setOpenCancel(true)
    let court = courts.find(c => c.courtId === slot.courtId)
    let branchId = court.branchId

    setFormState({
      date: slot.date,
      start: slot.start,
      end: slot.end,
      courtId: slot.courtId,
      bookingId: slot.bookingId,
      branchId: branchId,
      slotId: slot.bookedSlotId,
    })
  }
  const formatTime = (time) => {
    const hours = time.toString().padStart(2, '0');
    return `${hours}:00:00`;
  };
  function getUserId(token) {
    var decodedToken = jwtDecode(token)
    return decodedToken.UserId
  }
  const getPayment = async (bookingID) => {
    async function fetchPayment(bookingID) {
      try {
        var res = await fetch(`${apiUrl}/Payment/GetByUser?id=${getUserId(token)}`,
          {
            method: 'get',
            headers: {
              Authorization: `Bearer ${token}`
            }
          })
        var data = await res.json()
        var payment = data.find(d => d.bookingId === bookingID)
        setPayment(payment)
      }
      catch (err) {
        toast.error('Server error')
      }
    }
    await fetchPayment(bookingID)
  }
  const handleOk = async () => {
    const update = async (start, end, date, userId, courtId, slotId, paymentMethod, bookingId) => {
      var res = await fetch(`${apiUrl}/Slot/UpdateByUser?`
        + `start=${start}&`
        + `end=${end}&`
        + `date=${date}&`
        + `userId=${userId}&`
        + `courtId=${courtId}&`
        + `slotId=${slotId}&`
        + `paymentMethod=${paymentMethod}&`
        + `bookingId=${bookingId}`
        , {
          method: 'put',
          headers: {
            'Authorization': `Bearer ${token}`
          }
        })
      if (res.ok) {
        var data = await res.json()
        if (Object.is(data['url'], undefined)) {
          toast.success('Saved')
          window.location.reload()
        }
        else {
          window.location.assign(data['url'])
        }
      }
      if (res.status === HttpStatusCode.BadRequest) {
        toast.error('Couldn\'t change')
      }
    }
    let t = validateTime(formState.date, formState.start, formState.end)
    setValidSchedule(t)
    try {
      if (t) {
        setOpen(false)
        getPayment(formState.bookingId)
        await update(formState.start, formState.end, formState.date, getUserId(token),
          formState.courtId, formState.slotId, payment.method, formState.bookingId
        )
        setFormState(initialState)
      }
      else {
        toast.error('Invalid time')
      }
    }
    catch (err) {

    }
  }

  const cancelSlot = async () => {
    async function cancel(slotId, bookingId) {
      return res = await fetch(`${apiUrl}/Slot/Cancel?`
        + `slotId=${slotId}&`
        + `bookingId=${bookingId}`
        , {
          method: 'delete',
          headers: {
            Authorization: `Bearer ${token}`
          }
        })
    }
    try {
      var res = await cancel(formState.slotId, formState.bookingId)
      if (res.ok)
        window.location.reload()
      else {
        toast.error('Couldn\'t change')
      }
    }
    catch (err) {
      toast.error('Server error')
    }
  }

  const handleCancel = () => {
    setOpen(false)
    setOpenCancel(false)
    setFormState(initialState)
  }

  const getBookingTypeLabel = (bookingType) => {
    switch (bookingType) {
      case 1:
        return 'Once';
      case 2:
        return 'Fixed';
      case 3:
        return 'Flexible';
      default:
        return 'Buy Time';
    }
  };

  const currentDate = new Date();
  const currentDateString = currentDate.toDateString();

  const [feedbackModalVisible, setFeedbackModalVisible] = useState(false);
  const [feedbackData, setFeedbackData] = useState({});
  const handleFeedbackClick = (bookingId, branchId, userId) => {
    setFeedbackData({ bookingId, branchId, userId });
    setFeedbackModalVisible(true);
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
  const validateTime = (date, start, end) => {
    let now = Date.parse(new Date())
    let startTime = Date.parse(date) + start * 3600000
    let endTime = Date.parse(date) + end * 3600000
    return (startTime < endTime) && (startTime > now)
  }
  return (
    <div className='view-history'>
      <Modal title='Edit slot'
        open={open}
        footer={null}
        closable={false}
        centered={true}
      >
        <span>
          <p>Booking: {formState.bookingId}</p>
          <p>Slot: {formState.slotId}</p>
          <input type="date" id="datePicker" value={formState.date}
            onChange={() => setFormState({
              ...formState,
              date: document.getElementById('datePicker').value
            })}
          />
          <p>Starting time:</p>
          <select id='startingTime'
            onChange={() => {
              setFormState({
                ...formState,
                start: document.getElementById('startingTime').value
              })
            }}
          >
            {timeBound.map(t => (
              <option value={t} selected={t === formState.start}>{t}:00:00</option>
            ))}
          </select>
          <p>Ending time:</p>
          <select id='endingTime'
            onChange={() => setFormState({
              ...formState,
              end: document.getElementById('endingTime').value
            })}
          >
            {timeBound.map(t => (
              <option value={t} selected={t === formState.end}>{t}:00:00</option>
            ))}
          </select>
          Branch:
          <select id='branch'>
            {branches.map((b, i) => (
              b.branchStatus === 1 &&
              <option value={b.branchId} selected={b.branchId === formState.branchId}>{b.branchName}</option>
            ))}
          </select>
          Court:
          <select id='court'>
            {courts.map(c => (
              c.courtStatus && c.branchId === formState.branchId
              &&
              (
                <option value={c.courtId} selected={c.courtId === formState.courtId}>{c.courtName}</option>
              )
            ))}
          </select>
          <button className='view-history-button-small'
            onClick={handleOk}
          >Change slot</button>
          <button className='view-history-button-small'
            onClick={handleCancel}
          >Return</button>
        </span>
      </Modal>
      <Modal title='Confirm cancel'
        open={openCancel}
        footer={null}
        centered={true}
        closable={false}
      >
        <span>
          <p>Are you sure you want to cancel this slot?</p>
          <p id='warning'>THIS CANNOT BE UNDONE!</p>
          <button className='view-history-button-small'
            onClick={cancelSlot}
          >Cancel booking</button>
          <button className='view-history-button-small'
            onClick={handleCancel}
          >Return</button>
        </span>
      </Modal>
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
                              <th>EDIT/CANCEL</th>
                            </tr>
                          </thead>
                          <tbody>
                            {renderTableRows('today')}
                          </tbody>
                        </table>
                      </div>
                    </div>

                    <div>
                      <h3>Upcoming Bookings</h3>
                      <div className="view-history-table-wrapper">
                        {renderNoBookingsMessage('upcoming')}
                        <table className="view-history-upcoming-booking-table">
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
                              <th>EDIT/CANCEL</th>
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
      <CreateFeedbackModal
      visible={feedbackModalVisible}
      onCancel={() => setFeedbackModalVisible(false)}
      bookingId={feedbackData.bookingId}
      branchId={feedbackData.branchId}
      userId={feedbackData.userId}
      />
    </div>
  );
}

