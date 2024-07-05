import React, { useEffect, useState } from "react";
import axios from 'axios';
import { Box, Button, IconButton, Typography, useTheme, TextField, MenuItem, Select, FormControl, InputLabel } from "@mui/material";
import { tokens } from "../../theme";
import DownloadOutlinedIcon from "@mui/icons-material/DownloadOutlined";
import Head from "../../Components/Head";
import LineChart from "../../Components/LineChart";
import BarChart from "../../Components/BarChart";
import './dashboard.css';

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

  useEffect(() => {
    const fetchInitialData = async () => {
      try {
        setLoading(true);
        const token = sessionStorage.getItem('token'); // Retrieve the token from sessionStorage

        const paymentResponse = await axios.get('https://localhost:7233/Payment/GetAll', {
          headers: { 'Authorization': `Bearer ${token}` }
        });
        
        setPayments(paymentResponse.data);

        // Calculate the total revenue
        const total = paymentResponse.data.reduce((sum, payment) => sum + payment.amount, 0);
        setTotalRevenue(total);

        // Fetch initial statistics
        const response = await axios.get(`https://localhost:7233/Payment/Statistic?Year=${currentYear}&Type=1&StartMonth=1&MonthNum=1&Week=1`, {
          headers: { 'Authorization': `Bearer ${token}` }
        });
        setPaymentStatistics(response.data);

        // Calculate the total revenue from the fetched statistics
        const initialTotal = response.data.reduce((sum, payment) => sum + payment.amount, 0);
        setTotalRevenue(initialTotal);

      } catch (error) {
        console.error("Error fetching data", error);
      } finally {
        setLoading(false);
      }
    };

    fetchInitialData();
  }, []);

  const handleFilterChange = (event) => {
    setFilterType(event.target.value);
  };

  const handleFetchStatistics = async () => {
    try {
      const token = sessionStorage.getItem('token'); // Retrieve the token from sessionStorage
      const response = await axios.get(`https://localhost:7233/Payment/Statistic?Year=${year}&Type=${filterType === 'year' ? 1 : filterType === 'month' ? 2 : 3}&StartMonth=${startMonth}&MonthNum=${numberOfMonths}&Week=${weekOfMonth}`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      console.log('Payment Statistics:', response.data);
      setPaymentStatistics(response.data);

      // Calculate the total revenue from the fetched statistics
      const total = response.data.reduce((sum, payment) => sum + payment.amount, 0);
      setTotalRevenue(total);
    } catch (error) {
      console.error("Error fetching statistics", error);
    }
  };

  return (
    <Box m="20px">
      {/* HEADER */}
      <Box display="flex" justifyContent="space-between" alignItems="center">
        <Head 
          title="DASHBOARD" 
          subtitle="Welcome to your dashboard" 
        />

        <Box>
          <Button className="download-button"
            sx={{
              backgroundColor: colors.blueAccent[700],
              color: colors.grey[100],
            }}
          >
            <DownloadOutlinedIcon sx={{ mr: "10px" }} />
            Download Reports
          </Button>
        </Box>
      </Box>

      {/* GRID & CHARTS */}
      <Box
        display="grid"
        gridTemplateColumns="repeat(12, 1fr)"
        gridAutoRows="140px"
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
            <TextField
              label="Number of Months"
              type="number"
              value={numberOfMonths}
              onChange={(e) => setNumberOfMonths(e.target.value)}
              className="filter-input"
              fullWidth
              margin="normal"
            />
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

        {/* ROW 2 */}
        <Box
          gridColumn="span 12"
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
                ${totalRevenue.toLocaleString()}
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
        
        {/* ROW 3 */}
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
            <BarChart isDashboard={true} />
          </Box>
        </Box>
      </Box>
    </Box>
  );
};

export default Dashboard;
