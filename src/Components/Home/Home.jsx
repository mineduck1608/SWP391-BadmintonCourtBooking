import React from 'react'
import './home.css'  

const Home = () => {
    return (
        <section className='home'>
            <div className="secContainer container">

                <div className="homeText">
                    <h1 className="title">
                        Book Your Court With Badminton Dot
                    </h1>
                    <p className="subTitle">
                    Experience the pinnacle of badminton amidst the finest court available.
                    </p>

                    <button className='btn'>
                        <a href="#">Book Now!</a>
                    </button>
                </div>

                <div className="homeCard grid">

                    <div className="locationDiv">
                        <label htmlFor="location">Location</label>
                        <input type="text" placeholder='Choose court branch' />
                    </div>


                    <div className="distDiv">
                        <label htmlFor="distance">Date</label>
                        <input type="text" placeholder='Date' />
                    </div>


                    <div className="priceDiv">
                        <label htmlFor="price">Time</label>
                        <input type="text" placeholder='Time Slots' />
                    </div>
                    <button className='btn'>
                        Search
                    </button>

                </div>

            </div>
        </section>
    )
}

export default Home