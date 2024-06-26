import React, { useEffect, useState } from 'react';
import './BuyFail.css';
import { jwtDecode } from 'jwt-decode';

const BuyFail = () => {
  const [userID, setUserID] = useState('');
  const [remainingTime, setRemainingTime] = useState(0);
  const [errorMessage, setErrorMessage] = useState('');
  const apiUrl = 'https://localhost:7233/';

  // Get the userID
  useEffect(() => {
    async function fetchData() {
      var token = sessionStorage.getItem('token');
      if (!token) {
        setErrorMessage('Please log in');
      } else {
        try {
          var decodedToken = jwtDecode(token);
          setUserID(u => decodedToken.UserId);
          var res = await fetch(`${apiUrl}User/GetById?id=${decodedToken.UserId}`);
          var data = await res.json();
          if (decodedToken.UserId !== data['userId']) {
            throw new Error('Authorize failed');
          } else {
            setUserID(data['userId']);
            setRemainingTime(data['balance']);
          }
        } catch (err) {
          setErrorMessage('Authorization failed: ' + err.message);
        }
      }
    }
    fetchData();
  }, []);

  return (
    <div className='pay-success-background'>
    <div className='buyFail'>
      <div className='buyFail_bodyContainer'>
        <h1 className='buyFail_title'>Transaction Failed</h1>
        <article className='buyFail_article'>
          <p className='buyFail_p'>Remaining balance: {remainingTime}</p>
          {errorMessage && <p className='buyFail_err'>{errorMessage}</p>}
          <p className='buyFail_p'>Unfortunately, your transaction could not be completed. Please try again later.</p>
          <div className='buyFail_centerDiv'>
            <button className='buyFail_btn' onClick={() => window.location.replace('/buyTime')}>Try Again</button>
          </div>
        </article>
      </div>
    </div>
    </div>
  );
}
export default BuyFail;
