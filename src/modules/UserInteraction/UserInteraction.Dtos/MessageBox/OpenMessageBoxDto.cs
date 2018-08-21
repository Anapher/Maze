namespace UserInteraction.Dtos.MessageBox
{
    public class OpenMessageBoxDto
    {
        public string Text { get; set; }
        public string Caption { get; set; }
        public MsgBxButtons Buttons { get; set; }
        public MsgBxIcon Icon { get; set; }
    }
}