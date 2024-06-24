import React, { useEffect, useState } from 'react'
import './BuyTime.css'
import { jwtDecode } from 'jwt-decode';

const BuyTime = () => {
  const [userID, setUserID] = useState('');
  const [remainingTime, setRemainingTime] = useState(0);
  const [validAmount, setValidAmount] = useState(false)
  const apiUrl = 'http://localhost:5266/'
  const returnUrl = 'http://localhost:3000/home'
  const intRegex = /^(0{0,})[1-9]{1}[0-9]*$/
  var amount
  const validateAmount = () => {
    var temp = (document.getElementById('amount').value).toString()
    console.log(temp);
    if (!Object.is(temp, undefined) && intRegex.test(temp)) {
      amount = parseInt(temp)
      setValidAmount(amount >= 10)
    }
    else
      setValidAmount(false)
  }
  const completeBooking = async () => {
    validateAmount()
    if (validAmount) {
      //Create a cookie
      document.cookie = `token=${sessionStorage.getItem('token')}; path=/paySuccess`
      try {
        let method = document.getElementById('method').value
        var res = await fetch(`${apiUrl}Booking/TransactionProcess?`
          + `Method=${method}&`
          + `UserId=${userID}&`
          + `Type=buyTime&`
          + `Amount=${amount * 1000}`,
          {
            method: 'post',
            headers: {
              'Content-Type': 'application/json'
            }
          })
        try {
          var data = await (res.json())
          //window.location.replace(data['url'])
        }
        catch (err) {

        }
      }
      catch (err) {
        alert(err)
      }
    }
  }
  //Get the userID
  useEffect(() => {
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
            setRemainingTime(data['balance'])
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
          <p className='buyTime_p'>Convert rate: 1.000đ = 1 coin</p>
          <div className='buyTime_centerDiv'>
            <input className='buyTime_counter1' id='amount' type='number' min='10' onChange={() => validateAmount()} />
          </div>
          {!validAmount && (
            <p className='buyTime_err'>Input a valid number of coins to buy!</p>
          )}
          <p className='buyTime_p' >Payment method</p>
          <select className='buyTime_select' id='method'>
            <option value={2}>MoMo</option>
            <option value={1}>VnPay</option>
          </select>
          <div className='buyTime_centerDiv'>
            <button className='buyTime_btn' onClick={() => window.history.back()}>Cancel</button>
            <button className='buyTime_btn' onClick={() => {
              completeBooking()
            }}>Confirm</button>
          </div>
        </article>
      </div>
    </div>
  )
}
export default BuyTime