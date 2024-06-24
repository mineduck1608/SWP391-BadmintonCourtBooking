import React, { useEffect, useState } from 'react'
import './PaySuccess.css'
const PaySuccess = () => {
  //0 = fetching, 1 = ok, -1 = fail
  const [success, setSuccess] = useState(0)
  
  useEffect(() => {
    async function saveToDB() {
      const queryStr = window.location.search
      const urlParams = new URLSearchParams(queryStr)
      alert(urlParams.get('msg'))
      // var bankResult = queryStr.indexOf("orderInfo") === -1 ? "MomoResult" : "PaymentResult"
      // const resp = await fetch(`${apiUrl}Payment/${bankResult}${queryStr}`)
      // if (resp.ok) {
      //   alert('Good!')
      // }
      // else {
      //   alert('Oops')
      // }
      // window.location.replace('/home')
    }
    saveToDB()
    //Find the token
    let token = getCookie('token')
    sessionStorage.setItem('token', token)
    document.cookie = 'x='
    alert('You\'ll be redirected to main page!')
  }, [])
  return (
    <div>Thank you for using our services</div>
  )
  function getCookie(cname) {
    let name = cname + '=';
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) == ' ') {
        c = c.substring(1);
      }
      if (c.indexOf(name) == 0) {
        return c.substring(name.length, c.length);
      }
    }
    return "";
  }
}
export default PaySuccess