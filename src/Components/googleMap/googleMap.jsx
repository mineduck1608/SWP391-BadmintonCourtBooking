import React, { useState } from 'react';
import Header from '../Header/header';
import Footer from '../Footer/Footer';
import { Typography, Button } from '@mui/material';
import { Phone, LocationOn, CheckCircle, Cancel } from '@mui/icons-material';
import './GoogleMap.css'; // Import the CSS file
import { GrMapLocation } from "react-icons/gr";
import { BsFillInfoCircleFill } from "react-icons/bs";
import { BsPhoneVibrate } from "react-icons/bs";

const branches = [
    {
        name: "Branch 1",
        address: "Address 1",
        phone: "123-456-7890",
        status: "Open",
        image: "https://via.placeholder.com/150",
        mapLink: "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3918.60994153052!2d106.80730807583869!3d10.841132857995182!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752731176b07b1%3A0xb752b24b379bae5e!2sFPT%20University%20HCMC!5e0!3m2!1sen!2s!4v1719730961649!5m2!1sen!2s"
    },
    {
        name: "Branch 2",
        address: "Address 2",
        phone: "987-654-3210",
        status: "Closed",
        image: "https://via.placeholder.com/150",
        mapLink: "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.5972868458975!2d105.52829057585723!3d21.00706886082982!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x313452f3229d341f%3A0x25df1a1bbf46a947!2sFPT%20University%20Hanoi!5e0!3m2!1sen!2s!4v1719730961649!5m2!1sen!2s"
    },
    {
        name: "Branch 3",
        address: "Address 3",
        phone: "555-123-4567",
        status: "Open",
        image: "https://via.placeholder.com/150",
        mapLink: "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.5972868458975!2d105.52829057585723!3d21.00706886082982!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x313452f3229d341f%3A0x25df1a1bbf46a947!2sFPT%20University%20Hanoi!5e0!3m2!1sen!2s!4v1719730961649!5m2!1sen!2s"
    },
    {
        name: "Branch 4",
        address: "Address 4",
        phone: "444-555-6666",
        status: "Open",
        image: "https://via.placeholder.com/150",
        mapLink: "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.5972868458975!2d105.52829057585723!3d21.00706886082982!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x313452f3229d341f%3A0x25df1a1bbf46a947!2sFPT%20University%20Hanoi!5e0!3m2!1sen!2s!4v1719730961649!5m2!1sen!2s"
    }
];

const GoogleMap = () => {
    const [selectedBranch, setSelectedBranch] = useState(branches[0]);

    return (
        <>
            <Header />
            <div className="googlemap-container">
                <Typography variant="h4" gutterBottom className="googlemap-title">
                    Badminton Court Booking System
                </Typography>
                <div className="googlemap-branch-grid">
                    {branches.map((branch, index) => (
                        <div className="googlemap-branch-card" key={index}>
                            <img src={branch.image} alt={branch.name} className="googlemap-branch-image" />
                            <div className="googlemap-branch-content">
                                <Typography gutterBottom variant="h5" component="div">
                                    {branch.name}
                                </Typography>
                                <Typography variant="body2" color="textSecondary" component="p">
                                    <LocationOn style={{ color: 'orange' }} /> <span>{branch.address}</span>
                                </Typography>
                                <Typography variant="body2" color="textSecondary" component="p">
                                    <Phone style={{ color: 'darkred' }} /> <span>{branch.phone}</span>
                                </Typography>
                                <Typography variant="body2" color="textSecondary" component="p">
                                    {branch.status === "Open" ? (
                                        <CheckCircle className="googlemap-status-icon open" />
                                    ) : (
                                        <Cancel className="googlemap-status-icon closed" />
                                    )}{" "}
                                    <span>{branch.status}</span>
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
                <div className='googlemap-position'>
                    <iframe
                        src={selectedBranch.mapLink}
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
                                <p>{selectedBranch.name}</p>
                            </div>
                            <div className='googlemap-location'>
                                <GrMapLocation style={{ color: 'orange', marginLeft: '5px', fontSize: '50px' }} />
                                <h3>Location</h3>
                                <p>{selectedBranch.address}</p>
                            </div>
                            <div className='googlemap-phone'>
                                <BsPhoneVibrate style={{ color: 'darkred', marginLeft: '5px', fontSize: '50px' }} />
                                <h3>Phone</h3>
                                <p>{selectedBranch.phone}</p>
                            </div>
                        </div>
                        <div className='googlemap-contacts'>
                            Information
                        </div>
                        <div className='googlemap-description'>
                            sssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss
                        </div>
                    </div>
                </div>
            </div>
            <Footer />
        </>
    );
};

export default GoogleMap;
