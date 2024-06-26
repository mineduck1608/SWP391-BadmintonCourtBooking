import React, { useEffect, useState } from 'react'
import './PaySuccess.css'

const PaySuccess = () => {
  const [success, setSuccess] = useState(0)

  useEffect(() => {
    async function saveToDB() {
      const queryStr = window.location.search
      const urlParams = new URLSearchParams(queryStr)
      const msg = urlParams.get('msg')
    }
    saveToDB()
    let token = getCookie('token')
    sessionStorage.setItem('token', token)
    document.cookie = 'x='
  }, [])

  const handleConfirm = () => {
    alert("You'll be redirected to the main page!")
    window.location.replace('/home') // Change '/home' to your desired redirect URL
  }

  return (
    <div className='pay-success-background'>
      <div className="pay-success">
        <h1>Thank You for Your Purchase!</h1>
        <p>Your transaction has been successfully completed.</p>
        {success === 1 && <p>Payment was successful!</p>}
        {success === -1 && <p>There was an issue with your payment. Please try again.</p>}
        <button onClick={handleConfirm}>Go to Home Page</button>
      </div>
    </div>
  )

  function getCookie(cname) {
    let name = cname + '='
    let decodedCookie = decodeURIComponent(document.cookie)
    let ca = decodedCookie.split(';')
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i]
      while (c.charAt(0) === ' ') {
        c = c.substring(1)
      }
      if (c.indexOf(name) === 0) {
        return c.substring(name.length, c.length)
      }
    }
    return ""
  }
}

export default PaySuccess
