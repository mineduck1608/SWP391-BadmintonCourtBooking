import React, { useEffect, useState } from "react";
import './bookCourt.css';
import { DatePicker, Space } from "antd";

const BookCourt = () => {
    const [bookingType, setBookingType] = useState('');
    const [branches, setBranches] = useState([])
    const [courts, setCourts] = useState([])
    const [paymentType, setPaymentType] = useState('');
    const [timeBound, setTimeBound] = useState([])
    const [curDate, setCurDate] = useState('')
    const [validDate, setValidDate] = useState(false)
    const [validTimeRange, setValidTimeRange] = useState(false)
    //var branches = []
    //var courts = []
    const apiUrl = "http://localhost:5266/"
    const fetchBranches = async () => {
        const branchData = await (
            await fetch(`${apiUrl}Branch/GetAll`)
        ).json()
        setBranches(branchData)
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
    const fetchCourts = async () => {
        var branchId = document.getElementById("branch").value
        const courtData = await (
            await fetch(`${apiUrl}Court/GetByBranch?id=${branchId}`)
        ).json()
        setCourts(courtData)
    }
    const validateDate = () => {
        var value = document.getElementById("datePicker").value.replace(/-/g, "/")
        var selectedDate = Date.parse(value)
        setValidDate(!(selectedDate < curDate))
    }
    const validateTime = () => {
        var t1 = document.getElementById("time-start").value
        var t2 = document.getElementById("time-end").value
        setValidTimeRange(t1<t2)
    }
    const completeBooking = (cond) => {
        if(cond){
            console.log("Completed")
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
                        <select id="branch" name="branch" onChange={() => fetchCourts()}>
                            <option value="" hidden>Choose a branch</option>
                            {
                                branches.map(b => (
                                    <option value={b["branchId"]}>{b["branchName"]}</option>
                                ))
                            }
                        </select>
                    </div>
                    <div className="bookCourt-option2">
                        <label htmlFor="court">COURT:</label>
                        <select id="court" name="court">
                            {<option value="No" hidden selected>Choose a court</option>}
                            {
                                courts.map((c, i) => (
                                    <option value={c["courtId"]} selected>{c["courtId"]}</option>
                                ))
                            }
                        </select>
                    </div>
                    <h2 className="notes">2. TYPE OF BOOKING</h2>
                    <p>SELECT ONE TYPE OF BOOKING:</p>
                    <div className="bookCourt-radio-group">
                        <div className="bookCourt-form-group1">
                            <input className="inputradio" type="radio" id="fixed-time" name="booking-type" value="fixed-time" onChange={() => setBookingType('fixed-time')} />
                            <label htmlFor="fixed-time">Fixed Time (reserves at the specified time for the entire months)</label>
                            {bookingType === 'fixed-time' && (
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
                            <input className="inputradio" type="radio" id="once" name="booking-type" value="once" onChange={() => { setBookingType('once') }} />
                            <label htmlFor="once">Once (reserves at the specified time and date)</label>
                            {
                                bookingType === 'once' && (
                                    <div>
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
                                    </div>
                                )
                            }
                        </div>
                        {/* <div className="bookCourt-form-group3">
                            <input className="inputradio" type="radio" id="flexible-time" name="booking-type" value="flexible-time" onChange={() => setBookingType('flexible-time')} />
                            <label htmlFor="flexible-time">Flexible Time (reserves for the entire month, custom time)</label>
                            {bookingType === 'flexible-time' && (
                                <div className="bookCourt-form-subgroup">
                                    <label htmlFor="flexible-time-hours">Total:</label>
                                    <input type="number" id="flexible-time-hours" name="flexible-time-hours" min="1" max="744" />
                                    <span>hours</span>
                                </div>
                            )}
                        </div> */}
                    </div>
                </div>
                <div className="bookCourt-section bookCourt-right-section">
                    <h2 className="notes">3. TIME AND DATE</h2>
                    <div className="bookCourt-form-group4">
                        <label htmlFor="time-start">Time:</label>
                        <select id="time-start" name="time-start" onChange={()=>validateTime()}>
                            <option value="" hidden>Select Time</option>
                            {
                                timeBound.map(t => (
                                    <option value={t}>{t}:00</option>
                                ))
                            }
                        </select>
                        <span>to</span>
                        <select id="time-end" name="time-end" onChange={()=>validateTime()}>
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
            <button type="submit" className="bookCourt-complete-booking-button" onClick={()=>completeBooking(validDate && validTimeRange)}>Complete Booking</button>

        </div>
    );
}

export default BookCourt;