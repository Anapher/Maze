using System.Windows.Controls;
using Orcus.Administration.Library.Views;

namespace Orcus.Administration.Library.Resources
{
    internal class VisualStudioIcons : XamlIconsBase
    {
        public static Viewbox UserVoice()
        {
            return CreateImage(
                @"<Viewbox Width=""16"" Height=""16"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
  <Rectangle Width=""16"" Height=""16"">
    <Rectangle.Fill>
      <DrawingBrush>
        <DrawingBrush.Drawing>
          <DrawingGroup>
            <DrawingGroup.Children>
              <GeometryDrawing Brush=""#00FFFFFF"" Geometry=""F1M16,16L0,16 0,0 16,0z"" />
              <GeometryDrawing Brush=""{DynamicResource HaloBrush}"" Geometry=""F1M16,0L16,8 12.805,8 9.317,10.989C9.283,11.039 9.256,11.094 9.219,11.142 10.354,12.063 11.109,13.41 11.271,14.891L11.393,16 0,16 0.122,14.891C0.284000000000001,13.41 1.039,12.063 2.173,11.142 1.585,10.377 1.255,9.432 1.255,8.441 1.255,6.231 2.883,4.408 5,4.07L5,0z"" />
              <GeometryDrawing Brush=""{DynamicResource LightBrush2}"" Geometry=""F1M7,2L7,4.218C7.991,4.525,8.839,5.152,9.399,6L11,6 11,6.913 12.065,6 14,6 14,2z"" />
              <GeometryDrawing Brush=""{DynamicResource DarkBrush1}"" Geometry=""F1M8.1372,8.4414C8.1372,7.0954 7.0422,6.0004 5.6972,6.0004 4.3502,6.0004 3.2552,7.0954 3.2552,8.4414 3.2552,9.7874 4.3502,10.8824 5.6972,10.8824 7.0422,10.8824 8.1372,9.7874 8.1372,8.4414 M10.2762,15.0004L9.2632,15.0004C9.0172,13.2434 7.5192,11.8824 5.6972,11.8824 3.8732,11.8824 2.3752,13.2434 2.1292,15.0004L1.1162,15.0004C1.2962,13.3524 2.3422,11.9654 3.7932,11.3044 2.8672,10.6864 2.2552,9.6344 2.2552,8.4414 2.2552,6.5444 3.7992,5.0004 5.6972,5.0004 7.5942,5.0004 9.1372,6.5444 9.1372,8.4414 9.1372,9.6344 8.5252,10.6874 7.6002,11.3044 9.0512,11.9654 10.0962,13.3524 10.2762,15.0004 M15.0002,1.0004L15.0002,7.0004 12.4342,7.0004 10.0902,9.0094C10.1142,8.8214 10.1372,8.6334 10.1372,8.4414 10.1372,8.0804 10.0822,7.7334 10.0002,7.3974L10.0002,7.0004 9.8772,7.0004C9.7542,6.6464,9.6022,6.3074,9.3992,6.0004L11.0002,6.0004 11.0002,6.9134 12.0652,6.0004 14.0002,6.0004 14.0002,2.0004 7.0002,2.0004 7.0002,4.2174C6.6792,4.1184,6.3462,4.0544,6.0002,4.0304L6.0002,1.0004z"" />
            </DrawingGroup.Children>
          </DrawingGroup>
        </DrawingBrush.Drawing>
      </DrawingBrush>
    </Rectangle.Fill>
  </Rectangle>
</Viewbox>");
        }

        public static Viewbox ComputerService()
        {
            return CreateImage(
                @"<Viewbox Width=""16"" Height=""16"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
   <Rectangle Width=""16"" Height=""16"">
    <Rectangle.Fill>
      <DrawingBrush>
        <DrawingBrush.Drawing>
          <DrawingGroup>
            <DrawingGroup.Children>
              <GeometryDrawing Brush=""#00FFFFFF"" Geometry=""F1M16,16L0,16 0,0 16,0z"" />
              <GeometryDrawing Brush=""{DynamicResource HaloBrush}"" Geometry=""F1M5.0004,-0.000199999999999534L5.0004,4.0008 0.9994,4.0008 0.9994,6.9998 1.9994,6.9998 1.9994,7.9998 0.9994,7.9998 0.9994,10.9998 5.0004,10.9998 5.0004,15.9998 15.0004,15.9998 15.0004,-0.000199999999999534z"" />
              <GeometryDrawing Brush=""{DynamicResource DarkBrush2}"" Geometry=""F1M2,6L5.001,6 5.001,5 2,5z"" />
              <GeometryDrawing Brush=""{DynamicResource DarkBrush2}"" Geometry=""F1M3,8L5.001,8 5.001,7 3,7z"" />
              <GeometryDrawing Brush=""{DynamicResource DarkBrush2}"" Geometry=""F1M2,10L5.001,10 5.001,9 2,9z"" />
              <GeometryDrawing Brush=""{DynamicResource DarkBrush2}"" Geometry=""F1M12,4L8,4 8,3 12,3z M12,6L8,6 8,5 12,5z M12,13L11,13 11,12 12,12z M6,15L14,15 14,1 6,1z"" />
              <GeometryDrawing Brush=""{DynamicResource LightBrush1}"" Geometry=""F1M8,4L12,4 12,3 8,3z"" />
              <GeometryDrawing Brush=""{DynamicResource LightBrush1}"" Geometry=""F1M8,6L12,6 12,5 8,5z"" />
              <GeometryDrawing Brush=""{DynamicResource LightBrush1}"" Geometry=""F1M11,13L12,13 12,12 11,12z"" />
            </DrawingGroup.Children>
          </DrawingGroup>
        </DrawingBrush.Drawing>
      </DrawingBrush>
    </Rectangle.Fill>
  </Rectangle>
</Viewbox>");
        }
    }
}