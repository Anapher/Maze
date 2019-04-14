param([string] $Package)
Add-Type -AssemblyName System.IO.Compression.FileSystem

$zip =  [System.IO.Compression.ZipFile]::Open($Package, "Update")
$configFile = $zip.GetEntry("RemoteDesktop.nuspec")

# Update the contents of the nuspec
$reader = [System.IO.StreamReader]($configFile).Open()
$content = $reader.ReadToEnd()
$reader.Dispose()

$content = $content -replace '        <dependency id="OpenH264Lib" version="1.0.0" />' + "`r`n", ''
$content = $content -replace '        <dependency id="x264net" version="1.0.0" />' + "`r`n", ''

$writer = [System.IO.StreamWriter]($configFile).Open()
$writer.BaseStream.SetLength(0)

$writer.Write($content)
$writer.Dispose()

# Update package
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\libraries\OpenH264net\Release\OpenH264Lib.dll", "lib\admin10\OpenH264Lib.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\libraries\OpenH264net\Release\OpenH264Lib.pdb", "lib\admin10\OpenH264Lib.pdb")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\libraries\OpenH264net\lib\openh264\x64\openh264-1.8.0-win64.dll", "lib\admin10\x64\openh264-1.8.0-win64.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\libraries\OpenH264net\lib\openh264\x86\openh264-1.8.0-win32.dll", "lib\admin10\x86\openh264-1.8.0-win32.dll")

[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\libraries\x264net\Release\x264net.dll", "lib\client10\x264net.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\libraries\x264net\Release\x264net.pdb", "lib\client10\x264net.pdb")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\libraries\x264net\Release\x264net.xml", "lib\client10\x264net.xml")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\libraries\x264net\lib\x264\release-x64\x264.dll", "lib\client10\x64\x264.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\libraries\x264net\lib\x264\release-x86\x264.dll", "lib\client10\x86\x264.dll")

$zip.Dispose()