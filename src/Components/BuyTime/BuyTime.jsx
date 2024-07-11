import React, { useEffect, useState } from 'react'
import './BuyTime.css'
import { jwtDecode } from 'jwt-decode';
import { toast } from 'react-toastify';
import { fetchWithAuth } from '../fetchWithAuth/fetchWithAuth';
import { Modal } from 'antd';

const BuyTime = () => {
  const [userID, setUserID] = useState('');
  const [remainingTime, setRemainingTime] = useState(0);
  const [validAmount, setValidAmount] = useState(false)
  const apiUrl = 'https://localhost:7233'
  var token = sessionStorage.getItem('token')
  const [discounts, setDiscounts] = useState([])
  const [open, setOpen] = useState(false)
  const [amount, setAmount] = useState(0)
  const minAmount = 10000
  const fetchDiscounts = async () => {
    try {
      setDiscounts([])
      const discountData = await (
        await fetch(`${apiUrl}/Discount/GetAll`, {
          headers: {
            Authorization: `Bearer ${sessionStorage.getItem('token')}`
          }
        })
      ).json()
      for (let index = 0; index < discountData.length; index++) {
        setDiscounts(t => [...t, discountData[index]])
      }
    }
    catch (err) {
      toast.error('Server error')
    }
  }
  const process = (value) => {
    var r = value.replace('.', '').replace(/[^0-9]/g, '')
    return parseInt(r)
  }
  const validateAmount = (amount) => {
    let r = amount >= minAmount
    setValidAmount(r)
    return r
  }
  const completeBooking = async () => {
    if (validateAmount(amount)) {
      //Create a cookie
      document.cookie = `token=${sessionStorage.getItem('token')}; path=/paySuccess`
      try {
        let method = document.getElementById('method').value
        var res = await fetchWithAuth(`${apiUrl}/Booking/TransactionProcess?`
          + `Method=${method}&`
          + `UserId=${userID}&`
          + `Type=flexible&`
          + `Amount=${amount}`,
          {
            method: 'POST',
            headers: {
              'Authorization': `bearer ${token}`,
              'Content-Type': 'application/json'
            }
          })
        try {
          var data = await (res.json())
          window.location.replace(data['url'])
        }
        catch (err) {
          toast.error('Server error')
        }
      }
      catch (err) {
        toast.error('Server error')
      }
    }
  }
  //Get the userID
  useEffect(() => {
    async function fetchUserBalance() {
      try {
        var decodedToken = jwtDecode(token)
        setUserID(u => decodedToken.UserId)
        var res = await fetchWithAuth(`${apiUrl}/User/GetById?id=${decodedToken.UserId}`, {
          method: 'GET',
          headers: {
            'Authorization': `bearer ${token}`,
            'Content-Type': 'application/json'
          }
        })
        var data = await res.json()
        if (decodedToken.UserId !== data['userId']) {
          throw new Error('Authorize failed')
        } else {
          setUserID(data['userId'])
          setRemainingTime(data['balance'])
        }
      }
      catch (err) {
        toast.error('Server error')
      }

    }
    fetchUserBalance()
    fetchDiscounts()
  }, [])

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
    if (Object.is(n, NaN)) return 0
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
    <div className='buyTime'>
      <Modal title='Here are our discounts'
        open={open}
        footer={null}
        centered={true}
        closable={false}
      >
        <span>
          <table className='discount-table'>
            <thead>
              <tr>
                <th>Amount</th>
                <th>Proportion</th>
              </tr>
            </thead>
            <tbody>
              {
                discounts.map(d => (
                  <tr>
                    <td>{formatNumber(d.amount)}</td>
                    <td>{d.proportion}%</td>
                  </tr>
                ))
              }
            </tbody>
          </table>
          <div className='right-align-btn'>
            <button onClick={() => setOpen(false)}>Close</button>
          </div>
        </span>
      </Modal>
      <div className='buyTime_bodyContainer'>
        <h1 className='buyTime_title'>Buy more flexible money</h1>
        <article className='buyTime_article'>
          <p className='buyTime_p'>Remaining money: {formatNumber(Math.floor(remainingTime))}</p>
          <div className='buyTime_centerDiv'>
            <input className='buyTime_counter1' id='amount' type='text'
              value={formatNumber(amount)}
              inputMode='numeric' onChange={async (e) => {
                let t = process(e.target.value)
                setAmount(t)
                validateAmount(t)
              }} />

          </div>
          {!validAmount && (
            <p className='buyTime_err'>Input a valid number of money to buy!<br />Min: {formatNumber(minAmount)}đ</p>
          )}
          <p className='buyTime_p' >Payment method</p>
          <select className='buyTime_select' id='method'>
            <option value={1}>VnPay</option>
            <option value={2}>MoMo</option>
          </select>
          <div className='buyTime_centerDiv'>
            <button className='buyTime_btn' onClick={() => window.history.back()}>Cancel</button>
            <button className='buyTime_btn' onClick={() => {
              completeBooking()
            }}>Confirm</button>
            <button
              className='buyTime_btn'
              onClick={() => {
                setOpen(true)
              }}>
              View discounts
            </button>
          </div>
        </article>
      </div>
    </div>
  )
}
export default BuyTime