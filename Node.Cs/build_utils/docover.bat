@echo off >NUL 2>NUL
call dobuild_env.bat

set UTILS_DIR=%CD%

cd..

set OUT_DIR=.report\bin
set SLN_DIR=%CD%

if "%VERBOSITY%"=="TRUE" (
	mkdir %SLN_DIR%\.report >NUL 2>NUL
	mkdir %SLN_DIR%\.report\bin >NUL 2>NUL
) ELSE (
	mkdir %SLN_DIR%\.report
	mkdir %SLN_DIR%\.report\bin
)	

echo Restore packages
if "%VERBOSITY%"=="TRUE" (
	msbuild .nuget\NuGet.targets /target:RestorePackages
) ELSE (	
	msbuild .nuget\NuGet.targets /target:RestorePackages >NUL 2>NUL
)
echo Ensure build is up to date
if "%VERBOSITY%"=="TRUE" (
	msbuild "Node.Cs.sln" /target:Rebuild  "/property:Configuration=Release;OutDir=%SLN_DIR%\%OUT_DIR%"
) ELSE (	
	msbuild "Node.Cs.sln" /target:Rebuild  "/property:Configuration=Release;OutDir=%SLN_DIR%\%OUT_DIR%" >NUL 2>NUL
)

set TMP_BATCH=%SLN_DIR%\%OUT_DIR%\tmptest.bat

echo @echo off ^>NUL 2^>NUL > %TMP_BATCH%
echo echo Running MsTest in %%CD%% >> %TMP_BATCH%
echo cd build_utils  >> %TMP_BATCH%
echo call dobuild_env.bat ^>NUL 2^>NUL >> %TMP_BATCH%
echo cd ..  >> %TMP_BATCH%
echo cd .report >> %TMP_BATCH%
echo cd bin >> %TMP_BATCH%


for /F "tokens=*" %%P in ('dir /b "%SLN_DIR%\%OUT_DIR%\*.Test.dll"') do (
	if "%VERBOSITY%"=="TRUE" (
		ECHO MsTest.exe /noresults /noisolation "/testcontainer:%%P" >> %TMP_BATCH%
	) ELSE (	
		ECHO MsTest.exe /noresults /noisolation "/testcontainer:%%P" ^>NUL 2^>NUL >> %TMP_BATCH%
	)
) 
echo echo MsTestConcluded  >> %TMP_BATCH%
echo cd ..  >> %TMP_BATCH%
echo cd ..  >> %TMP_BATCH%

echo Starting OpenCover
echo Result: %SLN_DIR%\.report\output.xml

build_utils\opencover\OpenCover.Console.exe^
	 -register:user^
	 "-target:%TMP_BATCH%"^
	 -mergebyhash^
	 -filter:"+[*]* -[*.Test]* -[*.TestUtils]*"^
	 "-output:%SLN_DIR%\.report\output.xml"

echo Generate the report

build_utils\report_generator\ReportGenerator.exe^
	"-targetdir:%SLN_DIR%\.report"^
	"-reporttypes:Html"^
	"-reports:%SLN_DIR%\.report\output.xml"

echo Report created
if "%VERBOSITY%"=="TRUE" (
	Echo Leaving report dir bin
) ELSE (
	rd /s /q "%SLN_DIR%\.report\bin"  
)	

REM Open the report
start .report\index.htm