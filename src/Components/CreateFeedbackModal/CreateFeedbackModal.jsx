import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { Rate, Input, Button } from 'antd';
import './CreateFeedbackModal.css';

const { TextArea } = Input;

const CreateFeedbackModal = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { bookingId, branchId, userId } = location.state || {};
  const [feedback, setFeedback] = useState('');
  const [rating, setRating] = useState(0);

  const handleSubmitFeedback = async (event) => {
    event.preventDefault();
    const token = sessionStorage.getItem('token');

    try {
      const response = await axios.post('https://localhost:7233/Feedback/Post', null, {
        params: {
          rate: rating,
          content: feedback,
          id: userId,
          branchId: branchId
        },
        headers: {
          'Authorization': `Bearer ${token}`,
          'accept': '*/*'
        }
      });

      console.log('Feedback submitted successfully:', response.data);
      navigate('/bookingHistory'); 
    } catch (error) {
      console.error('Error submitting feedback:', error);
    }
  };

  const handleCancel = () => {
    navigate('/bookingHistory');
  };

  return (
    <div className="cfm-modal-overlay">
      <div className="cfm-modal-content">
        <h2>Create Feedback</h2>
        <p>Booking ID: {bookingId}</p>
        <p>Branch Id: {branchId}</p>

        <form onSubmit={handleSubmitFeedback}>
          <div className="cfm-form-group">
            <label htmlFor="rating">Rating:</label>
            <Rate
              id="rating"
              name="rating"
              value={rating}
              onChange={(value) => setRating(value)}
            />
          </div>
          <div className="cfm-feedback-group">
            <label htmlFor="feedback">Feedback:</label>
            <TextArea
              id="feedback"
              name="feedback"
              rows={4}
              value={feedback}
              onChange={(e) => setFeedback(e.target.value)}
            />
          </div>
          <Button className="cfm-btn-submit" type="primary" htmlType="submit">Submit Feedback</Button>
          <Button className="cfm-btn-cancel" type="default" onClick={handleCancel}>Cancel</Button>
        </form>
      </div>
    </div>
  );
};

export default CreateFeedbackModal;
