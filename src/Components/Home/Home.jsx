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
            .then(data => {
                setBranches(data);
                setLocations(data.map(branch => branch.location));
            })
            .catch(error => console.error('Error fetching branches:', error));
    }, []);

    const handleSearch = () => {
        setSearchCriteria({ branch: selectedBranch, location: selectedLocation });
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
                            onChange={(e) => setSelectedBranch(e.target.value)}
                        >
                            <option value="">Choose court branch</option>
                            {branches.map(branch => (
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
                            onChange={(e) => setSelectedLocation(e.target.value)}
                        >
                            <option value="">Choose location</option>
                            {locations.map((location, index) => (
                                <option key={index} value={location}>
                                    {location}
                                </option>
                            ))}
                        </select>
                    </div>
                    <button className='btn' onClick={handleSearch}>
                        Search
                    </button>
                </div>
            </div>
        </section>
    );
};

export default Home;
