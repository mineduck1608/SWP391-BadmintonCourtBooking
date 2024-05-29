import React from 'react';
import './findcourt.css';
import {TimePicker} from 'antd';


const findCourt = () => {
    return (
        <section className='find'>
            <div className="secContainer container">

                <div className="homeText">
                    <h1 className='title'>
                        Find Your Court
                    </h1>
                </div>

                <div className="searchCard grid">
                    <div className="branchDiv">
                        <label htmlFor='branch'>Choose Branch</label>
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
                        <label htmlFor="court">Choose Court</label>
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
                        <label htmlFor="date">Choose Date</label><br />
                        <input type="date" />
                    </div>

                    <div className='custom-time-picker'>
                        <TimePicker.RangePicker/>
                    </div>


                </div>

            </div>

        </section>
    )
}

export default findCourt