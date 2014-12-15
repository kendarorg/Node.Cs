@echo off >NUL 2>NUL

SET SLN_NAME=%1
SET FRAMEWORK_VERSION=%2
SET FRAMEWORK_NUGET_VERSION=%3

if "%VS_VERSION%"=="" (
    echo ERROR!!!
    echo This batch cannot be run alone!!
    pause
    exit 1
)  

if "%VERBOSITY%"=="TRUE" (
	echo Restore packages
	msbuild "%SOLUTION_DIR%\.nuget\NuGet.targets" /target:RestorePackages
	echo Rebuild solution
	msbuild "%SOLUTION_DIR%\%SLN_NAME%" /verbosity:n /target:Rebuild /p:TargetFrameworkVersion=v%FRAMEWORK_VERSION%;Configuration=Release /p:DefineConstants=%FRAMEWORK_NUGET_VERSION%
) ELSE (	
	echo Restore packages
	msbuild "%SOLUTION_DIR%\.nuget\NuGet.targets" /target:RestorePackages >NUL 2>NUL
	echo Rebuild solution
	msbuild "%SOLUTION_DIR%\%SLN_NAME%" /verbosity:q /target:Rebuild /p:TargetFrameworkVersion=v%FRAMEWORK_VERSION%;Configuration=Release /p:DefineConstants=%FRAMEWORK_NUGET_VERSION% >NUL 2>NUL
)