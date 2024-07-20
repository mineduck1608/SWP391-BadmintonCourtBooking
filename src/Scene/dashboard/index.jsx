import React, { useEffect, useState } from "react";
import { Box, Button, IconButton, Typography, useTheme, TextField, MenuItem, Select, FormControl, InputLabel } from "@mui/material";
import { tokens } from "../../theme";
import DownloadOutlinedIcon from "@mui/icons-material/DownloadOutlined";
import Head from "../../Components/Head";
import LineChart from "../../Components/LineChart";
import BarChart from "../../Components/BarChart";
import PieChart from '../../Components/PieChart';
import './dashboard.css';
import { fetchWithAuth } from "../../Components/fetchWithAuth/fetchWithAuth";

const Dashboard = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);

  const currentYear = new Date().getFullYear();

  const [payments, setPayments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [totalRevenue, setTotalRevenue] = useState(0);

  const [filterType, setFilterType] = useState("year");
  const [year, setYear] = useState(currentYear);
  const [startMonth, setStartMonth] = useState(1);
  const [numberOfMonths, setNumberOfMonths] = useState(1);
  const [weekOfMonth, setWeekOfMonth] = useState(1);

  const [paymentStatistics, setPaymentStatistics] = useState([]);
  const [branchAmounts, setBranchAmounts] = useState([]);

  useEffect(() => {
    const fetchAllData = async () => {
      try {
        setLoading(true);

        const branches = await fetchData('https://localhost:7233/Branch/GetAll');
        const courts = await fetchData('https://localhost:7233/Court/GetAll');
        const slots = await fetchData('https://localhost:7233/Slot/GetAll');
        const payments = await fetchData('https://localhost:7233/Payment/GetAll');
        
        // Process data for BarChart and PieChart
        const branchAmountsData = branches.map(branch => {
          const branchCourts = courts.filter(court => court.branchId === branch.branchId);
          const branchSlots = slots.filter(slot => 
            branchCourts.some(court => court.courtId === slot.courtId)
          );
          const branchPayments = payments.filter(payment => 
            branchSlots.some(slot => slot.bookingId === payment.bookingId)
          );
          const totalAmount = branchPayments.reduce((sum, payment) => sum + payment.amount, 0);
          return { 
            id: branch.branchName,
            label: branch.branchName,
            value: totalAmount,
            branchName: branch.branchName,
            totalAmount: totalAmount
          };
        });
        
        setBranchAmounts(branchAmountsData);

        // Calculate the total revenue
        const total = payments.reduce((sum, payment) => sum + payment.amount, 0);
        setTotalRevenue(total);

        // Fetch initial statistics
        const statisticsResponse = await fetchWithAuth(`https://localhost:7233/Payment/Statistic?Year=${currentYear}&Type=1&StartMonth=1&MonthNum=1&Week=1`, {
          method: 'GET'
        });
        const statisticsData = await statisticsResponse.json();
        setPaymentStatistics(statisticsData);

      } catch (error) {
        console.error("Error fetching data", error);
      } finally {
        setLoading(false);
      }
    };

    fetchAllData();
  }, []);

  const handleFilterChange = (event) => {
    setFilterType(event.target.value);
  };

  const handleFetchStatistics = async () => {
    try {
      const response = await fetchWithAuth(`https://localhost:7233/Payment/Statistic?Year=${year}&Type=${filterType === 'year' ? 1 : filterType === 'month' ? 2 : 3}&StartMonth=${startMonth}&MonthNum=${numberOfMonths}&Week=${weekOfMonth}`, {
        method: 'GET'
      });
      const data = await response.json();
      console.log('Payment Statistics:', data);
      setPaymentStatistics(data);

      // Calculate the total revenue from the fetched statistics
      const total = data.reduce((sum, payment) => sum + payment.amount, 0);
      setTotalRevenue(total);
    } catch (error) {
      console.error("Error fetching statistics", error);
    }
  };

  const fetchData = async (url, options = {}) => {
    const response = await fetchWithAuth(url, options);
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    return await response.json();
  };

  const filteredBranchAmounts = branchAmounts.filter(branch => branch.value > 0);

  return (
    <Box m="20px">
      {/* HEADER */}
      <Box display="flex" justifyContent="space-between" alignItems="center">
        <Head 
          title="DASHBOARD" 
          subtitle="Welcome to your dashboard" 
        />
      </Box>

      {/* GRID & CHARTS */}
      <Box
        display="grid"
        gridTemplateColumns="repeat(12, 1fr)"
        gridAutoRows="150px"
        gap="17px"
      >
        {/* FILTER ROW */}
        <Box
          gridColumn="span 12"
          className="filter-container"
        >
          <FormControl className="filter-input" fullWidth>
            <InputLabel>Filter Type</InputLabel>
            <Select
              value={filterType}
              onChange={handleFilterChange}
              label="Filter Type"
            >
              <MenuItem value="year">Year</MenuItem>
              <MenuItem value="month">Month</MenuItem>
              <MenuItem value="week">Week</MenuItem>
            </Select>
          </FormControl>
          <TextField
            label="Year"
            type="number"
            value={year}
            onChange={(e) => setYear(e.target.value)}
            className="filter-input"
            fullWidth
            margin="normal"
          />
          {filterType === "month" || filterType === "week" ? (
            <FormControl className="filter-input" fullWidth>
              <InputLabel>Start Month</InputLabel>
              <Select
                value={startMonth}
                onChange={(e) => setStartMonth(e.target.value)}
                label="Start Month"
              >
                {Array.from({ length: 12 }, (_, i) => i + 1).map(month => (
                  <MenuItem key={month} value={month}>{month}</MenuItem>
                ))}
              </Select>
            </FormControl>
          ) : null}
          {filterType === "month" ? (
            <FormControl className="filter-input" fullWidth>
            <InputLabel>Number of Months</InputLabel>
            <Select
              value={numberOfMonths}
              onChange={(e) => setNumberOfMonths(e.target.value)}
              label="Number of Months" 
            >
              {Array.from({ length: 3 }, (_, i) => i + 1).map(numberOfMonths => (
                <MenuItem key={numberOfMonths} value={numberOfMonths}>{numberOfMonths}</MenuItem>
              ))}
            </Select>
          </FormControl>
          ) : null}
          {filterType === "week" ? (
            <FormControl className="filter-input" fullWidth>
              <InputLabel>Week of Month</InputLabel>
              <Select
                value={weekOfMonth}
                onChange={(e) => setWeekOfMonth(e.target.value)}
                label="Week of Month"
              >
                {Array.from({ length: 4 }, (_, i) => i + 1).map(week => (
                  <MenuItem key={week} value={week}>{week}</MenuItem>
                ))}
              </Select>
            </FormControl>
          ) : null}
          <Button
            variant="contained"
            color="primary"
            onClick={handleFetchStatistics}
            sx={{ mt: 2 }}
          >
            Fetch Payment Data
          </Button>
        </Box>

        {/* CHARTS */}
        {loading ? (
          <Box gridColumn="span 12" display="flex" justifyContent="center" alignItems="center" height="100%">
            <Typography variant="h6" color={colors.grey[100]}>
              Loading...
            </Typography>
          </Box>
        ) : (
          <>
            <Box
              gridColumn="span 6"
              gridRow="span 2"
              backgroundColor={colors.primary[400]}
            >
              <Box
                mt="25px"
                p="0 30px"
                display="flex "
                justifyContent="space-between"
                alignItems="center"
              >
                <Box>
                  <Typography
                    variant="h5"
                    fontWeight="600"
                    color={colors.grey[100]}
                  >
                    Revenue Generated
                  </Typography>
                  <Typography
                    variant="h3"
                    fontWeight="bold"
                    color={colors.greenAccent[500]}
                  >
                    {totalRevenue.toLocaleString()} VND
                  </Typography>
                </Box>
                <Box>
                  <IconButton>
                    <DownloadOutlinedIcon
                      sx={{ fontSize: "26px", color: colors.greenAccent[500] }}
                    />
                  </IconButton>
                </Box>
              </Box>
              <Box height="250px" m="-20px 0 0 0">
                <LineChart data={paymentStatistics} isDashboard={true} />
              </Box>
            </Box>
            
            <Box
              gridColumn="span 6"
              gridRow="span 2"
              backgroundColor={colors.primary[400]}
            >
              <Typography
                variant="h5"
                fontWeight="600"
                sx={{ padding: "30px 30px 0 30px" }}
              >
                Revenue Comparison
              </Typography>
              <Box height="250px" mt="-20px">
                <PieChart data={filteredBranchAmounts} />
              </Box>
            </Box>
            
            <Box
              gridColumn="span 12"
              gridRow="span 2"
              backgroundColor={colors.primary[400]}
            >
              <Typography
                variant="h5"
                fontWeight="600"
                sx={{ padding: "30px 30px 0 30px" }}
              >
                Sales Quantity
              </Typography>
              <Box height="250px" mt="-20px">
                <BarChart isDashboard={true} data={filteredBranchAmounts} />
              </Box>
            </Box>
          </>
        )}
      </Box>
    </Box>
  );
};

export default Dashboard;
