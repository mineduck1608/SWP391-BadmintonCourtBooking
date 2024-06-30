import React, { useEffect, useState } from 'react';
import './PaySuccess.css';
import '@fortawesome/fontawesome-free/css/all.min.css';

const PaySuccess = () => {
  const [success, setSuccess] = useState(null); // null initially, can be 1 for success or -1 for fail

  useEffect(() => {
    async function checkPaymentStatus() {
      const queryStr = window.location.search;
      const urlParams = new URLSearchParams(queryStr);
      const msg = urlParams.get('msg');
      
      // Set success state based on msg parameter
      if (msg === 'Success') {
        setSuccess(1);
      } else if (msg === 'Fail') {
        setSuccess(-1);
      }
    }

    checkPaymentStatus();
    let token = getCookie('token');
    sessionStorage.setItem('token', token);
    document.cookie = 'x=';
  }, []);

  const handleConfirm = () => {
    // Redirect based on the success state
    if (success === 1) {
      alert("You'll be redirected to the main page!");
      window.location.replace('/home');
    } else if (success === -1) {
      alert("There was an issue with your payment. Redirecting to buy flexible...");
      window.location.replace('/buyTime'); // Change this URL to where users should go to retry payment
    }
  }

  return (
    <div className={`pay-success-background ${success === -1 ? 'failure' : ''}`}>
      <div className='pay-success-paymentresult'>
        <div className="pay-success-container-paymentresult">
          {success === 1 ? (
            <>
              <div className="icon-payment">
                <i className="fas fa-check-circle" style={{ color: '#4CAF50' }}></i>
              </div>
              <h1>Thank You for Your Purchase!</h1>
              <p>Your transaction has been successfully completed.</p>
            </>
          ) : (
            <>
              <div className="icon-payment">
                <i className="fas fa-times-circle" style={{ color: '#e53935' }}></i>
              </div>
              <h1>Transaction Failed</h1>
              <p>Unfortunately, your transaction could not be completed. Please try again.</p>
            </>
          )}
          <button onClick={handleConfirm}>{success === 1 ? 'Go to Home Page' : 'Try Again'}</button>
        </div>
      </div>
    </div>
  );

  function getCookie(cname) {
    let name = cname + '=';
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) === ' ') {
        c = c.substring(1);
      }
      if (c.indexOf(name) === 0) {
        return c.substring(name.length, c.length);
      }
    }
    return "";
  }
}

export default PaySuccess;
