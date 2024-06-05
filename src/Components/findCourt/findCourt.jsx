import React, { useState } from 'react';
import './findcourt.css';
import { TimePicker } from 'antd';

const { RangePicker } = TimePicker; // Import RangePicker


const FindCourt = () => {
      const [timeRange, setTimeRange] = useState([null, null]);

      const onChange = (time) => {
            setTimeRange(time);
      };

      return (
            <div>
                  <section className='find'>
                        <div className="secContainer container">

                              <div className="homeText">
                                    <h1 className='Title'>
                                          Find Your Court
                                    </h1>
                              </div>

                              <div className="searchCard grid">
                                    <div className="branchDiv">
                                          <label htmlFor='branch'>Branch</label>
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
                                          <select >
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

                                    <div className='custom-time-picker'>
                                          <label htmlFor="date">Time Range</label>
                                          <TimePicker.RangePicker
                                                popupClassName='custom-time-picker-dropdown'
                                                getPopupContainer={trigger => trigger.parentNode}
                                          />
                                    </div>

                                    <button className='Btn'>
                                          <a href="#">Search</a>
                                    </button>

                              </div>

                        </div>

                  </section>
                  <h1>sss</h1>
            </div>
      );
};

export default FindCourt