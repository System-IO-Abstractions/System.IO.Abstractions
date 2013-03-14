properties { 
  $majorVersion = "1.4"
  $majorWithReleaseVersion = "1.4.1"
  $version = GetVersion $majorWithReleaseVersion
  $signAssemblies = $false
  $signKeyPath = "D:\Development\Releases\newtonsoft.snk"  #path for whomever can sign the assemblies
  $buildDocumentation = $false
  $treatWarningsAsErrors = $false
  
  $baseDir  = resolve-path "."
  $buildDir = "$baseDir\Build"
  $sourceDir = "$baseDir"
  $toolsDir = "$baseDir\Tools"
  $testsDir = "$baseDir\Build\Testing"
  $docDir = "$baseDir\Doc"
  $workingDir = "$baseDir\Build\Working"
  
  $nuspecFile = "System.IO.Abstractions.nuspec" #filename only
  $nuget_executible = ".\.nuget\NuGet.exe"
  
  $builds = @(
	@{Project = "System.IO.Abstractions.csproj"; Tests = ""; Constants=""; FinalDir="Net"; NuGetDir = "net"; Framework="net-4.0"; Sign=$false}
	@{Project = "TestingHelpers.csproj"; Tests = "TestHelpers.Tests.csproj"; Constants=""; FinalDir="Net"; NuGetDir = "net"; Framework="net-4.0"; Sign=$false}
    #@{Project = "Newtonsoft.Json"; TestsName = "Newtonsoft.Json.Tests"; Constants=""; FinalDir="Net40"; NuGetDir = "net40"; Framework="net-4.0"; Sign=$true},
    #@{Project = "Newtonsoft.Json.Portable"; TestsName = "Newtonsoft.Json.Tests.Portable"; Constants="PORTABLE"; FinalDir="Portable"; NuGetDir = "portable-net40+sl4+wp7+win8"; Framework="net-4.0"; Sign=$true},
    #@{Project = "Newtonsoft.Json.WinRT"; TestsName = $null; Constants="NETFX_CORE"; FinalDir="WinRT"; NuGetDir = "winrt45"; Framework="net-4.5"; Sign=$true},
    #@{Project = "Newtonsoft.Json.WindowsPhone"; TestsName = $null; Constants="SILVERLIGHT;WINDOWS_PHONE"; FinalDir="WindowsPhone"; NuGetDir = "sl3-wp,sl4-windowsphone71"; Framework="net-4.0"; Sign=$true},
    #@{Project = "Newtonsoft.Json.Silverlight"; TestsName = "Newtonsoft.Json.Tests.Silverlight"; Constants="SILVERLIGHT"; FinalDir="Silverlight"; NuGetDir = "sl4"; Framework="net-4.0"; Sign=$true},
    #@{Project = "Newtonsoft.Json.Net35"; TestsName = "Newtonsoft.Json.Tests.Net35"; Constants="NET35"; FinalDir="Net35"; NuGetDir = "net35"; Framework="net-2.0"; Sign=$true},
    #@{Project = "Newtonsoft.Json.Net20"; TestsName = "Newtonsoft.Json.Tests.Net20"; Constants="NET20"; FinalDir="Net20"; NuGetDir = "net20"; Framework="net-2.0"; Sign=$true}
  )
}

$framework = '4.0x86'

task default -depends Test

# Ensure a clean working directory
task Clean {
  Set-Location $baseDir
  
  if (Test-Path -path $workingDir)
  {
    Write-Output "Deleting Working Directory"
    del $workingDir -Recurse -Force
  }
  
  if (Test-Path -path $testsDir)
  {
	Write-Output "Deleting Testing directory"
	del $testsDir -Recurse -Force
  }
  
}

# Build each solution, optionally signed
task Build -depends Clean,UpdateAssemblyInfoVersions { 

  New-Item -Path $workingDir -ItemType Directory

  foreach ($build in $builds)
  {
    $nameInfo = new-object System.IO.FileInfo([string]$build.Project)
	$name = $nameInfo.Name.Replace($nameInfo.Extension,"")  # chop off the extension
	$name = (Join-Path $name $nameInfo.Name)                # gives [name]\[name.csproj]
    $finalDir = $build.FinalDir
    $sign = ($build.Sign -and $signAssemblies)

	Write-Host 
    Write-Host -ForegroundColor Green "Building " $name
    Write-Host -ForegroundColor Green "Signed " $sign
    Write-Host
    exec { msbuild "/t:Clean;Rebuild" /p:Configuration=Release "/p:Platform=Any CPU" /p:OutputPath=$workingDir\bin\Release\$finalDir\ /p:AssemblyOriginatorKeyFile=$signKeyPath "/p:SignAssembly=$sign" "/p:TreatWarningsAsErrors=$treatWarningsAsErrors" (GetConstants $build.Constants $sign) "$sourceDir\$name" | Out-Default } "Error building $name"
  }
}

task UpdateAssemblyInfoVersions {
  Write-Host -ForegroundColor Green "Updating assembly version"
  Write-Host
  Update-AssemblyInfoFiles $sourceDir ($majorVersion + '.0.0') $version
}

# Run tests on deployed files
#task Test -depends Deploy {
task Test -depends Build {

	Write-Verbose "Ensuring $testsDir exists"
	if ((Test-Path -path $testsDir) -eq $false)
	{
		New-Item -Path $testsDir -ItemType Directory | Out-Null
	}

	foreach ($build in $builds)
	{
		if ([System.String]::IsNullOrEmpty($build.Tests) -ne $true )
		{
			$nameInfo = new-object System.IO.FileInfo([string]$build.Tests)
			$projname = $nameInfo.Name.Replace($nameInfo.Extension,"")  # chop off the extension
			$name = (Join-Path $projname $nameInfo.Name)                # gives [name]\[name.csproj]
		
			$finalDir = $build.FinalDir
			$framework = $build.Framework

			Write-Host
			Write-Host -ForegroundColor Green "Building tests assembly $name"
			Write-Host
			exec { & msbuild /p:Configuration=Release /p:OutputPath=bin\Release\ "$sourceDir\$name" }
			
			Write-Host
			Write-Host -ForegroundColor Green "Copying test assembly $name to deployed directory"
			Write-Host
			& robocopy /mir "$sourceDir\$projname\bin\Release" "$testsDir\bin\$finalDir"
			
			#robocopy ".\Src\Google.Maps\bin\Release\$finalDir" $workingDir\Deployed\Bin\$finalDir /MIR /NP /XO /XF LinqBridge.dll | Out-Default
			
			#Copy-Item -Path "$testsDir\bin\$finalDir\*" -Destination $workingDir\Testing\Bin\$finalDir\

			Write-Host
			Write-Host -ForegroundColor Green "Running tests " $projname
			Write-Host -ForegroundColor Green "  $workingDir\TestResult.xml"
			Write-Host
			exec { .\Tools\NUnit.Runners\tools\nunit-console.exe "$testsDir\Bin\$finalDir\$projname.dll" /labels /framework=$framework /xml:$workingDir\$projname.xml | Out-Default } "Error running $name tests"
		}
	}
}

# Optional build documentation, add files to final zip
task Package -depends Test {
  foreach ($build in $builds)
  {
    $name = $build.TestsName
    $finalDir = $build.FinalDir
    
    #robocopy "$sourceDir\Google.Maps\bin\Release\$finalDir" $workingDir\Package\Bin\$finalDir /NP /XO /XF *.pri | Out-Default
  }
  
#  if ($buildDocumentation)
#  {
#    $mainBuild = $builds | where { $_.Name -eq "Newtonsoft.Json" } | select -first 1
#    $mainBuildFinalDir = $mainBuild.FinalDir
#    $documentationSourcePath = "$workingDir\Package\Bin\$mainBuildFinalDir"
#    Write-Host -ForegroundColor Green "Building documentation from $documentationSourcePath"
#
#    # Sandcastle has issues when compiling with .NET 4 MSBuild - http://shfb.codeplex.com/Thread/View.aspx?ThreadId=50652
#    exec { msbuild "/t:Clean;Rebuild" /p:Configuration=Release "/p:DocumentationSourcePath=$documentationSourcePath" $docDir\doc.shfbproj | Out-Default } "Error building documentation. Check that you have Sandcastle, Sandcastle Help File Builder and HTML Help Workshop installed."
#    
#    move -Path $workingDir\Documentation\LastBuild.log -Destination $workingDir\Documentation.log
#  }
#
#  Copy-Item -Path $docDir\readme.txt -Destination $workingDir\Package\
#  Copy-Item -Path $docDir\versions.txt -Destination $workingDir\Package\Bin\
#
#  robocopy $sourceDir $workingDir\Package\Source\Src /MIR /NP /XD .svn bin obj TestResults AppPackages /XF *.suo *.user | Out-Default
#  robocopy $buildDir $workingDir\Package\Source\Build /MIR /NP /XD .svn | Out-Default
#  robocopy $docDir $workingDir\Package\Source\Doc /MIR /NP /XD .svn | Out-Default
#  robocopy $toolsDir $workingDir\Package\Source\Tools /MIR /NP /XD .svn | Out-Default
  
#  exec { .\Tools\7-zip\7za.exe a -tzip $workingDir\$zipFileName $workingDir\Package\* | Out-Default } "Error zipping"
}

# Unzip package to a location
task Deploy -depends Package {
  exec { .\Tools\7-zip\7za.exe x -y "-o$workingDir\Deployed" $workingDir\$zipFileName | Out-Default } "Error unzipping"
}

task UpdateNuspecVersion {
	
	$versionNodePattern = "<version>[0-9]+(\.([0-9]+|\*)){1,3}</version>"
	$versionOut = "<version>" + $majorWithReleaseVersion + "</version>"
	
	Get-ChildItem -Path $baseDir *.nuspec | ForEach-Object {
		
		$filename = $_.Directory.ToString() + '\' + $_.Name
		Write-Host "$filename -> $majorWithReleaseVersion"
		
		(Get-Content $filename) | Foreach-Object {
			% {$_ -replace $versionNodePattern, $versionOut }
		} | Set-Content $filename
		
	}
	
}

task NugetPackage -depends Test,UpdateNuspecVersion {
    
	New-Item -Path $workingDir\NuGet -ItemType Directory | Out-Null
    Copy-Item -Path "$baseDir\$nuspecFile" -Destination "$workingDir\NuGet"
    
    foreach ($build in $builds)
    {
      if ($build.NuGetDir -ne $null)
      {
        $name = $build.TestsName
        $finalDir = $build.FinalDir
        $frameworkDirs = $build.NuGetDir.Split(",")
        
        foreach ($frameworkDir in $frameworkDirs)
        {
          robocopy "$workingDir\bin\Release\$finalDir" "$workingDir\NuGet\lib\$frameworkDir" /NP /XO /XF *.pri | Out-Default
        }
      }
    }
  
    exec { & $nuget_executible pack "$workingDir\NuGet\$nuspecFile" -Symbols }
    move -Path .\*.nupkg -Destination $workingDir\NuGet
}



function GetConstants($constants, $includeSigned)
{
  $signed = switch($includeSigned) { $true { ";SIGNED" } default { "" } }

  return "/p:DefineConstants=`"CODE_ANALYSIS;TRACE;$constants$signed`""
}

function GetVersion($version)
{
    $now = [DateTime]::Now
	[TimeSpan]$span = $now - (new-object DateTime($now.Year,1,1))
    
    $build = "{0:0000}" -f $span.TotalHours
    
    return $version + "." + $build
}

function Update-AssemblyInfoFiles ([string] $sourceDir, [string] $assemblyVersionNumber, [string] $fileVersionNumber)
{
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $assemblyVersion = 'AssemblyVersion("' + $assemblyVersionNumber + '")';
    $fileVersion = 'AssemblyFileVersion("' + $fileVersionNumber + '")';
    
    Get-ChildItem -Path $sourceDir -r -filter AssemblyInfo.cs | ForEach-Object {
        $filename = $_.Directory.ToString() + '\' + $_.Name
        Write-Host $filename
        $filename + ' -> ' + $version
    
        (Get-Content $filename) | ForEach-Object {
            % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
            % {$_ -replace $fileVersionPattern, $fileVersion }
        } | Set-Content $filename
    }
	
	Get-ChildItem -Path $sourceDir -filter AssemblyVersion_Master.cs | ForEach-Object {
        $filename = $_.Directory.ToString() + '\' + $_.Name
        Write-Host $filename
        $filename + ' -> ' + $version
    
        (Get-Content $filename) | ForEach-Object {
            % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
            % {$_ -replace $fileVersionPattern, $fileVersion }
        } | Set-Content $filename
    }
}