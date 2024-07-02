import React, { useState, useEffect } from 'react';
import './findcourt.css';
import { TimePicker } from 'antd';
import { useLocation, useNavigate } from 'react-router-dom';
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

  useEffect(() => {
    const fetchData = async () => {
      const branchUrl = 'https://localhost:7233/Branch/GetAll';
      const courtUrl = 'https://localhost:7233/Court/GetAll';

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

        const courtsWithImages = courtData.map(court => {
          const imageUrl = court.courtImg?.[0]?.split(':')[1]?.trim(); // Extract the URL of Image 1
          return {
            ...court,
            image: imageUrl
          };
        });

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
      let token = sessionStorage.getItem('token');
      const feedbackUrl = 'https://localhost:7233/Feedback/GetAll';
      const userUrl = 'https://localhost:7233/User/GetAll';
      const userDetailsUrl = 'https://localhost:7233/UserDetail/GetAll';

      setLoadingFeedback(true);
      try {
        const [feedbackResponse, userResponse, userDetailsResponse] = await Promise.all([
          fetch(feedbackUrl),
          fetch(userUrl, {
            method: 'GET',
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          }),
          fetch(userDetailsUrl)
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
        const userData = await userResponse.json();
        const userDetailsData = await userDetailsResponse.json();

        const feedbackWithUserDetails = feedbackData.map(fb => {
          const user = userData.find(user => user.userId === fb.userId);
          const userDetails = userDetailsData.find(detail => detail.userId === fb.userId);
          return {
            ...fb,
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
                    return (
                      <div key={fb.feedbackId} className="findcourt-feedbackCard">
                        <div className="findcourt-feedbackInfo">
                          <div className="findcourt-user-info">
                            <img src={user ? user.image || userImg : userImg} alt={`User ${user ? user.userName : 'Unknown User'}`} className="findcourt-user-image" />
                            <p><strong>{user ? user.firstName + " " + user.lastName : 'Anonymous'}</strong></p>
                          </div>
                          <p>Rating: <span className="stars">{renderStars(fb.rating)}</span></p>
                          {selectedBranch && <p>Branch: {branches.find(branch => branch.branchId === fb.branchId)?.branchName}</p>}
                          <p>Feedback: {fb.content}</p>
                          
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
