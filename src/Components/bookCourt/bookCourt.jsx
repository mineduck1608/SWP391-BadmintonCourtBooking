import React, { useEffect, useState } from "react";
import './bookCourt.css';
import momoLogo from '../../Assets/MoMo_Logo.png'
import vnpayLogo from '../../Assets/vnpay.png'
import { jwtDecode } from 'jwt-decode';

const BookCourt = () => {
    const [bookingType, setBookingType] = useState('fixed'); //once, fixed, flexible
    const [branches, setBranches] = useState([]) //all active branches (status = 1)
    const [courts, setCourts] = useState([]) //all courts of that branch, if active
    const [paymentType, setPaymentType] = useState(''); //banking or not
    const [timeBound, setTimeBound] = useState([]) //0:00 to 23:00
    const [curDate, setCurDate] = useState('') //current date (time this page is interacted)
    const [validDate, setValidDate] = useState(false) //is booking date valid? (not in the past)
    const [validTimeRange, setValidTimeRange] = useState(false) //is booking time range valid (start < end)
    const [transferMethod, setTransferMethod] = useState('Momo') //momo, vnpay
    const [validBooking, setValidBooking] = useState(false) //is booking valid
    const [courtInfo, setCourtInfo] = useState({})
    const [isOccupied, setIsOccupied] = useState(true)
    const apiUrl = "https://localhost:7233/"

    const fetchBranches = async () => {
        try {
            await setBranches(b => [])
            const branchData = await (
                await fetch(`${apiUrl}Branch/GetAll`)
            ).json()
            for (let index = 0; index < branchData.length; index++) {
                if ((branchData[index])["branchStatus"] === 1) {
                    setBranches(b => [...b, branchData[index]])
                }
            }
        }
        catch (err) {
            //
        }
    }
    const fetchCourts = async () => {
        try {
            var branchId = document.getElementById("branch").value
            setCourts([])
            const courtData = await (
                await fetch(`${apiUrl}Court/GetByBranch?id=${branchId}`)
            ).json()
            console.log('done fetching courts');
            for (let index = 0; index < courtData.length; index++) {
                if ((courtData[index])["courtStatus"] === true) {
                    setCourts(c => [...c, courtData[index]])
                }
            }
        }
        catch (err) {
            //Toast: ko fetch dc branch
        }
    }
    useEffect(() => {
        fetchBranches()
        for (let i = 0; i <= 23; i++) {
            setTimeBound(t => [...t, i])
        }
        setCurDate(new Date())
    }, [])
    useEffect(() => {
        async function checkAvailableSlot() {
            let courtId = courtInfo['id']
            if (validateDate() && validateTime()) {
                var bookingDate = document.getElementById("datePicker").value.replace(/-/g, "/")
                var t1 = parseInt(document.getElementById("time-start").value)
                var t2 = parseInt(document.getElementById("time-end").value)
                const res = await fetch(`${apiUrl}Slot/GetSLotCourtInDay?
                    date=${bookingDate}&
                    id=${courtId}`)
                const data = await res.json()
            }
        }
        try {

        }
        catch (err) {

        }
    }, [courtInfo])
    const loadCourtInfo = async () => {
        try {
            const selectedCourt = document.getElementById("court").value;
            const response = await fetch(`${apiUrl}Court/GetById?id=${selectedCourt}`);
            const data = await response.json();
            setCourtInfo({ id: data['courtId'], price: data['price'], status: data['courtStatus'] })
        } catch (error) {
            console.error('Error fetching court data:', error);
        }
    }

    const validateDate = () => {
        var value = document.getElementById("datePicker").value.replace(/-/g, "/")
        var selectedDate = Date.parse(value)
        if (!Object.is(selectedDate, NaN)) {
            setValidDate(!(selectedDate < curDate))
            return !(selectedDate < curDate)
        }
        else {
            setValidDate(false)
            return false;
        }
    }
    const validateTime = () => {
        var t1 = parseInt(document.getElementById("time-start").value)
        var t2 = parseInt(document.getElementById("time-end").value)
        setValidTimeRange(t1 < t2)
        return t1 < t2
    }
    const validateBooking = async () => {
        setValidBooking(t => true);
        validateDate();
        validateTime();

        try {
            setValidBooking(t => t && courtInfo['status']);
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
                document.cookie = `token=${sessionStorage.getItem('token')}; path=/paySuccess`
                fetchApi()
            }
        } catch (error) {
            console.error('Error validating booking:', error);
        }
    };
    const getFromJwt = () => {

        var token = sessionStorage.getItem('token')
        if (!token) {
        } else {
            try {
                var decodedToken = jwtDecode(token)
                return decodedToken['UserId']
            }
            catch (err) {
                console.log(err)
            }
        }
        return ''
    }
    function calcAmount(bkType, price, start, end, month) {
        var amount = price * (end - start)
        if (bkType === 'fixed') amount *= 4 * month
        return amount
    }
    const fetchApi = async () => {
        console.log('fetchBegin');
        try {
            //Get slots in day
            console.log(bookingType + ", " + paymentType + ", " + transferMethod);
            let t = document.getElementById('monthNum');
            let monthNum = t == null ? -1 : t.value
            let startTime = document.getElementById('time-start').value
            let endTime = document.getElementById('time-end').value
            try {
                var res = await fetch(`${apiUrl}Booking/TransactionProcess?`
                    + `Method=${transferMethod}&`
                    + `Start=${startTime}&`
                    + `End=${endTime}&`
                    + `UserId=${getFromJwt()}&`
                    + `Date=${document.getElementById('datePicker').value}&`
                    + `CourtId=${courtInfo['id']}&`
                    + `Type=${bookingType}&`
                    + `NumMonth=${monthNum}&`
                    + `Amount=${calcAmount(bookingType, courtInfo['price'], startTime, endTime)}`,
                    {
                        method: 'post',
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    })
                var data = await (res.json())
                window.location.assign(data['url'])
            }
            catch (err) {

            }
        }
        catch (err) {
            console.log(err)
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
                            loadCourtInfo()
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
                    {/*
                    FIXED
                    */}
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
                                    <select id='monthNum' name="fixed-time-months">
                                        <option value={1}>1 month</option>
                                        <option value={2}>2 months</option>
                                        <option value={3}>3 months</option>
                                    </select>
                                    <span>month(s)</span>
                                </div>
                            )}
                        </div>
                        <div className="bookCourt-form-group2">
                            <input className="inputradio" type="radio" id="once" name="booking-type" value="once" onChange={() => { setBookingType('playonce') }}
                                checked={bookingType === 'playonce'}
                            />
                            <label htmlFor="once">Once (reserves at the specified time and date)</label>
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

                    <div className="bookcourt-status">
                        <h2 className="notes">4. STATUS: </h2>
                        <label htmlFor="paymentType">Payment type</label>
                        <input type='radio' className="inputradioRight2" name='paymentType' value='banking'
                            onChange={() => { setPaymentType(p => 'banking') }}
                            checked={paymentType === 'banking'}
                        />
                        <span className="bookCourt_span">Banking</span>
                        <input type='radio' className="inputradioRight2" name='paymentType' value='timeBalance'
                            onChange={() => {
                                setBookingType(p => 'flexible');
                                setPaymentType(p => '')
                            }}
                            checked={bookingType === 'flexible'}
                        />
                        <span>Time balance</span>
                        {
                            //Banking
                            paymentType === 'banking' && bookingType !== 'flexible' &&
                            <div id="bankingMethod">
                                <input
                                    type='radio'
                                    className="inputradioRight"
                                    name='transferMethod'
                                    value='vnpay'
                                    id="vnpayRadio"
                                    onChange={() => setTransferMethod(1)}
                                    checked={transferMethod === 1}
                                />
                                <label
                                    htmlFor="vnpayRadio"
                                    className={`bookCourt_span ${transferMethod === 1 ? 'selected' : ''}`}
                                >
                                    <img id='VnpayLogo' src={vnpayLogo} alt="Vnpay" />
                                </label>

                                <input
                                    type='radio'
                                    className="inputradioRight"
                                    name='transferMethod'
                                    value='momo'
                                    id="momoRadio"
                                    onChange={() => setTransferMethod(2)}
                                    checked={transferMethod === 2}
                                />
                                <label
                                    htmlFor="momoRadio"
                                    className={`bookCourt_span ${transferMethod === 2 ? 'selected' : ''}`}
                                >
                                    <img id='MomoLogo' src={momoLogo} alt="Momo" />
                                </label>
                            </div>
                        }

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