import { ResponsivePie } from "@nivo/pie";
import { tokens } from "../theme";
import { useTheme } from "@mui/material";

const PieChart = (data) => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  //data is wrapped in an object
  const actualData = data.data
  const findLongestIdLength = () => {
    var r = actualData.map(e => {
      return e.id
    }).sort((a, b) => b.length - a.length)
    return r[0]?.length
  }
  const longestIdLength = findLongestIdLength()
  return (
    <ResponsivePie
      data={actualData}
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
      margin={{ top: 40, right: 80, bottom: 80, left: 80 }}
      innerRadius={0.5}
      padAngle={0.7}
      cornerRadius={3}
      activeOuterRadiusOffset={8}
      borderColor={{
        from: "color",
        modifiers: [["darker", 0.2]],
      }}
      arcLinkLabelsSkipAngle={10}
      arcLinkLabelsTextColor={colors.grey[100]}
      arcLinkLabelsThickness={2}
      arcLinkLabelsColor={{ from: "color" }}
      enableArcLabels={false}
      arcLabelsRadiusOffset={0.4}
      arcLabelsSkipAngle={7}
      arcLabelsTextColor={{
        from: "color",
        modifiers: [["darker", 2]],
      }}
      legends={[{
        anchor: "bottom",
        direction: "row",
        justify: false,
        translateX: 0,
        translateY: 56,
        itemsSpacing: 10,
        itemWidth: Math.max(50, longestIdLength * 7),
        itemHeight: 18,
        itemTextColor: "#999",
        itemDirection: "left-to-right",
        itemOpacity: 1,
        symbolSize: 18,
        symbolShape: "circle",
        effects: [
          {
            on: "hover",
            style: {
              itemTextColor: "#000",
            },
          },
        ],
      }]
      }
      
    />
  );
};

export default PieChart;
