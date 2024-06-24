import React, { useEffect, useState } from 'react';
import Header from "../Header/header";
import './viewCourtInfo.css';
import Footer from "../Footer/Footer";
import image2 from '../../Assets/image2.jpg';
import { FaArrowLeft, FaArrowRight } from 'react-icons/fa';
import { format, addDays, subDays, startOfWeek } from 'date-fns';

const ViewCourtInfo = () => {
    const [mainCourt, setMainCourt] = useState(null);
    const [recommendedCourts, setRecommendedCourts] = useState([]);
    const [branch, setBranch] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [selectedDate, setSelectedDate] = useState(new Date());
    const [currentWeekStart, setCurrentWeekStart] = useState(startOfWeek(new Date()));
    const [currentHourIndex, setCurrentHourIndex] = useState(0);
    const [bookingData, setBookingData] = useState([]); // State cho dữ liệu booking
    const maxVisibleHours = 5; // Số khung giờ hiển thị tối đa

    useEffect(() => {
        const fetchInitialData = async () => {
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

        fetchInitialData();
    }, []);

    useEffect(() => {
        if (selectedDate) {
            fetchBookingData(selectedDate);
        }
    }, [selectedDate]);

    const fetchBookingData = async (date) => {
        const slotUrl = 'http://localhost:5266/Slot/GetAll';

        try {
            setLoading(true);
            const response = await fetch(slotUrl);

            if (!response.ok) {
                throw new Error(`Failed to fetch slot data: ${response.statusText}`);
            }

            const slotData = await response.json();

            // Filter slot data based on the selected date
            const filteredSlotData = slotData.filter(slot => {
                const slotStartDate = new Date(slot.startTime);
                return slotStartDate.toDateString() === date.toDateString();
            });

            setBookingData(filteredSlotData); // Update booking data
        } catch (error) {
            setError(error.message);
        } finally {
            setLoading(false);
        }
    };

    const handleDateClick = (date) => {
        setSelectedDate(date);
    };

    const handlePrevWeek = () => {
        setCurrentWeekStart(subDays(currentWeekStart, 7));
    };

    const handleNextWeek = () => {
        setCurrentWeekStart(addDays(currentWeekStart, 7));
    };

    const handlePrevHour = () => {
        setCurrentHourIndex((prev) => Math.max(prev - 1, 0));
    };

    const handleNextHour = () => {
        setCurrentHourIndex((prev) => Math.min(prev + 1, 24 - maxVisibleHours));
    };

    const handleBookCourt = (courtId) => {
        console.log(`Booking court with ID: ${courtId}`);
        alert(`Court No: ${courtId} booked successfully!`);
    };

    const generateWeekDates = (startOfWeek) => {
        const dates = [];
        for (let i = 0; i < 7; i++) {
            const date = addDays(startOfWeek, i);
            dates.push({
                day: format(date, 'EEE'),
                date: format(date, 'dd'),
                month: format(date, 'MMM'),
                fullDate: date,
            });
        }
        return dates;
    };

    const generateHourTimeline = (startHour) => {
        const hours = [];
        for (let i = startHour; i < startHour + maxVisibleHours; i++) {
            const hourStart = i % 24;
            const hourEnd = (i + 1) % 24;

            // Determine status for the time slot based on bookingData
            let status = 'available';
            const matchingSlot = bookingData.find(slot => {
                const slotStartTime = new Date(slot.startTime).getHours();
                const slotEndTime = new Date(slot.endTime).getHours();
                return slotStartTime <= hourStart && slotEndTime >= hourEnd;
            });

            if (matchingSlot) {
                // Assuming slot status is included in bookingData
                status = matchingSlot.bookingID ? 'booked' : 'available';
            }

            hours.push({
                start: hourStart.toString().padStart(2, '0') + ':00',
                end: hourEnd.toString().padStart(2, '0') + ':00',
                status: status,
            });
        }
        return hours;
    };

    const weekDates = generateWeekDates(currentWeekStart);
    const hours = generateHourTimeline(currentHourIndex);

    return (
        <div className="viewcourtinfo">
            <Header />
            <div className="viewCourtInfo-wrapper">
                <div className="background">
                    <div className="viewcourtinfo-body">
                        <div className="viewcourtinfo-body-pic">
                            <img className="viewcourtinfo-img" src={image2} alt="" />
                            <div className="viewcourtinfo-body-courtId-des">
                                <div className="viewcourtinfo-body-courtId">
                                    <h1>Court No: {mainCourt?.courtId}</h1>
                                </div>
                                <div className="viewcourtinfo-body-des">
                                    <h1 className='viewcourtinfo-des-h1'>Description:</h1>
                                    <p className='viewcourtinfo-des-p'>{mainCourt?.description}</p>
                                </div>
                                <div className='chooseTimeLine'>
                                    <div className="chooseDate">CHOOSE DATE</div>
                                    <div className="date-slider-wrapper">
                                        <button className="arrow-left" onClick={handlePrevWeek}>
                                            <FaArrowLeft />
                                        </button>
                                        <div className="date-slider">
                                            {weekDates.map((date, index) => (
                                                <div
                                                    key={index}
                                                    className={`date-item ${selectedDate && selectedDate.toDateString() === date.fullDate.toDateString() ? 'selected' : ''}`}
                                                    onClick={() => handleDateClick(date.fullDate)}
                                                >
                                                    <div>{date.day}</div>
                                                    <div className="line-separator"></div>
                                                    <div className="viewcourt-date">{date.date}</div>
                                                    <div className="viewcourt-month">{date.month}</div>
                                                </div>
                                            ))}
                                        </div>
                                        <button className="arrow-right" onClick={handleNextWeek}>
                                            <FaArrowRight />
                                        </button>
                                    </div>
                                    <div className="chooseTime">CHOOSE TIME</div>
                                    <div className="schedule-legend-wrapper">
                                        <div className="schedule">
                                            <div className="court">
                                                <button className="arrow-left-timeline" onClick={handlePrevHour}>
                                                    <FaArrowLeft />
                                                </button>
                                                <div className="court-timeline">
                                                    {hours.map((hour, index) => (
                                                        <div
                                                            key={index}
                                                            className={`time-slot ${hour.status}`}
                                                        >
                                                            {hour.start} - {hour.end}
                                                        </div>
                                                    ))}
                                                </div>
                                                <button className="arrow-right-timeline" onClick={handleNextHour}>
                                                    <FaArrowRight />
                                                </button>
                                            </div>
                                        </div>
                                        <div className="legend">
                                            <div className="legend-item">
                                                <div className="legend-color booked"></div>
                                                <div className="legend-text">Booked</div>
                                            </div>
                                            <div className="legend-item">
                                                <div className="legend-color available"></div>
                                                <div className="legend-text">Available</div>
                                            </div>
                                            <div className="legend-item">
                                                <div className="legend-color maintenance"></div>
                                                <div className="legend-text">Maintenance</div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="viewcourtinfo-info-status">
                            <div className="viewcourtinfo-info">
                                <p className='viewcourt-title'>Address: {branch?.location}</p>
                                <p className='viewcourt-title'>Time: 9:00 AM - 9:00 PM</p>
                                <p className='viewcourt-title'>Branch: {branch?.branchName}</p>
                                <p className='viewcourt-title'>Status: FREE</p>
                                <button className='viewCourt' onClick={() => handleBookCourt(mainCourt?.courtId)}>Book</button>
                            </div>
                        </div>
                    </div>
                    <div className="viewcourtinfo-othercourts">
                        <h1>OTHER COURTS</h1>
                        <div className="viewcourtinfo-othercourts-content">
                            {recommendedCourts.map((court, index) => (
                                <div key={index} className="viewcourtinfo-other-pic">
                                    <img className="viewcourtinfo-other-img" src={image2} alt="" />
                                    <div className="viewcourtinfo-other-info">
                                        <h2>Court No: {court.courtId}</h2>
                                        <p>Address: {branch?.location}</p>
                                        <p>Time: 9:00 AM - 9:00 PM</p>
                                        <p>Branch: {branch?.branchName}</p>
                                        <p>Status: FREE</p>
                                        <div className="viewcourtinfo-other-des">
                                            <h1>Description:</h1>
                                            <p>{court.description}</p>
                                        </div>
                                        <div className="other-court-button">
                                            <button className='viewCourt' onClick={() => handleBookCourt(court.courtId)}>Book</button>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            </div>
            <Footer />
        </div>
    );
}

export default ViewCourtInfo;
