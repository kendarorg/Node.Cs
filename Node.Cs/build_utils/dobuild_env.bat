@echo off >NUL 2>NUL

SET VS_VERSION=12.0
SET VERBOSITY=FALSE

echo Setting up environment variables
echo ================================================================

SET UTILS_ROOT=%CD%
SET THIS_DRIVE=%CD:~0,2%

SET VS_NAME=VSPATH

VisualStudioIdentifier %VS_VERSION% %VS_NAME% vs.bat"
call vs.bat
if "%VERBOSITY%"=="TRUE" (
    del /q vs.bat
) ELSE (
    del /q vs.bat >NUL 2>NUL
)
Cd ..
SET CURRENT_DIR=%CD%
SET SOLUTION_DIR=%CD%
SET NUGET_DIR=%CURRENT_DIR%\.nuget


SET VS_DRIVE=%VSPATH:~1,2%

%VS_DRIVE%
cd %VSPATH%
cd..
SET VSPATH=%CD%

call "%VSPATH%\Tools\VsDevCmd.bat"

%THIS_DRIVE%
CD %CURRENT_DIR%

echo @echo off ^>NUL 2^>NUL > %UTILS_ROOT%\cleanup.bat

REM mkdir tmp_nuget
CD %UTILS_ROOT%
echo.