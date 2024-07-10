import React, { useEffect, useState } from 'react';
import './home.css';  
import { Link } from 'react-router-dom';

const Home = ({ setSearchCriteria }) => {
    const [branches, setBranches] = useState([]);
    const [locations, setLocations] = useState([]);
    const [selectedBranch, setSelectedBranch] = useState('');
    const [selectedLocation, setSelectedLocation] = useState('');

    useEffect(() => {
        fetch('https://localhost:7233/Branch/GetAll')
            .then(response => response.json())
            .then((data) => {
                const parsedData = data
                .filter(branch => branch.branchStatus !== 0)  // Filter out branches with branchStatus of 0
                .map(branch => {
                    return {
                        ...branch,
                    };
                });
                setBranches(parsedData);
                setLocations(parsedData.map(branch => branch.location));
            })
            .catch(error => console.error('Error fetching branches:', error));
    }, []);

    const handleBranchChange = (e) => {
        setSelectedBranch(e.target.value);
        setSearchCriteria({ branch: e.target.value, location: selectedLocation });
    };

    const handleLocationChange = (e) => {
        setSelectedLocation(e.target.value);
        setSearchCriteria({ branch: selectedBranch, location: e.target.value });
    };

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
                        <Link to={'/findCourt'}>Book Now!</Link>
                    </button>
                </div>
                <div className="homeCard grid">
                    <div className="locationDiv">
                        <label htmlFor="branch">Branch Name</label>
                        <select 
                            id="branch" 
                            value={selectedBranch}
                            onChange={handleBranchChange}
                        >
                            <option value="">All Branch</option>
                            {branches.map(branch  => (
                                <option key={branch.id} value={branch.branchName}>
                                    {branch.branchName}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="distDiv">
                        <label htmlFor="location">Location</label>
                        <select 
                            id="location" 
                            value={selectedLocation}
                            onChange={handleLocationChange}
                        >
                            <option value="">All Location</option>
                            {locations.map((location, index) => (
                                <option key={index} value={location}>
                                    {location}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>
            </div>
        </section>
    );
};

export default Home;
