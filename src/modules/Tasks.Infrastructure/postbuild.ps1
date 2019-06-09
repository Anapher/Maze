param([string] $Package)
Add-Type -AssemblyName System.IO.Compression.FileSystem

$zip =  [System.IO.Compression.ZipFile]::Open($Package, "Update")
$configFile = $zip.GetEntry("Tasks.Infrastructure.nuspec")

# Update the contents of the nuspec
$reader = [System.IO.StreamReader]($configFile).Open()
$content = $reader.ReadToEnd()
$reader.Dispose()

$content = $content -replace "        <dependency id=""Tasks.Infrastructure.Core"" version="".*?"" />`r`n", ''

$writer = [System.IO.StreamWriter]($configFile).Open()
$writer.BaseStream.SetLength(0)

$writer.Write($content)
$writer.Dispose()

# Update package
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\Tasks.Infrastructure.Core\bin\Release\netstandard2.0\Tasks.Infrastructure.Core.dll", "lib\admin10\Tasks.Infrastructure.Core.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\Tasks.Infrastructure.Core\bin\Release\netstandard2.0\Tasks.Infrastructure.Core.dll", "lib\client10\Tasks.Infrastructure.Core.dll")
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$PSScriptRoot\Tasks.Infrastructure.Core\bin\Release\netstandard2.0\Tasks.Infrastructure.Core.dll", "lib\server10\Tasks.Infrastructure.Core.dll")

$zip.Dispose()