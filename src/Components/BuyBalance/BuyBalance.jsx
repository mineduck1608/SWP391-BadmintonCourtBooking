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
      <h1 className='buyBalance_title'>Buy more time for a flexible plan</h1>
      <div className='buyBalance_bodyContainer'>
        <aside className='buyBalance_aside'>
          <h2 className='buyBalance_title'>Buying more time for...</h2>
          <p>Booking ID: {bookingID}</p>
          <p>Remaining time: {getRemainingTime(userID, bookingID)}</p>
        </aside>
        <article className='buyBalance_article'>
          <h2 className='buyBalance_title'>Buy more time</h2>
          <div className='buyBalance_centerDiv'>
            <button className='buyBalance_btn' onClick={() => adjustHour(-1)}>-</button>
            <input className='buyBalance_counter1' type='number' value={buyHour} readOnly />
            <button className='buyBalance_btn' onClick={() => adjustHour(1)}>+</button>
          </div>
          <div className='buyBalance_centerDiv'>
            <button className='buyBalance_btn' onClick={() => window.history.back()}>Cancel</button>
            <button className='buyBalance_btn'>Confirm</button>
          </div>
        </article>
      </div>
    </div>
  )
}
export default BuyBalance