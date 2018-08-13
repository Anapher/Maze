namespace Orcus.Administration.ViewModels
{
    public class Settings
    {
        private Settings()
        {
        }

        public static Settings Current { get; } = new Settings();

        public string ModulesPath { get; }
    }
}