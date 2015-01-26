@echo off >NUL 2>NUL
call dobuild_env.bat

call dobuild_sln Node.Cs.sln 4.5 net45

REM call dobuild_single Kendar.TestUtils 4.0 net40 
call dobuild_single Kendar.TestUtils 4.5 net45
call dobuild_nuget Kendar.TestUtils

call dobuild_clean
pause
