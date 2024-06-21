import React, { useState, useEffect } from 'react';
import './findcourt.css';
import { TimePicker } from 'antd';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import image2 from '../../Assets/image2.jpg';

const { RangePicker } = TimePicker;

const FindCourt = () => {
  const [timeRange, setTimeRange] = useState([null, null]);
  const [courts, setCourts] = useState([]);
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [feedback, setFeedback] = useState([]);
  const [loadingFeedback, setLoadingFeedback] = useState(false);
  const [errorFeedback, setErrorFeedback] = useState('');
  const [expandedFeedback, setExpandedFeedback] = useState({});

  const onChange = (time) => {
    setTimeRange(time);
  };

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
      setLoadingFeedback(true);
      try {
        await new Promise((resolve) => setTimeout(resolve, 1000));
        const feedbackData = [
          { id: 1, firstName: 'Tran', lastName: 'Quynh', comment: 'dep', rating: 4, courtId: 1 },
          { id: 2, firstName: 'Quynh', lastName: 'Tran', comment: 'depp', rating: 5, courtId: 2 },
          
        ];
        setFeedback(feedbackData);
      } catch (error) {
        setErrorFeedback('Failed to load feedback');
      } finally {
        setLoadingFeedback(false);
      }
    };

    fetchFeedback();
  }, []);

  const toggleFeedbackExpansion = (id) => {
    setExpandedFeedback(prevState => ({
      ...prevState,
      [id]: !prevState[id]
    }));
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
                    <select required>
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
                        <select>
                          <option value="" disabled selected hidden>
                              Court Number
                          </option>
                          {courts.map((court) => (
                            <option key={court.courtId} value={court.courtId}>
                                {court.courtName}
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
                  {courts.map((court) => (
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
                  <h2>User Feedback</h2>
                  {loadingFeedback && <p>Loading feedback...</p>}
                  {errorFeedback && <p>{errorFeedback}</p>}
                  <div className="findcourt-feedbackGrid">
                    {feedback.map((fb) => {
                      const court = courts.find(court => court.courtId === fb.courtId);
                      return (
                        <div key={fb.id} className="findcourt-feedbackCard">
                          <div className="findcourt-feedbackImage">
                            <img src={court ? court.image : image2} alt={`Court ${fb.courtId}`} />
                          </div>
                          <div className="findcourt-feedbackInfo">
                            <h2>Court Id: {fb.courtId}</h2>
                            <p><strong>{fb.firstName} {fb.lastName}</strong>: {fb.comment}</p>
                            <p>Rating: {fb.rating}/5</p>
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
