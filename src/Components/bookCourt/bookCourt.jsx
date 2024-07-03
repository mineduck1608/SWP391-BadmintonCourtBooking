import React, { useEffect, useState } from "react";
import './bookCourt.css';
import momoLogo from '../../Assets/MoMo_Logo.png'
import vnpayLogo from '../../Assets/vnpay.png'
import { jwtDecode } from 'jwt-decode';
import { HttpStatusCode } from "axios";
import { getHours } from 'date-fns';

const BookCourt = () => {
    const [bookingType, setBookingType] = useState('fixed') //once, fixed, flexible
    const [branches, setBranches] = useState([]) //all active branches (status = 1)
    const [courts, setCourts] = useState([]) //all courts of that branch, if active
    const [paymentType, setPaymentType] = useState('') //banking or not
    const [timeBound, setTimeBound] = useState([]) //0:00 to 23:00
    const [curDate, setCurDate] = useState('') //current date (time this page is interacted)
    const [timeError, setTimeError] = useState(0) //
    const [transferMethod, setTransferMethod] = useState('') //momo, vnpay
    const [courtInfo, setCourtInfo] = useState({})
    const [isOccupied, setIsOccupied] = useState(true)
    const [amount, setAmount] = useState(0)
    const apiUrl = "https://localhost:7233"
    const [queryCourt, setQueryCourt] = useState('')
    const fetchBranches = async () => {
        try {
            setBranches(b => [])
            const branchData = await (
                await fetch(`${apiUrl}/Branch/GetAll`)
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
                await fetch(`${apiUrl}/Court/GetByBranch?id=${branchId}`)
            ).json()
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
            console.log(err);
        }
    }
    useEffect(() => {
        fetchBranches()
        loadTimeFrame()
        setCurDate(new Date())
        //Query court id
        var queryStr = window.location.search
        var params = new URLSearchParams(queryStr)
        if (params.has('courtId')) {
            setQueryCourt(params.get('courtId'))
        }
    }, [])
    const checkAvailableSlot = async () => {
        let courtId = courtInfo['id']
        // console.log("Check for: " + courtId);
        try {
            if (validateDateTime() === 0) {
                var bookingDate = document.getElementById("datePicker").value
                var t1 = parseInt(document.getElementById("time-start").value)
                var t2 = parseInt(document.getElementById("time-end").value)

                let startTime = bookingDate + "T" + (t1 >= 10 ? t1 : ("0" + t1)) + ":00:00"
                let endTime = bookingDate + "T" + (t2 >= 10 ? t2 : ("0" + t2)) + ":00:00"

                const res = await fetch(`${apiUrl}/Slot/GetSlotCourtInInterval?`
                    + `startTime=${startTime}&`
                    + `endTime=${endTime}&`
                    + `id=${courtId}`, {
                    method: 'post',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
                const data = await res.json()
                // console.log('Length: ' + data.length);
                setIsOccupied(data.length > 0)
            }
        } catch (err) {
            console.log(err);
        }
    }
    useEffect(() => {
        try {
            checkAvailableSlot()
        }
        catch (err) {

        }
    }, [courtInfo])

    useEffect(() => {
        handleCalcAmount()
    }, [courtInfo, bookingType])

    const loadCourtInfo = async () => {
        try {
            const selectedCourt = document.getElementById("court").value;
            const response = await fetch(`${apiUrl}/Court/GetById?id=${selectedCourt}`);
            const data = await response.json();
            setCourtInfo({ id: data['courtId'], price: data['price'], status: data['courtStatus'] })
        } catch (error) {

        }
    }
    const validateDateTime = () => {
        // console.log('Validate time');
        var selectedDate = document.getElementById("datePicker").value.replace(/-/g, "/")
        var t1 = parseInt(document.getElementById("time-start").value)
        var t2 = parseInt(document.getElementById("time-end").value)
        var startTime = Date.parse(selectedDate) + t1 * 3600000
        var endTime = Date.parse(selectedDate) + t2 * 3600000
        var curDateNum = Date.parse(curDate)
        var r = 0
        if (startTime > endTime) r = 1
        if (startTime < curDateNum) r = 2
        setTimeError(t => r)
        // console.log(r);
        return r
    }
    const validateBooking = () => {
        //reset
        // console.log('Validate booking');
        var t = (validateDateTime() === 0)
        // console.log("DateTime: " + t);
        try {
            return t && courtInfo['status'] && !isOccupied;
        } catch (error) {
            return false;
        }
    };

    const completeBooking = async () => {
        try {
            const result = validateBooking();
            // console.log(result);
            if (result) {
                document.cookie = `token=${sessionStorage.getItem('token')}; path=/paySuccess`
                fetchApi()
            }
        } catch (error) {
            console.log(error);
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

            }
        }
        return ''
    }
    function handleCalcAmount() {
        try {
            let startTime = document.getElementById('time-start').value
            let endTime = document.getElementById('time-end').value
            let t = document.getElementById('monthNum');
            let monthNum = t == null ? 1 : t.value
            return calcAmount(bookingType, courtInfo['price'], startTime, endTime, monthNum)
        }
        catch (err) {
            return -1
        }
    }
    function calcAmount(bkType, price, start, end, month) {
        var amount = price * (end - start)
        if (bkType === 'fixed') amount *= (4 * month)
        setAmount(a => Object.is(amount, NaN) ? 0 : amount)
        return amount
    }
    const fetchApi = async () => {

        try {
            let startTime = document.getElementById('time-start').value
            let endTime = document.getElementById('time-end').value
            let t = document.getElementById('monthNum');
            let monthNum = t == null ? null : t.value
            let date = document.getElementById('datePicker').value
            const urlTransfer = `${apiUrl}/Booking/TransactionProcess?`
                + `Method=${transferMethod}&`
                + `Start=${startTime}&`
                + `End=${endTime}&`
                + `UserId=${getFromJwt()}&`
                + `Date=${date}&`
                + `CourtId=${courtInfo['id']}&`
                + `Type=${bookingType}&`
                + `NumMonth=${monthNum}&`
                + `Amount=${handleCalcAmount()}`
            const urlFlexible = `${apiUrl}/Slot/BookingByBalance?`
                + `date=${date}&`
                + `start=${startTime}&`
                + `end=${endTime}&`
                + `courtId=${courtInfo['id']}&`
                + `userId=${getFromJwt()}`
            try {
                var res = await fetch(paymentType === 'flexible' ? urlFlexible : urlTransfer,
                    {
                        method: 'post',
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    })
                if (paymentType !== 'flexible') {
                    var data = await (res.json())
                    var payUrl = data['url']
                    if (payUrl !== undefined) {
                        window.location.assign(payUrl)
                    }
                }
                else {
                    window.location.assign(res.status === HttpStatusCode.Ok ? '/paySuccess?msg=success' : '/payFail')
                }
            }
            catch (err) {

            }
        }
        catch (err) {
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
        var rs = ''
        do {
            rs = formatTo3Digits(n % 1000, Math.floor(n / 1000) === 0) + rs
            n = Math.floor(n / 1000)
            if (n > 0) rs = '.' + rs
        }
        while (n > 0)
        return rs
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
                            checkAvailableSlot()
                            handleCalcAmount()
                        }}>
                            {<option value="No" hidden selected>Choose a court</option>}
                            {
                                courts.map((c, i) => (
                                    <option value={c["courtId"]} selected={c['courtId' === queryCourt]}>{c["courtName"]}</option>
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
                                value="fixed-time" onChange={() => {
                                    setBookingType('fixed')
                                    setPaymentType('banking')
                                    handleCalcAmount()
                                }}
                                checked={bookingType === 'fixed'}
                            />
                            <label htmlFor="fixed-time">Fixed Time (reserves at the specified time for the entire months)</label>
                            {bookingType === 'fixed' && (
                                <div className="bookCourt-form-subgroup">
                                    <label htmlFor="fixed-time-months">For:</label>
                                    <select id='monthNum' name="fixed-time-months" onChange={() => handleCalcAmount()}>
                                        <option value={1}>1 month</option>
                                        <option value={2}>2 months</option>
                                        <option value={3}>3 months</option>
                                    </select>
                                    <span>month(s)</span>
                                </div>
                            )}
                        </div>
                        <div className="bookCourt-form-group2">
                            <input className="inputradio" type="radio" id="once" name="booking-type" value="once" onChange={() => {
                                setBookingType('playonce')
                                handleCalcAmount()
                            }}
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
                        <select id="time-start" name="time-start" onChange={() => {
                            if (validateDateTime() === 0) {
                                checkAvailableSlot()
                                handleCalcAmount()
                            }
                        }}>
                            <option value="" hidden>Select Time</option>
                            {
                                timeBound.map(t => (
                                    <option value={t}>{t}:00</option>
                                ))
                            }
                        </select>
                        <span className="text">to</span>
                        <select id="time-end" name="time-end" onChange={() => {
                            if (validateDateTime() === 0) {
                                checkAvailableSlot()
                                handleCalcAmount()
                            }
                        }}>
                            <option value="" hidden>Select Time</option>
                            {
                                timeBound.map(t => (
                                    <option value={t}>{t}:00</option>
                                ))
                            }
                        </select>
                    </div>
                    {
                        (timeError === 1) && (
                            <p id="dateError">Invalid time range</p>
                        )
                    }
                    <div className="bookCourt-form-group5">
                        <label htmlFor="day">Day: </label>
                        <input type="date" id="datePicker" onChange={() => {
                            if (validateDateTime() === 0) {
                                checkAvailableSlot()
                                handleCalcAmount()
                            }
                        }} />

                    </div>
                    {
                        (timeError === 2) && (
                            <p id="dateError">Cannot create a booking for a date in the past</p>
                        )
                    }

                    <div className="bookcourt-status">
                        <h2 className="notes">
                        4. STATUS: <span className={isOccupied ? "occupied" : "free"}>{isOccupied ? "Occupied" : "Free"}</span></h2>
                        <span>Price: <span className='priceSpan'>{formatNumber(amount)}</span></span>
                        <label htmlFor="paymentType">Payment type</label>
                        <div className='inlineDiv'>
                            <input type='radio' className="inputradioRight2" name='paymentType' value='banking'
                                onChange={() => { setPaymentType(p => 'banking') }}
                                checked={paymentType === 'banking'}
                            />
                            <span>Banking</span>
                        </div>
                        {
                            bookingType === 'playonce' &&
                            (
                                <div className='inlineDiv'>
                                    <input type='radio' className="inputradioRight2" name='paymentType' value='timeBalance'
                                        onChange={() => {
                                            setPaymentType(p => 'flexible')
                                        }}
                                        checked={paymentType === 'flexible'}
                                    />
                                    <span>Time balance</span>
                                </div>
                            )
                        }
                        {
                            //Banking
                            paymentType === 'banking' &&
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