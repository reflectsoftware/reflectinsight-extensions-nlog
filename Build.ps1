param(
    [String] $majorMinor = "5.7",  # 5.7
    [String] $patch = "1",         # $env:APPVEYOR_BUILD_VERSION
    [String] $customLogger = "",   # C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll
    [Switch] $notouch,
    [String] $project = "ReflectSoftware.Insight.Extensions.NLog"
)

function Set-AssemblyVersions($informational, $assembly)
{
    (Get-Content assets/VersionInfo.cs) |
        ForEach-Object { $_ -replace """1.0.0.0""", """$assembly""" } |
        ForEach-Object { $_ -replace """1.0.0""", """$informational""" } |
        ForEach-Object { $_ -replace """1.1.1.1""", """$($informational).0""" } |
        Set-Content assets/VersionInfo.cs
}

function Install-NuGetPackages($solution)
{
	#(New-Object Net.WebClient).DownloadFile('https://www.nuget.org/nuget.exe', 'C:\Tools\NuGet\NuGet.exe')
    nuget restore $solution
}

function Invoke-MSBuild($solution, $customLogger)
{
    if ($customLogger)
    {
        msbuild "$solution" /verbosity:minimal /p:Configuration=Release /logger:"$customLogger"
    }
    else
    {
        msbuild "$solution" /verbosity:minimal /p:Configuration=Release
    }
}

function Invoke-NuGetPackProj($csproj)
{
    nuget pack -Prop Configuration=Release -Symbols $csproj
}

function Invoke-NuGetPackSpec($nuspec, $version)
{
		nuget pack $nuspec -Version $version -OutputDirectory ..\
}

function Invoke-NuGetPack($version)
{
    #ls src/**/*.csproj |
    #    Where-Object { -not ($_.Name -like "*net40*") } |
    #    ForEach-Object { Invoke-NuGetPackProj $_ }

		ls src/**/*.csproj |
        Where-Object { -not ($_.Name -like "*net40*") } |
        ForEach-Object { Invoke-NuGetPackProj $_ }

    pushd .\src
    Invoke-NuGetPackSpec "ReflectSoftware.Insight.Extensions.NLog.nuspec" $version
    popd
}

function Invoke-Build($project, $majorMinor, $patch, $customLogger, $notouch)
{
    #$solution2 = "$project 2.0.sln"
    #$solution4 = "$project 4.0.sln"
    $solution45 = "$project 4.5.sln"

    $package="$majorMinor.$patch"

    Write-Output "$project $package"

    if (-not $notouch)
    {
        $assembly = "$majorMinor.0.0"

        Write-Output "Assembly version will be set to $assembly"
        Set-AssemblyVersions $package $assembly
    }

    Install-NuGetPackages $solution45
    Invoke-MSBuild $solution45 $customLogger

	#Install-NuGetPackages $solution4
    #Invoke-MSBuild $solution4 $customLogger

	#Install-NuGetPackages $solution2
    #Invoke-MSBuild $solution2 $customLogger

    Invoke-NuGetPack $package
}

$ErrorActionPreference = "Stop"

if (-not $sln)
{
    $slnfull = ls *.sln |
        Where-Object { -not ($_.Name -like "*net40*") } |
        Select -first 1

    $sln = $slnfull.BaseName
}

Invoke-Build $project $majorMinor $patch $customLogger $notouch

