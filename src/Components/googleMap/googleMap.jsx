import React, { useState, useEffect } from 'react';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import { Typography, Button } from '@mui/material';
import { Phone, LocationOn, CheckCircle, Cancel } from '@mui/icons-material';
import './GoogleMap.css'; // Import the CSS file
import { GrMapLocation } from "react-icons/gr";
import { BsFillInfoCircleFill, BsPhoneVibrate } from "react-icons/bs";

const ITEMS_PER_PAGE = 4;

const GoogleMap = () => {
    const [branches, setBranches] = useState([]);
    const [selectedBranch, setSelectedBranch] = useState(null);
    const [currentPage, setCurrentPage] = useState(0);

    useEffect(() => {
        fetch('https://localhost:7233/Branch/GetAll')
            .then(response => response.json())
            .then(data => {
                const parsedData = data
                    .filter(branch => branch.branchStatus !== 0) // Filter out branches with branchStatus of 0
                    .map(branch => {
                        let imgURL = '';
                        if (branch.branchImg && branch.branchImg.length > 0) {
                            const urlString = branch.branchImg[0];
                            const parts = urlString.split(': ');
                            if (parts.length > 1) {
                                imgURL = parts[1].trim();
                            }
                        }
                        return {
                            ...branch,
                            branchImg: imgURL
                        };
                    });
                setBranches(parsedData);
                setSelectedBranch(parsedData[0]); // Select the first branch by default
            })
            .catch(error => console.error('Error fetching branches:', error));
    }, []);

    const handleNextPage = () => {
        setCurrentPage((prevPage) => Math.min(prevPage + 1, Math.ceil(branches.length / ITEMS_PER_PAGE) - 1));
    };

    const handlePrevPage = () => {
        setCurrentPage((prevPage) => Math.max(prevPage - 1, 0));
    };

    if (!selectedBranch) {
        return <div>Loading...</div>;
    }

    const displayedBranches = branches.slice(currentPage * ITEMS_PER_PAGE, (currentPage + 1) * ITEMS_PER_PAGE);

    return (
        <>
            <div className="googlemap-container">
                <Typography variant="h4" gutterBottom className="googlemap-title">
                    Badminton Court Booking System
                </Typography>
                <div className="googlemap-branch-grid">
                    {displayedBranches.map((branch, index) => (
                        <div className="googlemap-branch-card" key={index}>
                            <img 
                                src={branch.branchImg} 
                                alt={branch.branchName} 
                                className="googlemap-branch-image" 
                            />
                            <div className="googlemap-branch-content">
                                <Typography gutterBottom variant="h5" component="div">
                                    {branch.branchName}
                                </Typography>
                                <Typography variant="body2" color="textSecondary" component="p">
                                    <LocationOn style={{ color: 'orange' }} /> <span>{branch.location}</span>
                                </Typography>
                                <Typography variant="body2" color="textSecondary" component="p">
                                    <Phone style={{ color: 'darkred' }} /> <span>{branch.branchPhone}</span>
                                </Typography>
                                <Typography variant="body2" color="textSecondary" component="p">
                                    {branch.branchStatus === 1 ? (
                                        <CheckCircle className="googlemap-status-icon open" />
                                    ) : (
                                        <Cancel className="googlemap-status-icon closed" />
                                    )}{" "}
                                    <span>{branch.branchStatus === 1 ? 'Open' : 'Closed'}</span>
                                </Typography>
                                <Button
                                    variant="contained"
                                    color="primary"
                                    onClick={() => setSelectedBranch(branch)}
                                >
                                    View on Map
                                </Button>
                            </div>
                        </div>
                    ))}
                </div>
                <div className="googlemap-pagination-container">
                    <Button variant="contained" onClick={handlePrevPage} disabled={currentPage === 0}>
                        Previous
                    </Button>
                    <Button variant="contained" onClick={handleNextPage} disabled={(currentPage + 1) * ITEMS_PER_PAGE >= branches.length}>
                        Next
                    </Button>
                </div>
                <div className='googlemap-position'>
                    <iframe
                        src={selectedBranch.mapUrl}
                        width="600"
                        height="450"
                        style={{ border: 0, marginTop: '20px' }}
                        allowFullScreen
                        loading="lazy"
                        referrerPolicy="no-referrer-when-downgrade"
                    ></iframe>
                    <div className='googlemap-right'>
                        <div className='googlemap-contacts'>
                            Contacts
                        </div>
                        <div className='googlemap-flex'>
                            <div className='googlemap-name'>
                                <BsFillInfoCircleFill style={{ color: 'blue', marginLeft: '5px', fontSize: '50px' }} />
                                <h3>Name</h3>
                                <p>{selectedBranch.branchName}</p>
                            </div>
                            <div className='googlemap-location'>
                                <GrMapLocation style={{ color: 'orange', marginLeft: '5px', fontSize: '50px' }} />
                                <h3>Location</h3>
                                <p>{selectedBranch.location}</p>
                            </div>
                            <div className='googlemap-phone'>
                                <BsPhoneVibrate style={{ color: 'darkred', marginLeft: '5px', fontSize: '50px' }} />
                                <h3>Phone</h3>
                                <p>{selectedBranch.branchPhone}</p>
                            </div>
                        </div>
                        <div className='googlemap-information'>
                            Information
                        </div>
                        <div className='googlemap-description'>
                            <p>Welcome to the Badminton Court Booking System, your efficient solution for reserving badminton courts. Our user-friendly platform allows you to easily schedule games, check real-time availability, and access detailed branch information. Designed for both avid players and casual enthusiasts, our system ensures a seamless booking experience. Thank you for choosing the Badminton Court Booking System. Enjoy your game!</p>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
};

export default GoogleMap;
