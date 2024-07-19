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
import { fetchWithAuth } from '../fetchWithAuth/fetchWithAuth';
import { padStart } from '@fullcalendar/core/internal';

export default function ViewHistory() {
  const numOfChanges = 2
  const todayLabel = 'today'
  const upcomingLabel = 'upcoming'
  const pastLabel = 'past'
  const maxHourCanChange = 1
  const errorFetching = 'An error occured'
  const [bookings, setBookings] = useState([]);
  const [slots, setSlots] = useState([]);
  const [courts, setCourts] = useState([]);
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [open, setOpen] = useState(false);
  const [openCancel, setOpenCancel] = useState(false);
  const [timeBound, setTimeBound] = useState([]) //0:00 to 23:00
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
    bookingDate: '',
  };
  const [user, setUser] = useState({})
  const [formState, setFormState] = useState(initialState)
  const [chooseTypeModal, setChooseTypeModal] = useState(false)

  const currentDate = new Date();
  const currentDateString = currentDate.toDateString();
  const [currentTime, setCurrentTime] = useState(new Date())

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
      if (n > 0) rs = ',' + rs
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

        const bookingsResponse = await fetchWithAuth(`${apiUrl}/Booking/GetByUser?id=${userIdToken}`, {
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
        const slotsResponse = await fetchWithAuth(`${apiUrl}/Slot/GetAll`, {
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
        const courtsResponse = await fetchWithAuth(`${apiUrl}/Court/GetAll`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!courtsResponse.ok) {
          throw new Error('Failed to fetch courts');
        }

        const courtsData = await courtsResponse.json();
        setCourts(courtsData);

        const branchesResponse = await fetchWithAuth(`${apiUrl}/Branch/GetAll`, {
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
    const loadTimeFrame = async () => {
      const fetchTime = async () => {
        var res = await fetchWithAuth(`${apiUrl}/Slot/GetAll`)
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
        console.log(err);
        toast.error(errorFetching)
      }
    }
    const getUserData = async (id) => {
      const res = await fetchWithAuth(`${apiUrl}/User/GetById?id=${id}`)
      const data = await res.json()
      setUser(data)
    }
    try {
      fetchData()
      loadTimeFrame()
      getUserData(jwtDecode(sessionStorage.getItem('token'))['UserId'])
    }
    catch (err) {
      toast(errorFetching)
    }
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

  const onClickEdit = (slot) => {
    var t = calculateAmount(slot.courtId, slot.start, slot.end)
    setCurrentAmount(t)
    setAmount(t)
    setOpen(true)
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
      changeLog: booking.changeLog,
      bookingDate: booking.bookingDate
    })
  }
  const onClickCancelSlot = (slot) => {
    setOpenCancel(true)
    let court = courts.find(c => c.courtId === slot.courtId)
    let branchId = court.branchId
    let cancelAmount = (slot.end - slot.start) * court.price * 0.5
    let booking = bookings.find(b => b.bookingId === slot.bookingId)
    setFormState({
      date: slot.date,
      start: slot.start,
      end: slot.end,
      courtId: slot.courtId,
      bookingId: slot.bookingId,
      branchId: branchId,
      slotId: slot.bookedSlotId,
      cancelAmount: cancelAmount,
      bookingDate: booking.bookingDate
    })
  }
  const formatTime = (time) => {
    const hours = time.toString().padStart(2, '0');
    return `${hours}:00:00`;
  };

  const sendUpdateRequestWrapper = async (paymentMethod) => {
    const sendUpdateRequest = async (start, end, date, userId, courtId, slotId, paymentMethod, bookingId) => {
      var res = await fetchWithAuth(`${apiUrl}/Slot/UpdateByUser?`
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
      return res
    }
    try {
      return await sendUpdateRequest(formState.start, formState.end, formState.date, user.userId,
        formState.courtId, formState.slotId, paymentMethod, formState.bookingId
      )
    }
    catch (err) {

    }
  }
  const saveResult = async (paymentMethod) => {
    const handleResult = async (res) => {
      if (res.ok) {
        toast.success('Saved changes!')
        setTimeout(() => {
          window.location.reload()
        }, 500);
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
      var tmp = await sendUpdateRequestWrapper(paymentMethod)
      handleResult(tmp)
      setFormState(initialState)
    }
    catch (err) {
      console.log(err);
      toast.error(errorFetching)
    }
  }
  const handleUpdate = async (amount, current) => {
    if (user.balance < amount - current) {
      setChooseTypeModal(true)
      setOpen(false)
      return
    }
    //proceed as usual. Use null since it won't use payment method
    try {
      await saveResult(null)
    }
    catch (err) {
      toast.error(errorFetching)
    }
  }
  const createOnEditActionComment = (amount, current) => {
    if (!validateTime(formState.date, formState.start, formState.end)) {
      return 'Invalid time'
    }
    let delta = amount - current
    var comment = ''
    if (delta === 0) {
      return ''
    }
    if (delta < 0) {
      return `${formatNumber(delta * (-1))}đ will be added to your balance`
    }
    if (user.balance >= delta) {
      comment = `${formatNumber(delta)}đ will be subtracted from your balance`
    }
    else {
      //newPrice - (balance + current) >= 10000?
      var tmp = amount - (user.balance + current)
      if (tmp < 10000) {
        //Pay the whole thing
        comment = `You\'ll need to pay ${formatNumber(amount)}đ to change this booking`
      }
      else {
        //Use balance to lessen
        comment = `You\'ll need to pay ${formatNumber(tmp)}đ to change this booking`
      }
    }
    return comment
  }
  const cancelSlot = async () => {
    async function cancel(slotId, bookingId) {
      return res = await fetchWithAuth(`${apiUrl}/Slot/Cancel?`
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
      if (res.status === HttpStatusCode.Unauthorized) {
        toast.error('Unauthorized')
        return
      }
      toast.success(`Booking cancelled`)
      setTimeout(() => {
        window.location.reload()
      }, 500);
    }
    catch (err) {
      console.log(err);
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
  const isEditable = (booking) => {
    var bookingDateNum = Date.parse(new Date(booking.bookingDate))
    var currentTime = Date.parse(new Date())
    return (currentTime - bookingDateNum) <= maxHourCanChange * 3600000 && booking['changeLog'] < numOfChanges
  }
  function convertDateAndHour(date, hour) {
    return new Date(date).setHours(hour)
  }
  const renderSlotsType = (type) => {
    var collection = filterSlots(type)
      .sort((s1, s2) => {
        var s1StartTime = convertDateAndHour(s1.date, s1.start)
        var s2StartTime = convertDateAndHour(s2.date, s2.start)
        return s1StartTime - s2StartTime
      })
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
          <td>{new Date(booking.bookingDate).toLocaleTimeString()}</td>
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
              <button className={'view-history-button' + (!isEditable(booking) ? ' btn-disabled' : '')} onClick={() => onClickEdit(slot)}
                disabled={!isEditable(booking)}
              >Edit</button>
              <button
                className={'view-history-button view-history-cancel-btn' + (!isEditable(booking) ? ' btn-disabled' : '')}
                onClick={() => onClickCancelSlot(slot)}
                disabled={!isEditable(booking)}
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
    let startTime = convertDateAndHour(date, start)
    let endTime = convertDateAndHour(date, end)
    return (startTime < endTime) && (startTime > now)
  }
  //Tick down every 1s
  useEffect(() => {
    setTimeout(() => {
      setCurrentTime(new Date())
    }, 1000);
  })
  const calculateTimeLeft = (time) => {
    var range = currentTime - new Date(time)
    range = maxHourCanChange * 3600000 - range
    if (range <= 0) {
      handleCancel()
    }
    return range
  }
  const formatHMS = (time) => {
    let hour = Math.floor(time / 3600000)
    time = time - hour * 3600000
    let min = Math.floor(time / 60000)
    time = time - min * 60000
    let sec = Math.floor(time / 1000)
    return `${padStart(hour, 2)}h${padStart(min, 2)}m${padStart(sec, 2)}s`
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
          <p className='warning'>
            You can only change a booking up to {numOfChanges} times, and must not be more than {maxHourCanChange} hour(s) since booking time.</p>
          <p>
            Currently <span className='warning'>{numOfChanges - formState.changeLog}</span> changes left,
            and <span className='warning'>{formatHMS(calculateTimeLeft(formState.bookingDate))}</span> left.
          </p>
          <section className='dateSection'>
            <span htmlFor='vhDatePicker'>Date:</span>
            <input type="date" id="vhDatePicker" value={formState.date}
              onChange={() => {
                setFormState({
                  ...formState,
                  date: document.getElementById('vhDatePicker').value
                })
                handleCalculateAmount()
              }}
            />
          </section>
          <section className='timeSection'>
            <span>Time: </span>
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
                <option value={t} selected={t === formState.start}>{t}:00</option>
              ))}
            </select>
            <span> To </span>
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
                <option value={t} selected={t === formState.end}>{t}:00</option>
              ))}
            </select>
          </section>
          <section className='branchSection'>
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
          </section>
          <section className='courtSection'>
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
          </section>
          <h4>{createOnEditActionComment(amount, currentAmount)}</h4>
          <div className='right-align-btn'>
            <button className='view-history-button-small'
              onClick={() => handleUpdate(amount, currentAmount)}
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
          <p>Are you sure you want to cancel this slot? You'll gain
            <span> {formatNumber(formState.cancelAmount)}đ</span>
          </p>
          <p className='warning'>THIS CANNOT BE UNDONE!</p>
          <p>You have <span className='warning'>{formatHMS(calculateTimeLeft(formState.bookingDate))}</span> left</p>
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
      <Modal title='Choose payment type'
        open={chooseTypeModal}
        footer={null}
        centered={true}
        closable={false}
      >
        <span>
          <p>
            Since your balance isn't enough...
          </p>
          <select id='paymentMethod'>
            <option value={1}>VnPay</option>
            <option value={2}>MoMo</option>
          </select>
          <div className='right-align-btn'>

            <button className='view-history-button-small'
              onClick={() => {
                const sendAndAssign = async () => {
                  var t = await sendUpdateRequestWrapper(document.getElementById('paymentMethod').value)
                  var data = await t.json()
                  window.location.assign(data['url'])
                }
                sendAndAssign()
              }}
            >Pay by banking
            </button>
            <button className='view-history-button-small'
              onClick={() => {
                window.location.assign('/buyTime')
              }}
            >Buy Balance
            </button>
            <button className='view-history-button-small view-history-cancel-btn'
              onClick={() => { setChooseTypeModal(false) }}
            >Cancel change
            </button>
          </div>
          <p>Reminder: we'll use your balance to lessen the fee if possible (new price - balance is at least 10.000đ)</p>
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
                    <h3 className='warning'>
                      You can only change or cancel a booking within {numOfChanges} change(s), and within {maxHourCanChange} hour(s) after it has been created
                    </h3>
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
                              <th>BOOKING TIME</th>
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
                              <th>BOOKING TIME</th>
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
                              <th>BOOKING TIME</th>
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
