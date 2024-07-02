import React, { useState, useEffect } from 'react';
import './popular.css';
import { BsArrowLeftShort, BsArrowRightShort } from 'react-icons/bs';
import { Link } from 'react-router-dom';

const Popular = ({ searchCriteria }) => {
    const [courtBranches, setCourtBranches] = useState([]);
    const [filteredBranches, setFilteredBranches] = useState([]);
    const token = sessionStorage.getItem('token');

    useEffect(() => {
        fetch("https://localhost:7233/Branch/GetAll", {
            method: "GET",
            headers: {
                'Authorization': `Bearer ${token}`,  
                'Content-Type': 'application/json'
            }
        })
            .then(response => response.json())
            .then((data) => {
                setCourtBranches(data);
                setFilteredBranches(data);
            })
            .catch(error => console.error('Error fetching branches:', error));
    }, [token]);

    useEffect(() => {
        if (searchCriteria && (searchCriteria.branch || searchCriteria.location)) {
            const filtered = courtBranches.filter(branch => 
                (searchCriteria.branch ? branch.branchName.includes(searchCriteria.branch) : true) &&
                (searchCriteria.location ? branch.location.includes(searchCriteria.location) : true)
            );
            setFilteredBranches(filtered);
        } else {
            setFilteredBranches(courtBranches);
        }
    }, [searchCriteria, courtBranches]);

    return (
        <section className='popular section container'>
            <div className="secContainer">
                <div className="secHeader flex">
                    <div className="textDiv">
                        <h2 className="secTitle">
                            Popular Badminton Branch.
                        </h2>
                        <p>
                            From historical cities to natural spectaculars, come see
                            the best of the world!
                        </p>
                    </div>
                    <div className="iconsDiv flex">
                        <BsArrowLeftShort className="icon leftIcon" />
                        <BsArrowRightShort className="icon" />
                    </div>
                </div>
                <div className="mainContent grid">
                    {filteredBranches.map(branch => (
                        <Link to={`/findCourt?branch=${branch.branchId}`} key={branch.branchId}>
                            <div className="singleDestination">
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
                                        <h6 className="popular-location">Location: {branch.location}</h6>
                                    </div>
                                    <div className="destText flex black-text">
                                        <h6 className="popular-phone">Phone: {branch.branchPhone}</h6>
                                    </div>
                                </div>
                            </div>
                        </Link>
                    ))}
                </div>
            </div>
        </section>
    );
};

export default Popular;
