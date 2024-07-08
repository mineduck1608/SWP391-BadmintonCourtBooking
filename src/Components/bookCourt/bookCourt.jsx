import React, { useEffect, useState } from "react";
import './bookCourt.css';
import momoLogo from '../../Assets/MoMo_Logo.png'
import vnpayLogo from '../../Assets/vnpay.png'
import { jwtDecode } from 'jwt-decode';
import { toast } from "react-toastify";

const BookCourt = () => {
    const [bookingType, setBookingType] = useState('') //once, fixed, flexible
    const [branches, setBranches] = useState([]) //all active branches (status = 1)
    const [courts, setCourts] = useState([]) //all courts if active
    const [paymentType, setPaymentType] = useState('') //banking or not
    const [timeBound, setTimeBound] = useState([]) //0:00 to 23:00
    const [timeError, setTimeError] = useState(0) //
    const [transferMethod, setTransferMethod] = useState(1) //momo, vnpay
    const [courtInfo, setCourtInfo] = useState({})
    const [isOccupied, setIsOccupied] = useState(true)
    const [amount, setAmount] = useState(0)
    const apiUrl = "https://localhost:7233"
    const [selectedBranch, setSelectedBranch] = useState({})
    const [user, setUser] = useState({})
    const fetchBranches = async () => {
        try {
            setBranches(b => [])
            const branchData = await (
                await fetch(`${apiUrl}/Branch/GetAll`, {
                    headers: {
                        Authorization: `Bearer ${sessionStorage.getItem('token')}`
                    }
                })
            ).json()
            for (let index = 0; index < branchData.length; index++) {
                if ((branchData[index])["branchStatus"] === 1) {
                    setBranches(b => [...b, branchData[index]])
                }
            }
        }
        catch (err) {
            toast.error('Server error')
        }
    }
    const fetchCourts = async () => {
        try {
            var queryStr = window.location.search
            var params = new URLSearchParams(queryStr)
            setCourts([])
            const courtData = await (
                await fetch(`${apiUrl}/Court/GetAll`)
            ).json()
            for (let index = 0; index < courtData.length; index++) {
                if ((courtData[index])["courtStatus"] === true) {
                    setCourts(c => [...c, courtData[index]])
                    if (params.has('courtId') && courtData[index].courtId === params.get('courtId')) {
                        setCourtInfo(courtData[index])
                        setSelectedBranch(courtData[index].branchId)
                    }
                }
            }
        }
        catch (err) {
            toast.error('Server error')
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
            toast.error('Server error')
        }
    }
    useEffect(() => {
        async function getUser() {
            var token = sessionStorage.getItem('token')
            if (!token) return;
            try {
                var decodedToken = jwtDecode(token)
                var data = await (await
                    fetch(`${apiUrl}/User/GetById?id=${decodedToken['UserId']}`,
                        {
                            method: 'get',
                            headers: {
                                Authorization: `Bearer ${token}`
                            }
                        }
                    )).json()
                setUser(data)
            }
            catch (err) {
                console.log('error getting user');
                console.log(err);
            }

        }
        async function loadData() {
            await fetchBranches()
            await fetchCourts()
            await loadTimeFrame()
        }
        loadData()
        getUser()
    }, [])
    const checkAvailableSlot = async (courtId) => {
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
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${sessionStorage.getItem('token')}`
                    }
                })
                const data = await res.json()
                setIsOccupied(data.length > 0)
            }
        } catch (err) {
            toast.error('Server error')
        }
    }

    useEffect(() => {
        handleCalcAmount()
    }, [courtInfo, bookingType])

    const loadCourtInfo = async (id) => {
        try {
            const data = courts.find(c => c.courtId === id)
            setCourtInfo(data)
            await checkAvailableSlot(document.getElementById('court').value)
        } catch (error) {
            toast.error('Server error')
        }
    }
    const check = (date, start, end) => {
        date = date + ' 00:00:00'
        let dateNum = Date.parse(date)
        let startTime = dateNum + start * 3600000
        let endTime = dateNum + end * 3600000
        let r = 0
        if (startTime >= endTime) r = 1
        if (startTime <= Date.parse(new Date())) r = 2
        setTimeError(r)
        return r
    }
    const validateDateTime = () => {
        // console.log('Validate time');
        var selectedDate = document.getElementById("datePicker").value.replace(/-/g, "/")
        var t1 = parseInt(document.getElementById("time-start").value)
        var t2 = parseInt(document.getElementById("time-end").value)

        if (selectedDate === '' || Object.is(t1, NaN) || Object.is(t2, NaN)) {
            return 2
        }
        return check(selectedDate, t1, t2)
    }

    const validateBooking = () => {
        var tmp = (validateDateTime() === 0)
        if (!tmp) return false
        try {
            var checkBalance = (paymentType === 'flexible' ? user['balance'] >= handleCalcAmount() : true)
            if (!checkBalance) toast.error('Balance is not enough')
            tmp = checkBalance && courtInfo['courtStatus'] && !isOccupied;
            return tmp
        } catch (error) {
            return false;
        }
    };

    const completeBooking = async () => {
        try {
            const result = validateBooking();
            if (result) {
                document.cookie = `token=${sessionStorage.getItem('token')}; path=/paySuccess`
                pushBooking()
            }
        } catch (error) {
            toast.error('Server error')
        }
    };

    const handleCalcAmount = () => {
        try {
            let startTime = document.getElementById('time-start').value
            let endTime = document.getElementById('time-end').value
            let t = document.getElementById('monthNum');
            let monthNum = t == null ? 1 : t.value
            return calcAmount(bookingType, courtInfo['price'], startTime, endTime, monthNum)
        }
        catch (err) {
            return 0
        }
    }
    const calcAmount = (bkType, price, start, end, month) => {
        var amount = price * (end - start)
        if (bkType === 'fixed') amount *= (4 * month)
        setAmount(Object.is(amount, NaN) ? 0 : amount)
        return amount
    }
    const pushBooking = async () => {
        const buildTransferUrl = (method, start, end, userId, date, courtId, type, monthNum) => {
            let amount = calcAmount(bookingType, courtInfo['price'], start, end, monthNum)
            return `${apiUrl}/Booking/TransactionProcess?`
                + `Method=${method}&`
                + `Start=${start}&`
                + `End=${end}&`
                + `UserId=${userId}&`
                + `Date=${date}&`
                + `CourtId=${courtId}&`
                + `Type=${type}&`
                + `NumMonth=${monthNum}&`
                + `Amount=${amount}`
        }
        const buildFlexibleUrl = (date, start, end, courtId, userId) => {
            return `${apiUrl}/Slot/BookingByBalance?`
                + `date=${date}&`
                + `start=${start}&`
                + `end=${end}&`
                + `courtId=${courtId}&`
                + `userId=${userId}`
        }
        var token = sessionStorage.getItem('token')
        try {
            let startTime = document.getElementById('time-start').value
            let endTime = document.getElementById('time-end').value
            let t = document.getElementById('monthNum');
            let monthNum = t == null ? null : t.value
            let date = document.getElementById('datePicker').value
            let url = paymentType === 'flexible' ? buildFlexibleUrl(date, startTime, endTime, courtInfo['courtId'], user['userId'])
                : buildTransferUrl(transferMethod, startTime, endTime, user['userId'], date, courtInfo['courtId'], bookingType, monthNum)
            var res = await fetch(url,
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    }
                })
            handleFinishFetch(res, paymentType)
        }
        catch (err) {
            toast.error('Server error')
        }
    }
    const handleFinishFetch = async (res, type) => {
        const data = await res.json()
        if (type !== 'flexible') {
            var payUrl = data['url']
            if (payUrl !== undefined) {
                window.location.assign(payUrl)
            }
            return;
        }
        if (res.ok) {
            toast.success('Created booking!')
            setTimeout(() => {
                window.location.assign('/bookingHistory')
            }, 500);
            return
        }
        const msg = data['msg']
        if (msg.includes('not enough')) {
            alert(msg)
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
    return (
        <div className="bookCourt-container">
            <h1 className="bookCourt-title">BOOKING A COURT</h1>
            <div className="bookCourt-body">
                <div className="bookCourt-section bookCourt-left-section">
                    <h2 className="notes">1. SELECT A COURT</h2>
                    <div className="bookCourt-option1">
                        <label htmlFor="branch">BRANCH:</label>

                        <select id="branch" name="branch" onChange={() => {
                            setSelectedBranch(document.getElementById('branch').value)
                        }}>
                            <option value="" hidden selected>Choose a branch</option>
                            {
                                branches.map(b =>
                                (
                                    <option value={b["branchId"]}
                                        //Set selected branch
                                        selected={
                                            !Object.is(selectedBranch, undefined) && selectedBranch === b['branchId']
                                        }
                                    >{b["branchName"]}</option>
                                )
                                )
                            }
                        </select>
                    </div>
                    <div className="bookCourt-option2">
                        <label htmlFor="court">COURT:</label>

                        <select id="court" name="court" onChange={() => {
                            loadCourtInfo(document.getElementById('court').value)
                            checkAvailableSlot(document.getElementById('court').value)
                            handleCalcAmount()
                        }}>
                            {<option value="No" hidden selected>Choose a court</option>}
                            {
                                courts.map((c, i) => (
                                    c['branchId'] === selectedBranch &&
                                    <option value={c["courtId"]}
                                        selected={
                                            !Object.is(courtInfo, undefined) && c['courtId'] === courtInfo['courtId']
                                        }
                                    >{c["courtName"]}</option>
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
                                setBookingType('playOnce')
                                handleCalcAmount()
                            }}
                                checked={bookingType === 'playOnce'}
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
                                checkAvailableSlot(document.getElementById('court').value)
                                handleCalcAmount()
                            }
                        }}>
                            <option value="" hidden>Select Time</option>
                            {
                                timeBound.map(t => (
                                    <option value={t}>{t}:00:00</option>
                                ))
                            }
                        </select>
                        <span className="text">to</span>
                        <select id="time-end" name="time-end" onChange={() => {
                            if (validateDateTime() === 0) {
                                checkAvailableSlot(document.getElementById('court').value)
                                handleCalcAmount()
                            }
                        }}>
                            <option value="" hidden>Select Time</option>
                            {
                                timeBound.map(t => (
                                    <option value={t}>{t}:00:00</option>
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
                                checkAvailableSlot(document.getElementById('court').value)
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
                        <p>Price: <span className='priceSpan'>{formatNumber(amount)}</span></p>
                        <p>Your balance:
                            <span className='priceSpan'>
                                {' ' + (Object.is(user, undefined) || Object.is(user['balance'], undefined) ? '_' : formatNumber(user['balance']))}
                            </span>
                        </p>
                        <label htmlFor="paymentType">Payment type</label>
                        <div className='inlineDiv'>
                            <input type='radio' className="inputradioRight2" name='paymentType' value='banking'
                                onChange={() => { setPaymentType('banking') }}
                                checked={paymentType === 'banking'}
                            />
                            <span>Banking</span>
                        </div>
                        {
                            bookingType === 'playOnce' &&
                            (
                                <div className='inlineDiv'>
                                    <input type='radio' className="inputradioRight2" name='paymentType' value='timeBalance'
                                        onChange={() => {
                                            setPaymentType('flexible')
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