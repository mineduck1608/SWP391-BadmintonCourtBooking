import React, { useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import './viewpayment.css';
import { toast } from 'react-toastify';
import { Box, Button, TextField } from '@mui/material';

export default function ViewPayment() {
  const [payments, setPayments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [userId, setUserId] = useState('');
  const token = sessionStorage.getItem('token');
  const apiUrl = 'https://localhost:7233';

  const fetchPayments = async (userId) => {
    setLoading(true);
    setError(null);

    try {
      const paymentsResponse = await fetch(`${apiUrl}/Payment/GetByUser?id=${userId}`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!paymentsResponse.ok) {
        throw new Error('Failed to fetch payments');
      }

      const paymentsData = await paymentsResponse.json();
      setPayments(paymentsData);
      setLoading(false);
    } catch (err) {
      setError(err.message);
      setLoading(false);
    }
  };

  const handleSearch = () => {
    if (userId.trim() === '') {
      toast.error('Please enter a valid user ID');
      return;
    }
    fetchPayments(userId);
  };

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
              <h2>Payment History</h2>
              <Box display="flex" justifyContent="center" mb={2}>
                <TextField
                  label="User ID"
                  value={userId}
                  onChange={(e) => setUserId(e.target.value)}
                  variant="outlined"
                />
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleSearch}
                  style={{ marginLeft: '10px' }}
                >
                  Search
                </Button>
              </Box>
              <div className="view-payment-history">
                {loading ? (
                  <p>Loading payments...</p>
                ) : error ? (
                  <p className="view-payment-error-message">Error: {error}</p>
                ) : (
                  <div className="view-payment-table">
                    <table>
                      <thead>
                        <tr>
                          <th>Payment ID</th>
                          <th>Date</th>
                          <th>Time</th>
                          <th>Booking ID</th>
                          <th>Method</th>
                          <th>Amount</th>
                        </tr>
                      </thead>
                      <tbody>
                        {payments.map(payment => (
                          <tr key={payment.paymentId}>
                            <td>{payment.paymentId}</td>
                            <td>{new Date(payment.date).toLocaleDateString()}</td>
                            <td>{new Date(payment.date).toLocaleTimeString()}</td>
                            <td>{payment.bookingId}</td>
                            <td>{payment.method}</td>
                            <td>{formatNumber(payment.amount)}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
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
