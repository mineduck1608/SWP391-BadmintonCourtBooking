import React, { useState } from 'react';
import './findcourt.css';
import { TimePicker } from 'antd';
import Header from '../Header/header';

const { RangePicker } = TimePicker; // Import RangePicker

const FindCourt = () => {
  const [timeRange, setTimeRange] = useState([null, null]);

  const onChange = (time) => {
    setTimeRange(time);
  };

  // Dummy data for courts
  const courtList = [
    {
      id: 1,
      name: 'Court 1',
      address: '123 Court St.',
      time: '10:00 - 11:00',
      price: '$50',
      description: 'A great place to play.',
      rating: 5,
      image: 'path_to_image1',
    },
    {
      id: 2,
      name: 'Court 2',
      address: '456 Court Ave.',
      time: '12:00 - 13:00',
      price: '$40',
      description: 'Perfect for all skill levels.',
      rating: 4,
      image: 'path_to_image2'
    }
  ];

  return (
    <div className="findCourt">
      <div className="findCourtHeader">
        <Header />
      </div>
      <div>
        <section className="find">
          <div className="secContainer container">
            <div className="homeText">
              <h1 className="Title"></h1>
            </div>

            <div className="searchCard grid">
              <div className="branchDiv">
                <label htmlFor="branch">Branch</label>
                <select required>
                  <option value="0">Branch</option>
                  <option value="1">San Cau Kho</option>
                  <option value="2">San Lao Dong</option>
                  <option value="3">San Bo Cong An</option>
                  <option value="4">San Hien Hoa</option>
                  <option value="5">San Tran Nao</option>
                  <option value="6">San Dong Phuong</option>
                  <option value="7">San Luong Dinh Cua</option>
                </select>
              </div>

              <div className="courtDiv">
                <label htmlFor="court">Court</label>
                <select>
                  <option value="0" disabled selected hidden>Court Number</option>
                  <option value="1">Court 1</option>
                  <option value="2">Court 2</option>
                  <option value="3">Court 3</option>
                  <option value="4">Court 4</option>
                  <option value="5">Court 5</option>
                  <option value="6">Court 6</option>
                  <option value="7">Court 7</option>
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
                  getPopupContainer={trigger => trigger.parentNode}
                  onChange={onChange}
                />
              </div>

              <button className="Btn">
                <a href="#">Search</a>
              </button>
            </div>

            <div className="courtList">
              {courtList.map((court) => (
                <div className="courtCard" key={court.id}>
                  <div className="courtImage">
                    <img src={court.image} alt={court.name} />
                  </div>
                  <div className="courtInfo">
                    <h2>{court.name}</h2>
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
  );
};

export default FindCourt;
