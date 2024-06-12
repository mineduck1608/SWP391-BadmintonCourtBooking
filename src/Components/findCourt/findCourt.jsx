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

  return (
    <div className="findCourt">
      <div className="findCourtHeader">
        <Header />
      </div>
      <div className="findCourt-wrapper">
        <div className="background">
          <div>
            <section className="find">
              <div className="secContainer container">
                <div className="homeText">
                  <h1 className="Title">Find a Court</h1>
                </div>

                <div className="searchCard grid">
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

                  <div className="courtDiv">
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

                  <div className="dateDiv">
                    <label htmlFor="date">Date</label>
                    <input type="date" />
                  </div>

                  <div className="custom-time-picker">
                    <label htmlFor="timeRange">Time Range</label>
                    <RangePicker
                      popupClassName="custom-time-picker-dropdown"
                      getPopupContainer={(trigger) => trigger.parentNode}
                      onChange={onChange}
                    />
                  </div>

                  <button className="Btn">
                    <a href="#">Search</a>
                  </button>
                </div>

                <div className="courtList">
                  {loading && <p>Loading...</p>}
                  {error && <p>{error}</p>}
                  {courts.map((court) => (
                    <div className="courtCard" key={court.courtId}>
                      <div className="courtImage">
                        <img src={court.image} alt={court.courtName} />
                      </div>
                      <div className="courtInfo">
                        <h2>{court.courtName}</h2>
                        <p>Address: {court.address}</p>
                        <p>Time: {court.time}</p>
                        <p>Price: {court.price}</p>
                        <p>Description: {court.description}</p>
                        <p>Rating: {court.rating}/5</p>
                        <button className="bookBtn">Book</button>
                      </div>
                    </div>
                  ))}
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
