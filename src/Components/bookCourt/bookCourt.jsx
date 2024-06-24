import React, { useEffect, useState } from "react";
import './bookCourt.css';
import momoLogo from '../../Assets/MoMo_Logo.png'
import vnpayLogo from '../../Assets/vnpay.png'
import { jwtDecode } from 'jwt-decode';

const BookCourt = () => {
    const [bookingType, setBookingType] = useState('fixed-time');
    const [branches, setBranches] = useState([])
    const [courts, setCourts] = useState([])
    const [paymentType, setPaymentType] = useState('balance');
    const [timeBound, setTimeBound] = useState([])
    const [curDate, setCurDate] = useState('')
    const [validDate, setValidDate] = useState(false)
    const [validTimeRange, setValidTimeRange] = useState(false)
    const [transferMethod, setTransferMethod] = useState('Momo')
    const [validBooking, setValidBooking] = useState(false)
    const [userID, setUserID] = useState('');
    const apiUrl = "http://localhost:5266/"

    const fetchBranches = async () => {
        const branchData = await (
            await fetch(`${apiUrl}Branch/GetAll`)
        ).json()
        for (let index = 0; index < branchData.length; index++) {
            if ((branchData[index])["branchStatus"] === 1) {
                setBranches(b => [...b, branchData[index]])
            }
        }
    }
    const fetchCourts = async () => {
        var branchId = document.getElementById("branch").value
        setCourts([])
        const courtData = await (
            await fetch(`${apiUrl}Court/GetByBranch?id=${branchId}`)
        ).json()
        for (let index = 0; index < courtData.length; index++) {
            if ((courtData[index])["courtStatus"] === true) {
                setCourts(c => [...c, courtData[index]])
            }
        }
    }
    useEffect(() => {
        try {
            fetchBranches()
        }
        catch (err) {
            console.log(err)
        }
        for (let i = 0; i <= 23; i++) {
            setTimeBound(t => [...t, i])
        }
        setCurDate(new Date())
    }, [])
    useEffect(() => {
        validateBooking()
    }, [validDate, validTimeRange])


    const validateDate = () => {
        var value = document.getElementById("datePicker").value.replace(/-/g, "/")
        var selectedDate = Date.parse(value)
        if (!Object.is(selectedDate, NaN)) {
            setValidDate(!(selectedDate < curDate))
        }
        else { setValidDate(false) }
    }
    const validateTime = () => {
        var t1 = parseInt(document.getElementById("time-start").value)
        var t2 = parseInt(document.getElementById("time-end").value)
        setValidTimeRange(t1 < t2)
    }
    const validateBooking = async () => {
        console.log("Running");
        setValidBooking(t => t || true);
        validateDate();
        validateTime();

        try {
            const selectedCourt = document.getElementById("court").value;
            const response = await fetch(`${apiUrl}Court/GetById?id=${selectedCourt}`);
            if (!response.ok) {
                throw new Error('Failed to fetch court data');
            }
            const data = await response.json();
            const courtActive = data["courtStatus"];
            setValidBooking(t => t && courtActive);
            return validBooking;

        } catch (error) {
            console.error('Error fetching court data:', error);
            setValidBooking(false);
            return false;
        }
    };

    const completeBooking = async () => {
        console.log("Complete booking");
        try {
            const result = await validateBooking();
            if (result) {
                window.location.assign("https://www.google.com/")
            }
        } catch (error) {
            console.error('Error validating booking:', error);
        }
    };
    const getFromJwt = () => {
        async function fetchData() {
            var token = sessionStorage.getItem('token')
            if (!token) {
                //alert('Please log in')
            } else {
                try {
                    var decodedToken = jwtDecode(token)
                    setUserID(u => decodedToken.UserId)
                    var res = await fetch(`${apiUrl}User/GetById?id=${decodedToken.UserId}`)
                    var data = await res.json()
                    if (decodedToken.UserId !== data['userId']) {
                        throw new Error('Authorize failed')
                    } else {
                        setUserID(data['userId'])
                    }
                }
                catch (err) {
                    console.log(err)
                }
            }
        }
        fetchData()
    }
    const fetchApi = async () => {
        try {
            let method = document.getElementById('transfer').value
            let time_start = document.getElementById('time_start').value
            let time_end = document.getElementById('time_end').value
            getFromJwt()
            let bookedDate = document.getElementById('datePicker').value
            let courtId = document.getElementById('court').value
            let monthNum = document.getElementById('fixed-time-month')
            let amount = 0
            let type = ''
            var res = await fetch(`${apiUrl}Booking/TransactionProcess?`
                + `Method=${method}&`
                + `Start=${time_start}&`
                + `End=${time_end}&`
                + `UserId=${userID}&`
                + `Date=${bookedDate}&`
                + `CourtId=${courtId}&`
                + `Type=${type}&`
                + `NumMonth=${monthNum}&`
                + `Amount=${amount}`,
                {
                    method: 'post',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
            try {
                var data = await (res.json())
                window.location.assign(data['url'])
            }
            catch (err) {

            }
        }
        catch (err) {
            alert(err)
        }
    }

    return (
        <div className="bookCourt-container">
            <h1 className="bookCourt-title">BOOKING A COURT</h1>
            <div className="bookCourt-body">
                <div className="bookCourt-section bookCourt-left-section">
                    <h2 className="notes">1. SELECT A COURT</h2>
                    <div className="bookCourt-option1">
                        <label htmlFor="branch">BRANCH:</label>

                        <select id="branch" name="branch" onChange={() => {
                            fetchCourts()
                            validateBooking()
                        }}>
                            <option value="" hidden selected>Choose a branch</option>
                            {
                                branches.map(b =>
                                (
                                    <option value={b["branchId"]}>{b["branchName"]}</option>
                                )
                                )
                            }
                        </select>
                    </div>
                    <div className="bookCourt-option2">
                        <label htmlFor="court">COURT:</label>

                        <select id="court" name="court" onChange={() => {
                            validateBooking()
                        }}>
                            {<option value="No" hidden selected>Choose a court</option>}
                            {
                                courts.map((c, i) => (
                                    <option value={c["courtId"]}>{c["courtId"]}</option>
                                ))
                            }
                        </select>
                    </div>
                    <h2 className="notes">2. TYPE OF BOOKING</h2>
                    <p>SELECT ONE TYPE OF BOOKING:</p>
                    <div className="bookCourt-radio-group">
                        <div className="bookCourt-form-group1">

                            <input className="inputradio" type="radio" id="fixed-time" name="booking-type"
                                value="fixed-time" onChange={() => setBookingType('fixed')}
                                checked={bookingType === 'fixed'}
                            />
                            <label htmlFor="fixed-time">Fixed Time (reserves at the specified time for the entire months)</label>
                            {bookingType === 'fixed' && (
                                <div className="bookCourt-form-subgroup">
                                    <label htmlFor="fixed-time-months">For:</label>
                                    <select id="fixed-time-months" name="fixed-time-months">
                                        <option value="1">1 month</option>
                                        <option value="2">2 months</option>
                                        <option value="3">3 months</option>
                                        {/* Add more options as needed */}
                                    </select>
                                    <span>month(s)</span>
                                </div>
                            )}
                        </div>
                        <div className="bookCourt-form-group2">
                            <input className="inputradio" type="radio" id="once" name="booking-type" value="once" onChange={() => { setBookingType('playonce') }} />
                            <label htmlFor="once">Once (reserves at the specified time and date)</label>
                            {
                                bookingType === 'playonce' && (
                                    <div id="nowrap">
                                        <label htmlFor="payMethod">Select a method to pay</label>
                                        <input className="inputradio" type="radio" id="balance" value="balance" onChange={() => setPaymentType('balance')} name="onceMethod"
                                            checked={paymentType === 'balance'}
                                        />
                                        <label htmlFor="balance" id="balanceLabel" >Account time balance</label>
                                        {
                                            paymentType === 'balance' && (
                                                <button className="buyTimeBtn">Buy more time into balance!</button>
                                            )
                                        }
                                        <input className="inputradio" type="radio" id="transfer" value="transfer" name="onceMethod" onChange={() => setPaymentType('transfer')}
                                            checked={paymentType === 'transfer'}
                                        />
                                        <label htmlFor="transfer" id="transferLabel">Transfer by bank</label>
                                        {
                                            paymentType === "transfer" && (
                                                <div id="nowrapper">
                                                    <input className="inputradio tab1" type="radio" id="Momo" value={2}
                                                        onChange={() => setTransferMethod("Momo")} name="transferMethod"
                                                        checked={transferMethod === "Momo"}
                                                    />
                                                    <img htmlFor="Momo" src={momoLogo} alt="Momo" id="MomoLogo" className="tab2"></img>

                                                    <input className="inputradio tab1" type="radio" id="Vnpay" value={1}
                                                        onChange={() => setTransferMethod("Vnpay")} name="transferMethod"
                                                        checked={transferMethod === "Vnpay"}
                                                    />
                                                    <img htmlFor="Vnpay" src={vnpayLogo} alt="Vnpay" id="VnpayLogo" className="tab2"></img>
                                                </div>
                                            )
                                        }
                                    </div>
                                )
                            }
                        </div>
                    </div>
                </div>
                <div className="bookCourt-section bookCourt-right-section">
                    <h2 className="notes">3. TIME AND DATE</h2>
                    <div className="bookCourt-form-group4">
                        <label className="text" htmlFor="time-start">Time:</label>
                        <select id="time-start" name="time-start" onChange={() => validateTime()}>
                            <option value="" hidden>Select Time</option>
                            {
                                timeBound.map(t => (
                                    <option value={t}>{t}:00</option>
                                ))
                            }
                        </select>
                        <span className="text">to</span>
                        <select id="time-end" name="time-end" onChange={() => validateTime()}>
                            <option value="" hidden>Select Time</option>
                            {
                                timeBound.map(t => (
                                    <option value={t}>{t}:00</option>
                                ))
                            }
                        </select>
                    </div>
                    {
                        !validTimeRange && (
                            <p id="dateError">Invalid time range</p>
                        )
                    }
                    <div className="bookCourt-form-group5">
                        <label htmlFor="day">Day: </label>
                        <input type="date" id="datePicker" onChange={() => validateDate()} />

                    </div>
                    {
                        !validDate && (
                            <p id="dateError">Cannot create a booking for a date in the past</p>
                        )
                    }
                    <h2 className="notes">4. NOTES</h2>
                    <textarea id="notes" name="notes" placeholder="Enter your notes here"></textarea>
                    <div className="bookcourt-status">
                        <h2 className="notes">5. STATUS: </h2>
                        <h2 className="notes">ON GOING</h2>
                    </div>
                </div>
            </div>
            <button type="submit" className="bookCourt-complete-booking-button"
                onClick={() => {
                    completeBooking()
                }}
            >
                Complete booking</button>

        </div>
    );
}

export default BookCourt;