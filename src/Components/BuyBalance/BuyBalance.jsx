import React, { useState } from 'react'
import './BuyBalance.css'

//Need to fetch userID, bookingID and add new balance

const BuyBalance = () => {
  const [userID, setUserID] = useState('ID');
  const [bookingID, setBookingID] = useState(0);
  const [buyHour, setBuyHour] = useState(0);
  const getRemainingTime = (userID, bookingID) => {
    return 0;
  }
  const adjustHour = (amount) => {
    if (buyHour + amount >= 0 && buyHour + amount <= 12)
      setBuyHour(bh => bh + amount)
  }
  return (
    <div className='buyBalance'>
      <h1 className='title'>Buy more time for a flexible plan</h1>
      <div className='bodyContainer'>
        <aside>
          <h2>Buying more time for...</h2>
          <p>Booking ID: {bookingID}</p>
          <p>Remaining time: {getRemainingTime(userID, bookingID)}</p>
        </aside>
        <article>
          <h2 className='buyMoreTime'>Buy more time</h2>
          <div className='centerDiv'>
            <button className='adjustHour' onClick={() => adjustHour(-1)}>-</button>
            <input className='counter' type='number' value={buyHour} readOnly />
            <button className='adjustHour' onClick={() => adjustHour(1)}>+</button>
          </div>
          <div className='centerDiv'>
            <button onClick={() => window.history.back()}>Cancel</button>
            <button>Confirm</button>
          </div>
        </article>
      </div>
    </div>
  )
}
export default BuyBalance