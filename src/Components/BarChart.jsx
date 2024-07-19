import React, { useEffect, useState } from 'react';
import { useTheme } from "@mui/material";
import { ResponsiveBar } from "@nivo/bar";
import { tokens } from "../theme";
import { fetchWithAuth } from '../Components/fetchWithAuth/fetchWithAuth';


const fetchData = async (url, options = {}) => {
  const response = await fetchWithAuth(url, options);
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  const data = await response.json();
  return data;
};

const BarChart = ({ isDashboard = false }) => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const [branchAmounts, setBranchAmounts] = useState([]);

  useEffect(() => {
    const fetchAllData = async () => {
      try {
        const branches = await fetchData('https://localhost:7233/Branch/GetAll');

        const courts = await fetchData('https://localhost:7233/Court/GetAll');

        const slots = await fetchData('https://localhost:7233/Slot/GetAll');

        const token = sessionStorage.getItem('token');
        const payments = await fetchData('https://localhost:7233/Payment/GetAll', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        // Process data to calculate total amount for each branch
        const branchAmounts = branches.map(branch => {
          const branchCourts = courts.filter(court => court.branchId === branch.branchId);
          const branchSlots = slots.filter(slot => 
            branchCourts.some(court => court.courtId === slot.courtId)
          );
          const branchPayments = payments.filter(payment => 
            branchSlots.some(slot => slot.bookingId === payment.bookingId)
          );
          const totalAmount = branchPayments.reduce((sum, payment) => sum + payment.amount, 0);
          return { branchName: branch.branchName, totalAmount };
        });
        
        setBranchAmounts(branchAmounts);
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };

    fetchAllData();
  }, []);

  return (
    <ResponsiveBar
      data={branchAmounts}
      keys={['totalAmount']}
      indexBy="branchName"
      margin={{ top: 50, right: 130, bottom: 50, left: 60 }}
      padding={0.3}
      valueScale={{ type: "linear" }}
      indexScale={{ type: "band", round: true }}
      colors={{ scheme: "nivo" }}
      theme={{
        axis: {
          domain: {
            line: {
              stroke: colors.grey[100],
            },
          },
          legend: {
            text: {
              fill: colors.grey[100],
            },
          },
          ticks: {
            line: {
              stroke: colors.grey[100],
              strokeWidth: 1,
            },
            text: {
              fill: colors.grey[100],
            },
          },
        },
        legends: {
          text: {
            fill: colors.grey[100],
          },
        },
      }}
      axisTop={null}
      axisRight={null}
      axisBottom={{
        tickSize: 5,
        tickPadding: 5,
        tickRotation: 0,
        legend: isDashboard ? undefined : "Branch Name",
        legendPosition: "middle",
        legendOffset: 32,
      }}
      axisLeft={{
        tickSize: 5,
        tickPadding: 5,
        tickRotation: 0,
        legend: isDashboard ? undefined : "Total Amount",
        legendPosition: "middle",
        legendOffset: -40,
      }}
      enableLabel={false}
      labelSkipWidth={12}
      labelSkipHeight={12}
      labelTextColor={{
        from: "color",
        modifiers: [["darker", 1.6]],
      }}
      legends={[
        {
          dataFrom: "keys",
          anchor: "bottom-right",
          direction: "column",
          justify: false,
          translateX: 120,
          translateY: 0,
          itemsSpacing: 2,
          itemWidth: 100,
          itemHeight: 20,
          itemDirection: "left-to-right",
          itemOpacity: 0.85,
          symbolSize: 20,
          effects: [
            {
              on: "hover",
              style: {
                itemOpacity: 1,
              },
            },
          ],
        },
      ]}
      role="application"
      barAriaLabel={function (e) {
        return e.id + ": " + e.formattedValue + " in branch: " + e.indexValue;
      }}
    />
  );
};

export default BarChart;
