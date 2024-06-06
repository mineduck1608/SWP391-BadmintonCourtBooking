import { Box } from "@mui/material";
import Head from "../../Components/Head";
import LineChart from "../../Components/LineChart";

const Line = () => {
  return (
    <Box m="20px">
      <Head title="Line Chart" subtitle="Simple Line Chart" />
      <Box height="75vh">
        <LineChart />
      </Box>
    </Box>
  );
};

export default Line;
