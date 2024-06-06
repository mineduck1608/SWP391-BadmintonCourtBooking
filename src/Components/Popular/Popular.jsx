import React, { useState, useEffect } from 'react'
import './popular.css'
import { BsArrowLeftShort } from 'react-icons/bs'
import { BsArrowRightShort } from 'react-icons/bs'
import { BsDot } from 'react-icons/bs'

//Import the images====================>
import img2 from '../../Assets/image1.jpg'
import img5 from '../../Assets/image2.jpg'
import img7 from '../../Assets/image3.jpg'
import img9 from '../../Assets/image4.jpg'


//Use high order array method to display all the destination




const Popular = () => {
    const [courtBranches, setCourtBranches] = useState([]);

    useEffect(() => {
        fetch("http://localhost:5266/Branch/GetAll", {
            method: "GET",
            headers: { 'Content-Type': 'application/json' },
        })
            .then(response => response.json())
            .then((data) => {
                setCourtBranches(data);
            })
            .catch(error => console.error('Error fetching user info:', error));
    }, []);

    return (
        <section className='popular section container'>
            <div className="secContainer">

                <div className="secHeader flex">
                    <div className="textDiv">
                        <h2 className="secTitle">
                            Popular Badminton Branch.
                        </h2>
                        <p>
                            From historical cities to natural specteculars, come see
                            the best of the world!
                        </p>
                    </div>

                    <div className="iconsDiv flex">
                        <BsArrowLeftShort className="icon leftIcon" />
                        <BsArrowRightShort className="icon" />
                    </div>
                </div>

                <div className="mainContent grid">
                    {courtBranches.map(branch => (
                        <div className="singleDestination" key={branch.branchId}>
                            <div className="destImage">
                                <img src={img2} alt="Image title" />
                                <div className="overlayInfo">
                                    <p>{branch.location}</p>
                                    <BsArrowRightShort className='icon' />
                                </div>
                            </div>
                            <div className="destFooter">
                                <div className="number">0{branch.branchId}</div>
                                <div className="destText flex">
                                    <h6>{branch.location}</h6>
                                    <span className='flex'>
                                        <span className="dot">
                                            <BsDot className="icon" />
                                        </span>
                                    </span>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>

            </div>
        </section>
    )
}

export default Popular