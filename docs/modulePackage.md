# Target Frameworks

| .Net  | Orcus   |
| ----- | ------- |
| uap   | admin   |
| net   | server  |
| win   | client  |

Sample nuspec:
```xml
<?xml version="1.0" encoding="utf-8"?>
<package
    xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
    <metadata>
        <id>PowerUserTools</id>
        <version>1.0</version>
        <description>Hello World!</description>
        <authors>Anapher</authors>
        <!-- Optional elements -->
        <title>Hello World</title>
        <!-- <projectUrl>asdasd</projectUrl> -->
        <!-- <iconUrl></iconUrl> -->
        <references>
            <group targetFramework="net">
                <reference file="PowerUserTools.dll" />
            </group>
            <!--
        <group targetFramework="netcore"><reference file="b45.dll" /></group><group targetFramework="netcoreapp"><reference file="bcore45.dll" /></group>
         -->
        </references>
        <dependencies>
            <group targetFramework="net">
                <dependency id="NETStandard.Library" version="1.5.0" />
            </group>
        </dependencies>
        <packageTypes>
            <packageType name="Administration" />
        </packageTypes>
    </metadata>
    <!-- Optional 'files' node -->
    <files>
        <file src="bin\Debug\netstandard2.0\PowerUserTools.dll" target="lib\client" />
    </files>
</package>
```