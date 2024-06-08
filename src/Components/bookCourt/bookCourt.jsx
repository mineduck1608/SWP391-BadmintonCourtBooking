import React, { useState } from "react";
import './bookCourt.css';

const BookCourt = () => {
    const [bookingType, setBookingType] = useState('');

    return (
        <div className="bookCourt-container">
            <form action="">
                <h1 className="bookCourt-title">BOOKING A COURT</h1>
                <div className="bookCourt-body">
                    <div className="bookCourt-section bookCourt-left-section">
                        <h2>1. SELECT A COURT</h2>
                        <div className="bookCourt-option1">
                            <label htmlFor="branch">BRANCH:</label>
                            <select id="branch" name="branch">
                                <option value="0" disabled selected hidden>Branch Number</option>
                                <option value="1">Branch 1</option>
                                <option value="2">Branch 2</option>
                            </select>
                        </div>
                        <div className="bookCourt-option2">
                            <label htmlFor="court">COURT:</label>
                            <select id="court" name="court">
                                <option value="0" disabled selected hidden>Court Number</option>
                                <option value="1">Court 1</option>
                                <option value="2">Court 2</option>
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
                                <input className="inputradio" type="radio" id="once" name="booking-type" value="once" onChange={() => setBookingType('once')} />
                                <label htmlFor="once">Once (reserves at the specified time and date)</label>
                            </div>
                            <div className="bookCourt-form-group3">
                                <input className="inputradio" type="radio" id="flexible-time" name="booking-type" value="flexible-time" onChange={() => setBookingType('flexible-time')} />
                                <label htmlFor="flexible-time">Flexible Time (reserves for the entire month, custom time)</label>
                                {bookingType === 'flexible-time' && (
                                    <div className="bookCourt-form-subgroup">
                                        <label htmlFor="flexible-time-hours">Total:</label>
                                        <input type="number" id="flexible-time-hours" name="flexible-time-hours" min="1" max="744" />
                                        <span>hours</span>
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                    <div className="bookCourt-section bookCourt-right-section">
                        <h2>3. TIME AND DATE</h2>
                        <div className="bookCourt-form-group4">
                            <label htmlFor="time-start">Time:</label>
                            <select id="time-start" name="time-start">
                                <option value="">Select Time</option>
                                {/* Add time options here */}
                            </select>
                            <span>to</span>
                            <select id="time-end" name="time-end">
                                <option value="">Select Time</option>
                                {/* Add time options here */}
                            </select>
                        </div>
                        <div className="bookCourt-form-group5">
                            <label htmlFor="day">Day:</label>
                            <select id="day" name="day">
                                <option value="">Select Day</option>
                                {/* Add day options here */}
                            </select>
                        </div>
                        <h2 className="notes">4. NOTES</h2>
                        <textarea id="notes" name="notes" placeholder="Enter your notes here"></textarea>
                    </div>
                </div>
                <button type="submit" className="bookCourt-complete-booking-button">Complete Booking</button>
            </form>
        </div>
    );
}

export default BookCourt;
