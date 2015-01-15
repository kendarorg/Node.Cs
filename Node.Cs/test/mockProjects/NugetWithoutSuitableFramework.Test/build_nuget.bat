@echo off
echo %CD%
cd ..
cd .. 
cd ..
REM Solution dir
cd build_utils
call dobuild_env.bat TRUE
call dobuild_single NugetWithoutSuitableFramework.Test 4.0 net40 test\mockProjects\NugetWithoutSuitableFramework.Test
call dobuild_nuget NugetWithoutSuitableFramework.Test test\mockProjects\NugetWithoutSuitableFramework.Test

rem call dobuild_clean

