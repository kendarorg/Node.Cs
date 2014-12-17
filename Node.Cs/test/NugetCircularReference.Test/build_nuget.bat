@echo off
echo %CD%
cd ..
cd .. 
REM Solution dir
cd build_utils
call dobuild_env.bat TRUE
call dobuild_single NugetCircularReference.Test 4.0 net40 test\NugetCircularReference.Test
call dobuild_single NugetCircularReference.Test 4.5 net45 test\NugetCircularReference.Test
call dobuild_nuget NugetCircularReference.Test test\NugetCircularReference.Test

rem call dobuild_clean

