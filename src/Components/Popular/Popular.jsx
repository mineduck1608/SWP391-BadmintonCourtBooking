import React, { useState, useEffect } from 'react'
import './popular.css'
import { BsArrowLeftShort } from 'react-icons/bs'
import { BsArrowRightShort } from 'react-icons/bs'
import { BsDot } from 'react-icons/bs'
import { Link } from 'react-router-dom';



//Use high order array method to display all the destination




const Popular = () => {
    const [courtBranches, setCourtBranches] = useState([]);
    const token = sessionStorage.getItem('token');

    useEffect(() => {
        fetch("http://localhost:5266/Branch/GetAll", {
            method: "GET",
            headers: {
                'Authorization': `Bearer ${token}`,  // Attach token to Authorization header
                'Content-Type': 'application/json'
            }
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
                <Link to={'/findCourt'}>
                    <div className="mainContent grid" >
                        {courtBranches.map(branch => (
                            <div className="singleDestination" key={branch.branchId}>
                                <div className="destImage">
                                    <img src={branch.branchImg} alt="Image title" />
                                    <div className="overlayInfo black-text">
                                        <p>{branch.location}</p>
                                        <BsArrowRightShort className='icon' />
                                    </div>
                                </div>
                                <div className="destFooter black-text">
                                    <div className="number">{branch.branchName}</div>
                                    <div className="destText flex black-text">
                                        <h6>Location: {branch.location}</h6>
                                        <span className='flex'>
                                            <span className="dot">
                                                <BsDot className="icon" />
                                            </span>
                                        </span>
                                    </div>
                                    <div className="destText flex black-text">
                                        <h6>Phone: {branch.branchPhone}</h6>
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
                </Link>
            </div>
        </section>
    )
}

export default Popular