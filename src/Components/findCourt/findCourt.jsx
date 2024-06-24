import React, { useState, useEffect } from 'react';
import './findcourt.css';
import { TimePicker } from 'antd';
import { useLocation } from 'react-router-dom';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import image2 from '../../Assets/image2.jpg';
import userImg from '../../Assets/user.jpg';

const { RangePicker } = TimePicker;

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

  const location = useLocation();

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

  const filteredCourts = courts.filter((court) => {
    return (
      (selectedBranch === '' || court.branchId === selectedBranch) &&
      (selectedCourt === '' || court.courtId === selectedCourt)
    );
  });

  useEffect(() => {
    const fetchData = async () => {
      const branchUrl = 'http://localhost:5266/Branch/GetAll';
      const courtUrl = 'http://localhost:5266/Court/GetAll';

      try {
        setLoading(true);
        const [branchResponse, courtResponse] = await Promise.all([
          fetch(branchUrl),
          fetch(courtUrl),
        ]);

        if (!branchResponse.ok) {
          throw new Error(`Failed to fetch branch data: ${branchResponse.statusText}`);
        }
        if (!courtResponse.ok) {
          throw new Error(`Failed to fetch court data: ${courtResponse.statusText}`);
        }

        const branchData = await branchResponse.json();
        const courtData = await courtResponse.json();

        const courtsWithImages = courtData.map(court => ({
          ...court,
          image: court.image || image2
        }));

        setBranches(branchData);
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
      const feedbackUrl = 'http://localhost:5266/Feedback/GetAll'; 
      const userUrl = 'http://localhost:5266/User/GetAll';

      setLoadingFeedback(true);
      try {
        const [feedbackResponse, userResponse] = await Promise.all([
          fetch(feedbackUrl),
          fetch(userUrl)
        ]);

        if (!feedbackResponse.ok) {
          throw new Error(`Failed to fetch feedback data: ${feedbackResponse.statusText}`);
        }
        if (!userResponse.ok) {
          throw new Error(`Failed to fetch user data: ${userResponse.statusText}`);
        }

        const feedbackData = await feedbackResponse.json();
        const userData = await userResponse.json();

        const feedbackWithUserDetails = feedbackData.map(fb => ({
          ...fb,
          user: userData.find(user => user.userId === fb.userId)
        }));

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
                      <option value="">Select Branch</option>
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
                      <option value="">Court Number</option>
                      {courts
                        .filter(court => !selectedBranch || court.branchId === selectedBranch)
                        .map((court) => (
                          <option key={court.courtId} value={court.courtId}>
                            {court.courtId}
                          </option>
                      ))}
                    </select>
                  </div>

                  <button className="Btn">
                    <a href="#">Search</a>
                  </button>
                </div>
              </div>

              <div className="findCourt-courtList">
                {loading && <p>Loading...</p>}
                {error && <p>{error}</p>}
                {filteredCourts.map((court) => (
                  <div className="findCourt-courtCard" key={court.courtId}>
                    <div className="findCourt-courtImage">
                      <img src={court.image} alt={`Court ${court.courtId}`} />
                    </div>
                    <div className="findCourt-courtInfo">
                      <h2>Court No: {court.courtId}</h2>
                      <p>Address: {court.address}</p>
                      <p>Time: {court.time}</p>
                      <p>Price: {court.price}</p>
                      <p>Description: {court.description}</p>
                      <p>Rating: {court.rating}/5</p>
                      <button className="findCourt-bookBtn">Book</button>
                    </div>
                  </div>
                ))}
              </div>

              <div className="findcourt-feedbackBox">
                <h1>User Feedback</h1>
                {loadingFeedback && <p>Loading feedback...</p>}
                {errorFeedback && <p>{errorFeedback}</p>}
                <div className="findcourt-feedbackGrid">
                  {feedback.map((fb) => {
                    const court = courts.find(court => court.courtId === fb.branchId);
                    const user = users.find(user => user.userId === fb.userId);
                    return (
                      <div key={fb.feedbackId} className="findcourt-feedbackCard">
                        <div className="findcourt-feedbackInfo">
                          <div className="findcourt-user-info">
                            <img src={user ? user.userImage || userImg : userImg} alt={`User ${user ? user.userName : 'Unknown User'}`} className="findcourt-user-image" />
                            <p><strong>{user ? user.userName : 'Anonymous'}</strong></p>
                          </div>
                          <p>{fb.content}</p>
                          <p>Rating: {fb.rate}/5</p>
                          {court && <p>Court No: {court.courtId}</p>}
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
    </div>
  );
};

export default FindCourt;
