using Maze.Administration.Library.Deployment;
using Maze.Client.Administration.Core;
using Maze.Client.Administration.Core.Wix;
using Maze.Client.Administration.Core.Wix.Tools;
using NuGet.Frameworks;
using NuGet.Versioning;
using Prism.Ioc;
using Prism.Modularity;
using Unclassified.TxLib;

namespace Maze.Client.Administration
{
    public class PrismModule : IModule
    {
        public static readonly NuGetFramework ClientFramework = FrameworkConstants.CommonFrameworks.MazeClient10;
        public static readonly SemanticVersion ClientVersion = SemanticVersion.Parse("0.0.1-alpha");

        //18>		C:\Program Files (x86)\WiX Toolset v3.11\bin\Heat.exe dir ..\CodeElements.Suite\bin\Release\net47 -cg ReleaseComponents -dr INSTALLFOLDER -scom -sreg -srd -var var.BasePath -gg -sfrag -out Components.Generated.wxs
        //18>		C:\Program Files (x86)\WiX Toolset v3.11\bin\candle.exe -dBasePath=..\CodeElements.Suite\bin\Release\net47 -d"DevEnvDir=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\\" -dSolutionDir=F:\Projects\CodeElements\ -dSolutionExt=.sln -dSolutionFileName=CodeElements.sln -dSolutionName=CodeElements -dSolutionPath=F:\Projects\CodeElements\CodeElements.sln -dConfiguration=Debug -dOutDir=bin\Debug\ -dPlatform=x86 -dProjectDir=F:\Projects\CodeElements\src\CodeElements.Suite.Installer\ -dProjectExt=.wixproj -dProjectFileName=CodeElements.Suite.Installer.wixproj -dProjectName=CodeElements.Suite.Installer -dProjectPath=F:\Projects\CodeElements\src\CodeElements.Suite.Installer\CodeElements.Suite.Installer.wixproj -dTargetDir=F:\Projects\CodeElements\src\CodeElements.Suite.Installer\bin\Debug\ -dTargetExt=.msi -dTargetFileName=CodeElements.Suite.msi -dTargetName=CodeElements.Suite -dTargetPath=F:\Projects\CodeElements\src\CodeElements.Suite.Installer\bin\Debug\CodeElements.Suite.msi -dCodeElements.Suite.Configuration=Debug -d"CodeElements.Suite.FullConfiguration=Debug|AnyCPU" -dCodeElements.Suite.Platform=AnyCPU -dCodeElements.Suite.ProjectDir=F:\Projects\CodeElements\src\CodeElements.Suite\ -dCodeElements.Suite.ProjectExt=.csproj -dCodeElements.Suite.ProjectFileName=CodeElements.Suite.csproj -dCodeElements.Suite.ProjectName=CodeElements.Suite -dCodeElements.Suite.ProjectPath=F:\Projects\CodeElements\src\CodeElements.Suite\CodeElements.Suite.csproj -dCodeElements.Suite.TargetDir=F:\Projects\CodeElements\src\CodeElements.Suite\bin\Debug\net47\ -dCodeElements.Suite.TargetExt=.exe -dCodeElements.Suite.TargetFileName=CodeElements.Suite.exe -dCodeElements.Suite.TargetName=CodeElements.Suite -dCodeElements.Suite.TargetPath=F:\Projects\CodeElements\src\CodeElements.Suite\bin\Debug\net47\CodeElements.Suite.exe -out obj\Debug\ -arch x86 -ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUIExtension.dll" Components.Generated.wxs Components.wxs Directories.wxs Product.wxs
        //18>		C:\Program Files (x86)\WiX Toolset v3.11\bin\Light.exe -out F:\Projects\CodeElements\src\CodeElements.Suite.Installer\bin\Debug\en-us\CodeElements.Suite.msi -pdbout F:\Projects\CodeElements\src\CodeElements.Suite.Installer\bin\Debug\en-us\CodeElements.Suite.wixpdb -cultures:en-us -ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUIExtension.dll" -loc Common.wxl -contentsfile obj\Debug\CodeElements.Suite.Installer.wixproj.BindContentsFileListen-us.txt -outputsfile obj\Debug\CodeElements.Suite.Installer.wixproj.BindOutputsFileListen-us.txt -builtoutputsfile obj\Debug\CodeElements.Suite.Installer.wixproj.BindBuiltOutputsFileListen-us.txt -wixprojectfile F:\Projects\CodeElements\src\CodeElements.Suite.Installer\CodeElements.Suite.Installer.wixproj obj\Debug\Components.Generated.wixobj obj\Debug\Components.wixobj obj\Debug\Directories.wixobj obj\Debug\Product.wixobj

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("Maze.Client.Administration.Resources.Maze.Client.Translation.txd");

            containerRegistry.Register<IClientDeployer, DefaultClientDeployer>(nameof(DefaultClientDeployer));
            containerRegistry.RegisterInstance<IWixToolRunner>(new WixToolRunner("C:\\Program Files (x86)\\WiX Toolset v3.11\\bin"));

            containerRegistry.Register<IMazeWixBuilder, MazeWixBuilder>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}