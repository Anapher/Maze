using Maze.Client.Administration.Core.Wix.Tools;

namespace Maze.Client.Administration.Core.Wix
{
    public class WixTools
    {
        public WixTools(IWixToolRunner toolRunner)
        {
            Heat = new WixHeatTool(toolRunner);
            Candle = new WixCandleTool(toolRunner);
            Light = new WixLightTool(toolRunner);
        }

        public WixHeatTool Heat { get; }
        public WixCandleTool Candle { get; }
        public WixLightTool Light { get; }
    }
}