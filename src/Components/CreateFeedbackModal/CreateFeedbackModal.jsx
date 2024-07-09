import React, { useState } from 'react';
import { Modal, Rate, Input, Button } from 'antd';
import './CreateFeedbackModal.css';
import { fetchWithAuth } from '../fetchWithAuth/fetchWithAuth';

const { TextArea } = Input;

const CreateFeedbackModal = ({ visible, onCancel, bookingId, branchId, userId }) => {
  const [feedback, setFeedback] = useState('');
  const [rating, setRating] = useState(0);

  const handleSubmitFeedback = async (event) => {
    event.preventDefault();

    try {
      const response = await fetchWithAuth('https://localhost:7233/Feedback/Post', {
        method: 'POST',
        body: JSON.stringify({
          rate: rating,
          content: feedback,
          id: userId,
          branchId: branchId,
        }),
        headers: {
          'Accept': '*/*',
        },
      });

      const responseData = await response.json();
      console.log('Feedback submitted successfully:', responseData);
      onCancel(); // Close the modal after submission
    } catch (error) {
      console.error('Error submitting feedback:', error);
    }
  };

  return (
    <Modal
      visible={visible}
      onCancel={onCancel}
      footer={null}
      centered={true}
    >
      <div className="cfm-fb-info">
        <h1>Feedback</h1>
        <p>Booking ID: {bookingId}</p>
        <p>Branch Id: {branchId}</p>
      </div>

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
        <div className="cfm-btn">
          <Button className="cfm-btn-cancel" type="default" onClick={onCancel}>Cancel</Button>
          <Button className="cfm-btn-submit" type="primary" htmlType="submit">Submit Feedback</Button>
        </div>
      </form>
    </Modal>
  );
};

export default CreateFeedbackModal;
