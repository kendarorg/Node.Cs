@echo off >NUL 2>NUL
call dobuild_env.bat

call dobuild_sln Node.Cs.sln 4.5 net45
REM call dobuild_sln Node.Cs.sln 4.0 net40

call dobuild_clean
pause
