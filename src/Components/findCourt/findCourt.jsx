import React, { useState, useEffect } from 'react';
import './findcourt.css';
import { TimePicker, Modal, Button, Rate, Input } from 'antd';
import { useLocation, useNavigate } from 'react-router-dom';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import image2 from '../../Assets/image2.jpg';
import userImg from '../../Assets/user.jpg';
import { jwtDecode } from 'jwt-decode';
import { fetchWithAuth } from '../fetchWithAuth/fetchWithAuth';
import { SettingsSuggestOutlined } from '@mui/icons-material';

const { RangePicker } = TimePicker;
const { TextArea } = Input;

const FindCourt = () => {
  const [timeRange, setTimeRange] = useState([null, null]);
  const [courts, setCourts] = useState([]);
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedBranch, setSelectedBranch] = useState('');
  const [selectedCourt, setSelectedCourt] = useState('');
  const [feedback, setFeedback] = useState([]);
  const [users, setUsers] = useState([]);
  const [loadingFeedback, setLoadingFeedback] = useState(false);
  const [errorFeedback, setErrorFeedback] = useState('');
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [newRating, setNewRating] = useState(0);
  const [newContent, setNewContent] = useState('');
  const [selectedFeedback, setSelectedFeedback] = useState(null);

  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const branchId = params.get('branch');
    if (branchId) {
      setSelectedBranch(branchId);
    }
  }, [location]);

  const onChange = (time) => {
    setTimeRange(time);
  };

  const handleBranchChange = (event) => {
    const branchId = event.target.value;
    setSelectedBranch(branchId);
    setSelectedCourt(''); // Reset selected court when branch changes
  };

  const handleCourtChange = (event) => {
    const courtId = event.target.value;
    setSelectedCourt(courtId);
  };

  const handleDelete = async (feedbackId, feedbackUserId) => {
    const token = sessionStorage.getItem('token');
    let decodedToken;
    try {
      decodedToken = jwtDecode(token);
    } catch (error) {
      sessionStorage.clear();
      navigate('/');
      return;
    }

    const currentUserId = decodedToken.UserId;

    if (currentUserId !== feedbackUserId) {
      alert("You don't have permission to delete this feedback.");
      return;
    }

    try {
      const response = await fetchWithAuth(
        `https://localhost:7233/Feedback/Delete?id=${feedbackId}&userID=${currentUserId}`,
        {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        }
      );

      if (!response.ok) {
        throw new Error('Failed to delete feedback');
      }

      // Remove the deleted feedback from the state
      setFeedback(prevFeedback => prevFeedback.filter(fb => fb.feedbackId !== feedbackId));
      alert('Feedback deleted successfully');
    } catch (error) {
      console.error('Error deleting feedback:', error);
      alert('Failed to delete feedback. Please try again.');
    }
  };

  const filteredCourts = courts.filter((court) => {
    return (
      (selectedBranch === '' || court.branchId === selectedBranch) &&
      (selectedCourt === '' || court.courtId === selectedCourt)
    );
  });

  const filteredFeedback = feedback.filter((fb) => {
    return (
      selectedBranch === '' || fb.branchId === selectedBranch
    );
  });

  const extractImageUrls = (courtImg) => {
    const regex = /Image \d+:(https?:\/\/[^\s,]+)/g;
    let matches;
    const urls = [];
    while ((matches = regex.exec(courtImg)) !== null) {
      urls.push(matches[1]);
    }
    return urls;
  };

  const formatDateTime = (dateString) => {
    const date = new Date(dateString);
    const formattedDate = date.toLocaleDateString('en-GB', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).replace(/\//g, '-');
    const formattedTime = date.toLocaleTimeString('en-GB', {
      hour: '2-digit',
      minute: '2-digit'
    });
    return `${formattedDate} ${formattedTime}`;
  };

  useEffect(() => {
    const fetchData = async () => {
      const branchUrl = 'https://localhost:7233/Branch/GetAll';
      const courtUrl = 'https://localhost:7233/Court/GetAll';

      try {
        setLoading(true);
        const [branchResponse, courtResponse] = await Promise.all([
          fetchWithAuth(branchUrl),
          fetchWithAuth(courtUrl),
        ]);

        if (!branchResponse.ok) {
          throw new Error(`Failed to fetch branch data: ${branchResponse.statusText}`);
        }
        if (!courtResponse.ok) {
          throw new Error(`Failed to fetch court data: ${courtResponse.statusText}`);
        }

        const branchData = await branchResponse.json();
        const courtData = await courtResponse.json();

        const filteredBranchData = branchData.filter(branch => branch.branchStatus !== 0);

        const courtsWithImages = courtData.map(court => {
          const imageUrl = court.courtImg ? extractImageUrls(court.courtImg)[0] : image2;
          return {
            ...court,
            image: imageUrl
          };
        });

        setBranches(filteredBranchData);
        setCourts(courtsWithImages);

      } catch (error) {
        setError(error.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  useEffect(() => {
    const fetchFeedback = async () => {
      const token = sessionStorage.getItem('token');

      const feedbackUrl = 'https://localhost:7233/Feedback/GetAll';
      const userUrl = 'https://localhost:7233/User/GetAll';
      const userDetailsUrl = 'https://localhost:7233/UserDetail/GetAll';

      setLoadingFeedback(true);
      try {
        const [feedbackResponse, userResponse, userDetailsResponse] = await Promise.all([
          fetchWithAuth(feedbackUrl),
          fetchWithAuth(userUrl, {
            method: 'GET',
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          }),
          fetchWithAuth(userDetailsUrl)
        ]);

        if (!feedbackResponse.ok) {
          throw new Error(`Failed to fetch feedback data: ${feedbackResponse.statusText}`);
        }
        if (!userResponse.ok) {
          throw new Error(`Failed to fetch user data: ${userResponse.statusText}`);
        }
        if (!userDetailsResponse.ok) {
          throw new Error(`Failed to fetch user details: ${userDetailsResponse.statusText}`);
        }

        const feedbackData = await feedbackResponse.json();
        console.log('fetched feedback data', feedbackData);
        const userData = await userResponse.json();
        const userDetailsData = await userDetailsResponse.json();

        const feedbackWithUserDetails = feedbackData.map(fb => {
          const user = userData.find(user => user.userId === fb.userId);
          const userDetails = userDetailsData.find(detail => detail.userId === fb.userId);
          return {
            ...fb,
            formattedDateTime: formatDateTime(fb.period),
            user: {
              ...user,
              image: userDetails ? userDetails.img : null,
              firstName: userDetails ? userDetails.firstName : '',
              lastName: userDetails ? userDetails.lastName : ''
            }
          };
        });

        setFeedback(feedbackWithUserDetails);
        setUsers(userData);
      } catch (error) {
        setErrorFeedback('Failed to load feedback');
      } finally {
        setLoadingFeedback(false);
      }
    };

    fetchFeedback();
  }, []);

  const handleBook = (courtId) => {
    navigate(`/viewCourtInfo?courtId=${courtId}`);
  };

  const renderStars = (rating) => {
    const fullStar = '★';
    const emptyStar = '☆';
    return (
      <span className="stars">
        {fullStar.repeat(rating) + emptyStar.repeat(5 - rating)}
      </span>
    );
  };

  const handleEdit = (feedbackId, rating, content) => {
    setSelectedFeedback({ feedbackId, rating, content });
    setNewRating(rating);
    setNewContent(content);
    setIsModalVisible(true);
  };

  const handleModalOk = async () => {
    const token = sessionStorage.getItem('token');
    const decodedToken = jwtDecode(token);
    const userIdToken = decodedToken.UserId;

    const feedbackData = {
      userId: userIdToken,
      rating: newRating,
      content: newContent,
      feedbackId: selectedFeedback ? selectedFeedback.feedbackId : undefined,
    };

    try {
      const url = `https://localhost:7233/Feedback/Update?rate=${feedbackData.rating}&content=${feedbackData.content}&id=${feedbackData.feedbackId}&userId=${feedbackData.userId}`;
      const response = await fetchWithAuth(url, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) {
        throw new Error(`Failed to update feedback: ${response.statusText}`);
      }

      const updatedFeedback = await response.json();
      setFeedback((prevFeedback) =>
        prevFeedback.map((fb) =>
          fb.feedbackId === selectedFeedback.feedbackId ? updatedFeedback : fb
        )
      );

      setIsModalVisible(false);
      setSelectedFeedback(null);
      setNewRating(0);
      setNewContent('');
      window.location.reload();
    } catch (error) {
      console.error(error);
      setErrorFeedback('Failed to update feedback');
    }
  };

  const handleModalCancel = () => {
    setIsModalVisible(false);
    setSelectedFeedback(null);
    setNewRating(0);
    setNewContent('');
  };

  const sendFeedback = async (feedback) => {
    const url = `https://localhost:7233/Feedback/Update/${feedback.feedbackId}`;
    const token = sessionStorage.getItem('token');

    try {
      const response = await fetchWithAuth(url, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(feedback),
      });

      if (!response.ok) {
        throw new Error(`Failed to update feedback: ${response.statusText}`);
      }

      const result = await response.json();
      return result;
    } catch (error) {
      console.error(error);
      setErrorFeedback(`Failed to update feedback: ${error.message}`);
    }
  };

  return (
    <div className="findCourt">
      <div className="findCourtHeader">
        <Header />
      </div>
      <div className="findCourt-wrapper">
        <div className="background">
          <div>
            <section className="findCourt-find">
              <div className="secContainer container">
                <div className="findCourt-homeText">
                  <h1 className="findcourt-Title">Find a Court</h1>
                </div>

                <div className="findCourt-searchCard grid">
                  <div className="branchDiv">
                    <label htmlFor="branch">Branch</label>
                    <select
                      onChange={handleBranchChange}
                      value={selectedBranch}
                    >
                      {branches.map((branch) => (
                        <option key={branch.branchId} value={branch.branchId}>
                          {branch.branchName}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="findCourt-courtDiv">
                    <label htmlFor="court">Court</label>
                    <select
                      onChange={handleCourtChange}
                      value={selectedCourt}
                    >
                      {courts
                        .filter(court => !selectedBranch || court.branchId === selectedBranch)
                        .map((court) => (
                          <option key={court.courtId} value={court.courtId}>
                            {court.courtName}
                          </option>
                        ))}
                    </select>
                  </div>
                </div>
              </div>

              <div className="findCourt-courtList">
                {loading && <p>Loading...</p>}
                {error && <p>{error}</p>}
                {filteredCourts.map((court) => {
                  const branch = branches.find(branch => branch.branchId === court.branchId);
                  return (
                    <div className="findCourt-courtCard" key={court.courtId}>
                      <div className="findCourt-courtImage">
                        <img src={court.image || image2} alt={`Court ${court.courtId}`} />
                      </div>
                      <div className="findCourt-courtInfo">
                        <h2>Court Name: {court.courtName}</h2>
                        <p>Branch: {branch ? branch.branchName : 'Unknown Branch'}</p>
                        <p>Address: {branch ? branch.location : 'Unknown Address'}</p>
                        <p>Price: {court.price}</p>
                        <p>Description: {court.description}</p>
                        <button className="findCourt-bookBtn" onClick={() => handleBook(court.courtId)}>Book</button>
                      </div>
                    </div>
                  );
                })}
              </div>

              <div className="findcourt-feedbackBox">
                <h1>User Feedback</h1>
                {loadingFeedback && <p>Loading feedback...</p>}
                {errorFeedback && <p>{errorFeedback}</p>}
                <div className="findcourt-feedbackGrid">
                  {filteredFeedback.map((fb) => {
                    const user = fb.user;
                    const token = sessionStorage.getItem('token');
                    const decodedToken = jwtDecode(token);
                    const userIdToken = decodedToken.UserId;

                    return (
                      <div key={fb.feedbackId} className="findcourt-feedbackCard">

                        <div className="findcourt-feedbackInfo">
                          <div className="findcourt-user-info">
                            <img src={user ? user.image || userImg : userImg} alt={`User ${user ? user.userName : 'Unknown User'}`} className="findcourt-user-image" />
                            <div className="findourt-user-details">
                              <p className="findcourt-user-name"><strong>{user ? user.firstName + " " + user.lastName : 'Anonymous'}</strong></p>
                              <p className="findcourt-user-rating"><span className="stars">{renderStars(fb.rating)}</span></p>
                            </div>
                          </div>
                          {selectedBranch && <p>Branch: {branches.find(branch => branch.branchId === fb.branchId)?.branchName}</p>}
                          <p>Feedback: {fb.content}</p>
                          <p className='feedback-datetime'>{fb.formattedDateTime}</p>

                        </div>
                        <div className="feedback-actions">
                          <button
                            className="find-court-delete-feedback-btn"
                            onClick={() => handleDelete(fb.feedbackId, fb.userId)}
                            disabled={userIdToken !== fb.userId}
                          >
                            Delete
                          </button>
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>
            </section>
          </div>
        </div>
      </div>
      <div className="findCourtFooter">
        <Footer />
      </div>
      <Modal
        title={<div className="findcourt-modal-title">Edit Feedback</div>}
        visible={isModalVisible}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
        footer={[
          <Button key="cancel" onClick={handleModalCancel} className="findcourt-modal-cancel-btn">
            Cancel
          </Button>,
          <Button key="ok" type="primary" onClick={handleModalOk} className="findcourt-modal-ok-btn">
            OK
          </Button>,
        ]}
        className="findcourt-modal"
      >
        <div className="findcourt-modal-content">
          <div className="findcourt-rating-container">
            <p>Rating: </p>
            <Rate onChange={setNewRating} value={newRating} />
          </div>
          <TextArea
            rows={4}
            onChange={(e) => setNewContent(e.target.value)}
            value={newContent}
          />
        </div>
      </Modal>
    </div>
  );
};

export default FindCourt;
