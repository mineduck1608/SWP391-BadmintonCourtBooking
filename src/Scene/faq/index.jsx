import {Box, useTheme, Typography} from "@mui/material";
import Head from "../../Components/Head"
import Accordion from "@mui/material/Accordion";
import AccordionSummary from "@mui/material/AccordionSummary";
import AccordionDetails from "@mui/material/AccordionDetails";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import {tokens} from "../../theme"

const FAQ =() => {
    const theme = useTheme();
    const colors = tokens(theme.palette.mode);

    return <Box m="20px">
        <Head title="FAQ" subtitle="Frequently Asked Questions Page"/>

        <Accordion defaultExpanded>
            <AccordionSummary expandIcon={<ExpandMoreIcon/>}>
            <Typography color={colors.greenAccent[500]} variant="h5">
                An important question
            </Typography>
            </AccordionSummary>
            <AccordionDetails>
                <Typography>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius nam, minus ratione illum excepturi dolor a enim cumque ipsum quae saepe dicta neque perferendis iure? Reprehenderit nemo ducimus harum quia?
                </Typography>
            </AccordionDetails>
        </Accordion>

        <Accordion defaultExpanded>
            <AccordionSummary expandIcon={<ExpandMoreIcon/>}>
            <Typography color={colors.greenAccent[500]} variant="h5">
                Another important question
            </Typography>
            </AccordionSummary>
            <AccordionDetails>
                <Typography>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius nam, minus ratione illum excepturi dolor a enim cumque ipsum quae saepe dicta neque perferendis iure? Reprehenderit nemo ducimus harum quia?
                </Typography>
            </AccordionDetails>
        </Accordion>

        <Accordion defaultExpanded>
            <AccordionSummary expandIcon={<ExpandMoreIcon/>}>
            <Typography color={colors.greenAccent[500]} variant="h5">
                Your Favorite Question
            </Typography>
            </AccordionSummary>
            <AccordionDetails>
                <Typography>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius nam, minus ratione illum excepturi dolor a enim cumque ipsum quae saepe dicta neque perferendis iure? Reprehenderit nemo ducimus harum quia?
                </Typography>
            </AccordionDetails>
        </Accordion>

        <Accordion defaultExpanded>
            <AccordionSummary expandIcon={<ExpandMoreIcon/>}>
            <Typography color={colors.greenAccent[500]} variant="h5">
                Some random questions
            </Typography>
            </AccordionSummary>
            <AccordionDetails>
                <Typography>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius nam, minus ratione illum excepturi dolor a enim cumque ipsum quae saepe dicta neque perferendis iure? Reprehenderit nemo ducimus harum quia?
                </Typography>
            </AccordionDetails>
        </Accordion>

        <Accordion defaultExpanded>
            <AccordionSummary expandIcon={<ExpandMoreIcon/>}>
            <Typography color={colors.greenAccent[500]} variant="h5">
                The Final Question
            </Typography>
            </AccordionSummary>
            <AccordionDetails>
                <Typography>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius nam, minus ratione illum excepturi dolor a enim cumque ipsum quae saepe dicta neque perferendis iure? Reprehenderit nemo ducimus harum quia?
                </Typography>
            </AccordionDetails>
        </Accordion>

    </Box>
}

export default FAQ;