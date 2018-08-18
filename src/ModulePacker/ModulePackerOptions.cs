using CommandLine;

namespace ModulePacker
{
    public class ModulePackerOptions
    {
        [Value(0, Required = true, HelpText = "The directory that contains the nuget packages")]
        public string DirectoryPath { get; set; }

        [Option('o', "output", Required = false, HelpText = "The output directory")]
        public string Output { get; set; }

        [Option("name", Required = false, HelpText = "The name of the module. This application will search for name.[client|server|administration].nupkg in the directory.")]
        public string ModuleName { get; set; }

        [Option("delete", Default = false, HelpText = "Delete the source packages")]
        public bool DeleteSourcePackages { get; set; }
    }
}