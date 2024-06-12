import React, { useEffect, useState } from 'react';
import Header from "../Header/header";
import './viewCourtInfo.css';
import Footer from "../Footer/Footer";
import image2 from '../../Assets/image2.jpg';

const ViewCourtInfo = () => {
    const [mainCourt, setMainCourt] = useState(null);
    const [recommendedCourts, setRecommendedCourts] = useState([]);
    const [branch, setBranch] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            const branchUrl = 'http://localhost:5266/Branch/GetAll';
            const courtUrl = 'http://localhost:5266/Court/GetAll';

            try {
                setLoading(true);
                const [branchResponse, courtResponse] = await Promise.all([
                    fetch(branchUrl),
                    fetch(courtUrl),
                ]);

                if (!branchResponse.ok) {
                    throw new Error(`Failed to fetch branch data: ${branchResponse.statusText}`);
                }
                if (!courtResponse.ok) {
                    throw new Error(`Failed to fetch court data: ${courtResponse.statusText}`);
                }

                const branchData = await branchResponse.json();
                const courtData = await courtResponse.json();

                console.log('Branch Data:', branchData);
                console.log('Court Data:', courtData);

                const mainCourtData = courtData[0];

                const mainBranchData = branchData.find(branch => branch.branchId === mainCourtData.branchId);

                const recommendedCourtsData = courtData.filter(court => court.branchId === mainCourtData.branchId && court.courtId !== mainCourtData.courtId).slice(0, 2);

                setBranch(mainBranchData);
                setMainCourt(mainCourtData);
                setRecommendedCourts(recommendedCourtsData);

            } catch (error) {
                setError(error.message);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    const handleBookCourt = (courtId) => {
        // Handle the booking logic here
        console.log(`Booking court with ID: ${courtId}`);
        alert(`Court No: ${courtId} booked successfully!`);
    };

    if (loading) return <div>Loading...</div>;
    if (error) return <div>Error: {error}</div>;

    return (
        <div className="viewcourtinfo">
            <Header />
            <div className="viewcourtinfo-body">
                <div className="viewcourtinfo-body-pic">
                    <img className="viewcourtinfo-img" src={image2} alt="" />
                    <div className="viewcourtinfo-body-des">
                        <h1>Description:</h1>
                        <p>{mainCourt.description}</p>
                    </div>
                </div>
                <div className="viewcourtinfo-info">
                    <h2>Court No: {mainCourt.courtId}</h2>
                    <p>Address: {branch.location}</p>
                    <p>Time: AAAAA</p>
                    <p>Branch: {branch.branchName}</p>
                    <p>Status: FREE</p>
                    <button onClick={() => handleBookCourt(mainCourt.courtId)}>Book</button>
                </div>
            </div>

            <div className='viewcourtinfo-othercourts'>
                <h1>OTHER COURTS</h1>
                {recommendedCourts.map((court, index) => (
                    <div key={index} className="viewcourtinfo-other-pic">
                        <img className="viewcourtinfo-other-img" src={image2} alt="" />
                        <div className="viewcourtinfo-other-info">
                            <h2>Court No: {court.courtId}</h2>
                            <p>Address: {branch.location}</p>
                            <p>Time: AAAAA</p>
                            <p>Branch: {branch.branchName}</p>
                            <p>Status: FREE</p>
                            <div className="viewcourtinfo-other-des">
                                <h1>Description:</h1>
                                <p>{court.description}</p>
                            </div>
                            <button onClick={() => handleBookCourt(court.courtId)}>Book</button>
                        </div>
                    </div>
                ))}
            </div>

            <Footer />
        </div>
    );
}

export default ViewCourtInfo;
