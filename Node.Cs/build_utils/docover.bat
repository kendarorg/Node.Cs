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

rd /s /q "%SLN_DIR%\.report\bin\*"  >NUL 2>NUL


echo Restore packages
if "%VERBOSITY%"=="TRUE" (
	msbuild .nuget\NuGet.targets /target:RestorePackages
) ELSE (	
	msbuild .nuget\NuGet.targets /target:RestorePackages >NUL 2>NUL
)
echo Ensure build is up to date
if "%VERBOSITY%"=="TRUE" (
	msbuild "Node.Cs.sln" /target:Clean  "/property:Configuration=Release;OutDir=%SLN_DIR%\%OUT_DIR%"
	msbuild "Node.Cs.sln" /target:Rebuild  "/property:Configuration=Release;OutDir=%SLN_DIR%\%OUT_DIR%"
) ELSE (	
	msbuild "Node.Cs.sln" /target:Clean  "/property:Configuration=Release;OutDir=%SLN_DIR%\%OUT_DIR%" >NUL 2>NUL
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

REM ^(+Test\.dll)$
REM "-coverbytest:+[*]* -[*.Test]* -[*.TestUtils]*"
if "%VERBOSITY%"=="TRUE" (
	
	build_utils\opencover\OpenCover.Console.exe -register:user "-target:%OUT_DIR%\tmptest.bat" -mergebyhash "-filter:+[*]* -[*.Test]* -[*.TestUtils]*" "-output:.report\bin\output.xml"
	
	echo Generate the report

	build_utils\report_generator\ReportGenerator.exe "-targetdir:.report" "-reporttypes:Html" "-reports:.report\bin\output.xml"

	echo Generate the syntethic report

	build_utils\report_generator\ReportGenerator.exe "-targetdir:docs\Node.Cs.Help" "-reporttypes:TextSummary" "-reports:.report\bin\output.xml"
		
) ELSE (

	build_utils\opencover\OpenCover.Console.exe -register:user "-target:%OUT_DIR%\tmptest.bat"  -mergebyhash "-filter:+[*]* -[*.Test]* -[*.TestUtils]*" "-output:.report\bin\output.xml"  >NUL 2>NUL

	echo Generate the report

	build_utils\report_generator\ReportGenerator.exe "-targetdir:.report" "-reporttypes:Html" "-reports:.report\bin\output.xml"  >NUL 2>NUL

	echo Generate the syntethic report

	build_utils\report_generator\ReportGenerator.exe "-targetdir:docs\Node.Cs.Help" "-reporttypes:TextSummary" "-reports:.report\bin\output.xml"  >NUL 2>NUL
		
)	
echo Report created

type docs\Node.Cs.Help\_coverage.template >docs\Node.Cs.Help\coverage.md
type docs\Node.Cs.Help\Summary.txt >>docs\Node.Cs.Help\coverage.md
echo. >>docs\Node.Cs.Help\coverage.md
echo ^</pre^> >>docs\Node.Cs.Help\coverage.md

if "%VERBOSITY%"=="TRUE" (
	Echo Leaving report dir bin
) ELSE (
	rd /s /q "%SLN_DIR%\.report\bin"  >NUL 2>NUL
	rd /s /q "%SLN_DIR%\TestResults"  >NUL 2>NUL
	build_utils\cleanup.bat  >NUL 2>NUL
	del /y docs\Node.Cs.Help\Summary.txt  >NUL 2>NUL
	del /y build_utils\cleanup.bat   >NUL 2>NUL
)	

REM Open the report
start .report\index.htm