import {Box} from "@mui/material"
import Head from "../../Components/Head"
import PieChart from "../../Components/PieChart"

const Pie = () => {
    return (
        <Box m="20px">
            <Head title="Pie Chart" subtitle="Simple Pie Chart"/>
            <Box height="75vh">
                <PieChart />
            </Box>
        </Box>
    )
}

export default Pie;