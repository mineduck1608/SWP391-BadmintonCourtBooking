import React, { useState, useEffect } from 'react';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import './viewpayment.css';
import { jwtDecode } from 'jwt-decode';
import {toast} from 'react-toastify';

export default function ViewPayment() {
  const [payments, setPayments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const token = sessionStorage.getItem('token');
  const apiUrl = 'https://localhost:7233';
  const decodedToken = jwtDecode(token);
  const userIdToken = decodedToken.UserId;

  useEffect(() => {
    const fetchPayments = async () => {
      setLoading(true);
      setError(null);

      try {
        const paymentsResponse = await fetch(`${apiUrl}/Payment/GetByUser?id=${userIdToken}`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!paymentsResponse.ok) {
          throw new Error('Failed to fetch payments');
        }

        
        const paymentsData = await paymentsResponse.json();
        console.log('Fetched Payment Data', paymentsData);


        setPayments(paymentsData);
        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
        toast.error('Error fetching payments: ' + err.message);
      }
    };

    fetchPayments();
  }, [token, apiUrl]);

  const formatNumber = (n) => {
    return n.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  };

  return (
    <div className='view-payment'>
      <div className='view-payment-header'>
        <Header />
      </div>
      <div className='view-payment-wrapper'>
        <div className='view-payment-background'>
          <div className="view-payment-profile-container">
            <div className="view-payment-profile-content">
              <h2>Payment Details</h2>
              <div className="view-payment-history">
                {loading ? (
                  <p>Loading payments...</p>
                ) : error ? (
                  <p className="view-payment-error-message">Error: {error}</p>
                ) : (
                  <div className="view-payment-table">
                    <div className="view-payment-table-wrapper">
                      {payments.length === 0 ? (
                        <p>No payments found</p>
                      ) : (
                        <>
                          <div className="view-payment-section">
                            <table className="view-payment-table">
                              <thead>
                                <tr>
                                  <th>PAYMENT ID</th>
                                  <th>DATE</th>
                                  <th>TIME</th>
                                  <th>BOOKING ID</th>
                                  <th>METHOD</th>
                                  <th>AMOUNT</th>
                                </tr>
                              </thead>
                              <tbody>
                                {payments.map(payment => (
                                  <tr key={payment.paymentId}>
                                    <td>{payment.paymentId}</td>
                                    <td>{new Date(payment.date).toLocaleDateString()}</td>
                                    <td>{new Date(payment.date).toLocaleTimeString()}</td>
                                    <td>{payment.bookingId}</td>
                                    <td>{payment.method === 1 ? 'VnPay' : payment.method === 2 ? 'Momo' : payment.method}</td>
                                    <td>{formatNumber(payment.amount)}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          </div>
                        </>
                      )}
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className='view-payment-footer'>
        <Footer />
      </div>
    </div>
  );
}
