function Invoke-MSBuild()
{
    $dotNetVersion = "14.0"
    $regKey = "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$dotNetVersion"
    $regProperty = "MSBuildToolsPath"

    $msbuildExe = join-path -path (Get-ItemProperty $regKey).$regProperty -childpath "msbuild.exe"

    &$msbuildExe dbup-migrator.sln /verbosity:minimal /p:Configuration=Release
}

Invoke-MSBuild $version
.\src\DataMigrations\bin\Release\DataMigrations.exe
