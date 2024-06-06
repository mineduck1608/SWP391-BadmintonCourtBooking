import {Box} from "@mui/material"
import Head from "../../Components/Head"
import BarChart from "../../Components/BarChart"

const Bar = () => {
    return (
        <Box m="20px">
            <Head title="Bar Chart" subtitle="Simple Bar Chart"/>
            <Box height="75vh">
                <BarChart />
            </Box>
        </Box>
    )
}

export default Bar;