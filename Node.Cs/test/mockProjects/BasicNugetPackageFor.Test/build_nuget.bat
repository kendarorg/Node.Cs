@echo off
echo %CD%
cd ..
cd .. 
cd .. 
REM Solution dir
cd build_utils
call dobuild_env.bat TRUE
call dobuild_single BasicNugetPackageFor.Test 4.0 net40 test\mockProjects\BasicNugetPackageFor.Test
call dobuild_single BasicNugetPackageFor.Test 4.5 net45 test\mockProjects\BasicNugetPackageFor.Test
call dobuild_nuget BasicNugetPackageFor.Test test\mockProjects\BasicNugetPackageFor.Test

rem call dobuild_clean
