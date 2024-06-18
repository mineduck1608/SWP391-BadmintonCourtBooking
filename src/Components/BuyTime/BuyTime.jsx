import React, { useEffect, useState } from 'react'
import './BuyTime.css'
import { jwtDecode } from 'jwt-decode';

//Need to fetch userID, bookingID and add new balance

const BuyTime = () => {
  const [userID, setUserID] = useState('');
  const [remainingTime, setremainingTime] = useState(0);
  const [buyHour, setBuyHour] = useState(0);
  const apiUrl = "http://localhost:5266/"
  const adjustHour = (amount) => {
    if (buyHour + amount >= 0 && buyHour + amount <= 12)
      setBuyHour(bh => bh + amount)
  }
  //Get the userID
  useEffect(() => {
    async function fetchData() {
      var token = sessionStorage.getItem("token")
      if (!token) {
        alert("Please log in")
      } else {
        try {
          var decodedToken = jwtDecode(token)
          setUserID(u => decodedToken.UserId)
          var res = await fetch(`${apiUrl}User/GetById?id=${decodedToken.UserId}`)
          var data = await res.json()
          if (decodedToken.UserId !== data["userId"]) {
            throw new Error("Authorize failed")
          } else {
            setremainingTime(data["balance"])
          }
        }
        catch (err) {
          console.log(err)
        }
      }
    }
    fetchData()
  }, [])
  return (
    <div className='buyTime'>
      <div className='buyTime_bodyContainer'>
        <h1 className='buyTime_title'>Buy more time for a flexible plan</h1>
        <article className='buyTime_article'>
          <p className='buyTime_p'>Remaining time: {remainingTime}</p>
          <div className='buyTime_centerDiv'>
            <button className='buyTime_btn' onClick={() => adjustHour(-1)}>-</button>
            <input className='buyTime_counter1' type='number' value={buyHour} readOnly />
            <button className='buyTime_btn' onClick={() => adjustHour(1)}>+</button>
          </div>
          <p className='buyTime_p'>Payment method</p>
          <select >
            <option value="Momo">MoMo</option>
            <option value="Vnpay">VnPay</option>
          </select>
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