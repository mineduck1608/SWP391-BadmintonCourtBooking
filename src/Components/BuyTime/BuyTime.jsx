import React, { useState } from 'react'
import './BuyTime.css'

//Need to fetch userID, bookingID and add new balance

const BuyTime = () => {
  const [userID, setUserID] = useState('ID');
  const [remainingTime, setremainingTime] = useState(1);
  const [buyHour, setBuyHour] = useState(0);
  const getRemainingTime = () => {
    //Lấy từ session
    return 0;
  }
  const adjustHour = (amount) => {
    if (buyHour + amount >= 0 && buyHour + amount <= 12)
      setBuyHour(bh => bh + amount)
  }
  return (
    <div className='buyTime'>

      <div className='buyTime_bodyContainer'>
        <h1 className='buyTime_title'>Buy more time for a flexible plan</h1>
        <article className='buyTime_article'>
          <h2 className='buyTime_title'>Buy more time</h2>
          <p className='buyTime_p'>Remaining time: {remainingTime}</p>
          <div className='buyTime_centerDiv'>
            <button className='buyTime_btn' onClick={() => adjustHour(-1)}>-</button>
            <input className='buyTime_counter1' type='number' value={buyHour} readOnly />
            <button className='buyTime_btn' onClick={() => adjustHour(1)}>+</button>
          </div>
          <div className='buyTime_centerDiv'>
            <button className='buyTime_btn' onClick={() => window.history.back()}>Cancel</button>
            <button className='buyTime_btn'>Confirm</button>
          </div>
        </article>
      </div>
    </div>
  )
}
export default BuyTime