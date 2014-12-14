
@echo off
SETLOCAL DisableDelayedExpansion
cd..
cd .report
cd bin
SET "r=%__CD__%"
FOR /R . %%F IN (*.Test.dll) DO (
  SET "p=%%F"
  SETLOCAL EnableDelayedExpansion
  ECHO !p:%r%=!
  ENDLOCAL
)
setlocal enabledelayedexpansion
cd ..
cd ..
cd build_utils

pause