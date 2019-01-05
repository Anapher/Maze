#dotnet build Maze.sln -c Release

Get-ChildItem .\src\modules -Directory | ForEach-Object {
    Write-Host "Process module $_" -ForegroundColor Yellow
    dotnet .\src\ModulePacker\bin\Release\netcoreapp2.1\ModulePacker.dll .\artifacts --name $_ --delete
    if ($?) {
        Write-Host "Execute NuGet Packer" -ForegroundColor Green
        .\src\submodules\NuGet.Client\artifacts\NuGet.CommandLine\15.0\bin\Release\net46\NuGet.exe pack .\artifacts\$_ -OutputDirectory .\artifacts
        Remove-Item -Recurse -Force .\artifacts\$_
    }
}

src/modules/RemoteDesktop/patch.ps1 -Package ".\artifacts\RemoteDesktop.1.0.0.nupkg"