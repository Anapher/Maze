#dotnet build Orcus.sln -c Release

Get-ChildItem .\src\modules -Directory | ForEach-Object {
    dotnet .\src\ModulePacker\bin\Release\netcoreapp2.1\ModulePacker.dll .\artifacts --name $_ --delete
    .\src\submodules\NuGet.Client\artifacts\NuGet.CommandLine\15.0\bin\Release\net46\NuGet.exe pack .\artifacts\$_ -OutputDirectory .\artifacts

    Remove-Item -Recurse -Force .\artifacts\$_
}