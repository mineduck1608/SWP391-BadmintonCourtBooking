import React, { useState, useEffect } from "react";
import { Box, useTheme } from "@mui/material";
import { tokens } from "../theme";
import { fetchWithAuth } from "./fetchWithAuth/fetchWithAuth";

const ProgressCircle = ({ size = 40 }) => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const [progress, setProgress] = useState();
  const token = sessionStorage.getItem('token');
  
  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetchWithAuth("https://localhost:7233/Slot/CancelStatistic?id=0&year=2024", {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          }
        });
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data = await response.json();
        // Assuming the progress value you get from the API is `data.progress`
        setProgress(data.progress); // Update the progress state
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };

    fetchData();
  }, [token]);
  const angle = 0.01 * 360;

  return (
    <Box
      sx={{
        background: `radial-gradient(${colors.primary[400]} 55%, transparent 56%),
            conic-gradient(${colors.blueAccent[500]} 0deg ${angle}deg, transparent ${angle}deg 360deg),
            ${colors.greenAccent[500]}`,
        borderRadius: "50%",
        width: `${size}px`,
        height: `${size}px`,
      }}
    />
  );
};

export default ProgressCircle;
