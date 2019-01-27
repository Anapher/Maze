using Maze.Client.Administration.Core.Wix.Tools;

namespace Maze.Client.Administration.Core.Wix
{
    public class WixTools
    {
        public string Directory { get; }

        public WixTools(string directory)
        {
            Directory = directory;

            Heat = new WixHeatTool(this);
            Candle = new WixCandleTool(this);
        }

        public WixHeatTool Heat { get; }
        public WixCandleTool Candle { get; }
    }
}