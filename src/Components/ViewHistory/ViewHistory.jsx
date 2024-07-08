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
  const [timeError, setTimeError] = useState(0)
  const token = sessionStorage.getItem('token')
  const [amount, setAmount] = useState()
  const [currentAmount, setCurrentAmount] = useState(0)
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
    changeLog: 0,
  };
  const [formState, setFormState] = useState(initialState)
  const [payment, setPayment] = useState({})
  const numOfChanges = 2
  const navigate = useNavigate();
  const todayLabel = 'today'
  const upcomingLabel = 'upcoming'
  const pastLabel = 'past'
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
        const bookingIds = bookingsData.map(b => b.bookingId)
        const slotsOfUser = slotsData.filter(s => bookingIds.includes(s.bookingId))
        setSlots(slotsOfUser);
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
  }, []);
  const calculateAmount = (courtId, start, end) => {
    var c = courts.find(c => c['courtId'] === courtId)
    return c['price'] * (end - start)
  }
  const handleCalculateAmount = () => {
    var courtId = document.getElementById('court').value
    var start = document.getElementById('startingTime').value
    var end = document.getElementById('endingTime').value
    setAmount(calculateAmount(courtId, start, end))
  }

  // const renderTableRows = (filterType) => {
  //   const filteredBookings = bookings.filter((booking) => {
  //     const slot = slots.find(slot => slot.bookingId === booking.bookingId);
  //     const slotDate = slot ? new Date(slot.date) : null;

  //     switch (filterType) {
  //       case 'today':
  //         return slotDate && slotDate.toDateString() === currentDateString;
  //       case 'upcoming':
  //         return slotDate && slotDate > currentDate;
  //       case 'past':
  //         return slotDate && slotDate < currentDate;
  //       default:
  //         return false;
  //     }
  //   });

  //   const sortedBookings = filteredBookings.sort((a, b) => {
  //     return new Date(b.bookingDate) - new Date(a.bookingDate);
  //   });

  //   return sortedBookings.flatMap((booking) => {
  //     const relatedSlots = slots.filter(slot => slot.bookingId === booking.bookingId);

  //     return relatedSlots.map((slot) => {
  //       const slotDate = new Date(slot.date).toLocaleDateString();
  //       const startTime = formatTime(slot.start);
  //       const endTime = formatTime(slot.end);
  //       const courtId = slot.courtId;
  //       const court = courts.find(court => court.courtId === courtId);
  //       const courtName = court ? court.courtName : 'Unknown Court';
  //       const branch = branches.find(branch => branch.branchId === (court ? court.branchId : null));
  //       const branchName = branch ? branch.branchName : 'Unknown Branch';

  //       return (
  //         !booking.isDeleted &&
  //         <tr key={`${booking.bookingId}-${slot.bookedSlotId}`}>
  //           <td>{booking.bookingId}</td>
  //           <td>{formatNumber(booking.amount)}</td>
  //           <td>{getBookingTypeLabel(booking.bookingType)}</td>
  //           <td>{new Date(booking.bookingDate).toLocaleDateString()}</td>
  //           <td>{slotDate}</td>
  //           <td>{startTime}</td>
  //           <td>{endTime}</td>
  //           <td>{courtName}</td>
  //           <td>{branchName}</td>
  //           {filterType === 'past' ? (
  //             <td>
  //               <button className="vh-feedback-btn" onClick={() => handleFeedbackClick(booking.bookingId, branch.branchId, booking.userId)}>Feedback</button>
  //             </td>
  //           ) : (
  //             <td>
  //               <button className={'view-history-button' + (booking['changeLog'] >= numOfChanges ? ' btn-disabled' : '')} onClick={() => onClickEdit(slot)}
  //                 disabled={booking['changeLog'] >= numOfChanges}
  //               >Edit</button>
  //               <button
  //                 className={'view-history-button view-history-cancel-btn' + (booking['changeLog'] >= numOfChanges ? ' btn-disabled' : '')}
  //                 onClick={() => onClickCancelSlot(slot)}
  //                 disabled={booking['changeLog'] >= numOfChanges}
  //               >Cancel</button>
  //             </td>
  //           )}
  //         </tr>
  //       );
  //     });
  //   });
  // };

  const onClickEdit = (slot) => {
    var t = calculateAmount(slot.courtId, slot.start, slot.end)
    setCurrentAmount(t)
    setAmount(t)
    setOpen(true)
    loadTimeFrame()
    let court = courts.find(c => c.courtId === slot.courtId)
    let branchId = court.branchId
    let booking = bookings.find(b => b.bookingId === slot.bookingId)

    setFormState({
      date: slot.date,
      start: slot.start,
      end: slot.end,
      courtId: slot.courtId,
      bookingId: slot.bookingId,
      branchId: branchId,
      slotId: slot.bookedSlotId,
      changeLog: booking.changeLog
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
      handleResult(res)
    }
    const handleResult = async (res) => {
      if (res.ok) {
        var data = await res.json()
        if (Object.is(data['url'], undefined)) {
          toast.success('Saved changes!')
          setTimeout(() => {
            window.location.reload()
          }, 500);
        }
        else {
          toast.success('Since your balance isn\'t enough, you\'ll be redirected to the payment page')
          setTimeout(() => {
            window.location.assign(data['url'])
          }, 1000);
        }
      }
      if (res.status === HttpStatusCode.BadRequest) {
        var data = await res.json()
        toast.error(data['msg'])
      }
    }
    let t = validateTime(formState.date, formState.start, formState.end)
    try {
      if (!t) {
        toast.error('Invalid time')
        return;
      }
      setOpen(false)
      await getPayment(formState.bookingId)
      await update(formState.start, formState.end, formState.date, getUserId(token),
        formState.courtId, formState.slotId, payment.method, formState.bookingId
      )
      setFormState(initialState)
    }
    catch (err) {
      console.log(err);
      toast.error('Server error')
    }
  }
  
  const createOnEditActionComment = (amount, current) => {
    let delta = amount - current
    var comment = ''
    if (validateTime(formState.date, formState.start, formState.end)) {
      if (delta > 0) comment = `You\'ll need to pay ${formatNumber(delta)}đ to change this booking`
      if (delta < 0) comment = `You\'ll gain ${formatNumber(Math.abs(delta))}đ as time balance when this change is made`
    }
    else {
      comment = 'Invalid time'
    }
    return comment
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
      if (res.status === HttpStatusCode.BadRequest) {
        var data = await res.json()
        toast.error(data['msg'])
        return;
      }
      if(res.status === HttpStatusCode.Unauthorized){
        toast.error('Unauthorized')
        return
      }
      let price = bookings.find(b => b.bookingId === formState.bookingId).amount
      toast.success(`Booking cancelled. \n${formatNumber(price)}đ has been transferred into your balance`)
      setTimeout(() => {
        window.location.reload()
      }, 500);
    }
    catch (err) {
      toast.error(err)
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
  const filterSlots = (type) => {
    const compareDate = (dateStr1, dateStr2) => {
      var date1 = new Date(dateStr1).setHours(0)
      var date2 = new Date(dateStr2).setHours(0)
      return date1 - date2
    }
    return slots.filter(s => {
      switch (type) {
        case todayLabel:
          return compareDate(s.date, currentDateString) === 0
        case upcomingLabel:
          return compareDate(s.date, currentDateString) > 0
        case pastLabel:
          return compareDate(s.date, currentDateString) < 0
        default: return false
      }
    })
  }
  const renderSlotsType = (type) => {
    var collection = filterSlots(type)
    return collection.map((slot) => {
      const booking = bookings.find(b => b.bookingId === slot.bookingId)
      const slotDate = new Date(slot.date).toLocaleDateString();
      const startTime = formatTime(slot.start);
      const endTime = formatTime(slot.end);
      const courtId = slot.courtId;
      const court = courts.find(court => court.courtId === courtId);
      const courtName = court ? court.courtName : 'Unknown Court';
      const branch = branches.find(branch => branch.branchId === (court ? court.branchId : null));
      const branchName = branch ? branch.branchName : 'Unknown Branch';

      return (
        !booking.isDeleted &&
        <tr key={`${booking.bookingId}-${slot.bookedSlotId}`}>
          <td>{slot.bookingId}</td>
          <td>{formatNumber(booking.amount)}</td>
          <td>{getBookingTypeLabel(booking.bookingType)}</td>
          <td>{new Date(booking.bookingDate).toLocaleDateString()}</td>
          <td>{slotDate}</td>
          <td>{startTime}</td>
          <td>{endTime}</td>
          <td>{courtName}</td>
          <td>{branchName}</td>
          {type === pastLabel ? (
            <td>
              <button className="vh-feedback-btn" onClick={() => handleFeedbackClick(booking.bookingId, branch.branchId, booking.userId)}>Feedback</button>
            </td>
          ) : (
            <td>
              <button className={'view-history-button' + (booking['changeLog'] >= numOfChanges ? ' btn-disabled' : '')} onClick={() => onClickEdit(slot)}
                disabled={booking['changeLog'] >= numOfChanges}
              >Edit</button>
              <button
                className={'view-history-button view-history-cancel-btn' + (booking['changeLog'] >= numOfChanges ? ' btn-disabled' : '')}
                onClick={() => onClickCancelSlot(slot)}
                disabled={booking['changeLog'] >= numOfChanges}
              >Cancel</button>
            </td>
          )}
        </tr>
      )
    }
    )
  }
  const renderNoBookingsMessage = (filterType) => {
    if (filterSlots(filterType).length === 0) {
      switch (filterType) {
        case todayLabel:
          return <p>No booking today, <Link to="../findCourt" className="view-history-book-now">Book now</Link> !!</p>;
        case upcomingLabel:
          return <p>No upcoming booking, <Link to="../findCourt" className="view-history-book-now">Book now</Link> !!</p>;
        default:
          return null;
      }
    }

    return null;
  };
  const validateTime = (date, start, end) => {
    let now = new Date()
    //Convert a date string to number automatically shift 7 hours
    date = new Date(Date.parse(date) - 7 * 3600000)
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
          <h4>Edit for booking: {formState.bookingId}, slot: {formState.slotId}</h4>
          <p className='warning'>You can only change a booking up to {numOfChanges} times ({numOfChanges - formState.changeLog} changes left).</p>
          <p>Date:</p>
          <input type="date" id="datePicker" value={formState.date}
            onChange={() => {
              setFormState({
                ...formState,
                date: document.getElementById('datePicker').value
              })
              handleCalculateAmount()
            }}
          />
          <p>Starting time:</p>
          <select id='startingTime'
            onChange={() => {
              setFormState({
                ...formState,
                start: parseInt(document.getElementById('startingTime').value)
              })
              handleCalculateAmount()
            }}
          >
            {timeBound.map(t => (
              <option value={t} selected={t === formState.start}>{t}:00:00</option>
            ))}
          </select>
          <p>Ending time:</p>
          <select id='endingTime'
            onChange={() => {
              setFormState({
                ...formState,
                end: parseInt(document.getElementById('endingTime').value)
              })
              handleCalculateAmount()
            }}
          >
            {timeBound.map(t => (
              <option value={t} selected={t === formState.end}>{t}:00:00</option>
            ))}
          </select>
          <p>Branch:</p>
          <select id='branch'
            onChange={() => {
              setFormState({
                ...formState,
                branchId: document.getElementById('branch').value
              })
            }}
          >
            {branches.map((b, i) => (
              b.branchStatus === 1 &&
              <option value={b.branchId} selected={b.branchId === formState.branchId}>{b.branchName}</option>
            ))}
          </select>
          <p>Court:</p>
          <select id='court' onChange={() => {
            setFormState({
              ...formState,
              courtId: document.getElementById('court').value
            })
            handleCalculateAmount()
          }}
          >
            {courts.map(c => (
              c.courtStatus && c.branchId === formState.branchId
              &&
              (
                <option value={c.courtId} selected={c.courtId === formState.courtId}>{c.courtName}</option>
              )
            ))}
          </select>
          <h4>{createOnEditActionComment(amount, currentAmount)}</h4>
          <div className='right-align-btn'>
            <button className='view-history-button-small'
              onClick={handleOk}
            >Change slot</button>
            <button className='view-history-button-small view-history-cancel-btn'
              onClick={handleCancel}
            >Cancel</button>
          </div>
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
          <p className='warning'>THIS CANNOT BE UNDONE!</p>
          <div className='right-align-btn'>
            <button className='view-history-button-small view-history-cancel-btn'
              onClick={cancelSlot}
            >Cancel booking</button>
            <button className='view-history-button-small'
              onClick={handleCancel}
            >Return</button>
          </div>
        </span>
      </Modal>
      <div className='view-history-header'>
        <Header />
      </div>
      <div className='view-history-wrapper'>
        <div className='view-history-background'>
          <div className="view-history-profile-container">
            <div className="view-history-profile-content">
              <h2>Booking Details</h2>
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
                        {renderNoBookingsMessage(todayLabel)}
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
                            {renderSlotsType(todayLabel)}
                          </tbody>
                        </table>
                      </div>
                    </div>

                    <div>
                      <h3>Upcoming Bookings</h3>
                      <div className="view-history-table-wrapper">
                        {renderNoBookingsMessage(upcomingLabel)}
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
                            {renderSlotsType(upcomingLabel)}
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
                            {renderSlotsType(pastLabel)}
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
        centered={true}
      />
    </div>
  );
}
