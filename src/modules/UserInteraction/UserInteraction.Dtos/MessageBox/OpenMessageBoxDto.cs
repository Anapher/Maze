using System.Collections.Generic;
using System.Text;

namespace UserInteraction.Dtos
{
   public class OpenMessageBoxDto
    {
        public string Text { get; set; }
        public string Caption { get; set; }
        public SystemButtons Buttons { get; set; }
        public SystemIcon Icon { get; set; }

    }
}