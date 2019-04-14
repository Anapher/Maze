Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

# Make .NET's current directory follow PowerShell's
# current directory, if possible.
if ($PWD.Provider.Name -eq 'FileSystem') {
      [System.IO.Directory]::SetCurrentDirectory($PWD)
}

$tempFolder = "./bld"
$libsFile = "$tempFolder/libs.zip"
If (!(Test-Path $libsFile))
{
      New-Item -ItemType Directory -Force -Path $tempFolder | Out-Null

      [Net.ServicePointManager]::SecurityProtocol = "tls12, tls11, tls"
      Invoke-WebRequest -Uri "https://github.com/MazeAdmin/x264net/releases/download/1.0/nativelibs.zip" -OutFile "$tempFolder/libs.zip"
}

Remove-Item -LiteralPath "./libraries/OpenH264net/lib/" -Force -Recurse -ErrorAction Ignore
Remove-Item -LiteralPath "./libraries/x264net/lib/" -Force -Recurse -ErrorAction Ignore

Unzip "$tempFolder/libs.zip" "./libraries"