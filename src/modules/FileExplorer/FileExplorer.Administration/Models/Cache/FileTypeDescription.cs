using System.Windows.Media;

namespace FileExplorer.Administration.Models.Cache
{
    public class FileTypeDescription
    {
        public FileTypeDescription(string description, ImageSource icon)
        {
            Description = description;
            Icon = icon;
        }

        public string Description { get; }
        public ImageSource Icon { get; }
    }
}