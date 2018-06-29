@echo off
SETLOCAL
@REM  ----------------------------------------------------------------------------
@REM  open-cover.cmd
@REM
@REM  author: mario.moreno@live.com
@REM
@REM  ----------------------------------------------------------------------------

echo.
echo =========================================================
echo   Report Generator Script                                          
echo =========================================================
echo.

pushd %~dp0
set start_time=%time%
set reportgenerator_bin=..\report-generator\ReportGenerator.exe
set opencover_file=%CD%\..\..\Src\open-cover.xml
set target_dir="%CD%\..\..\Src\coverage-report"

@REM  Shorten the command prompt for making the output easier to read
set savedPrompt=%prompt%
set prompt=$$$g$s

IF NOT EXIST %target_dir%\NUL GOTO NoCoverageReport
rmdir /s /q %target_dir%
:NoCoverageReport
md %target_dir%

%reportgenerator_bin% -reports:%opencover_file% -targetdir:%target_dir% -reporttypes:Html
@if %errorlevel% NEQ 0 goto error
goto success

:error
echo an error has occurred.
GOTO finish

:success
echo process successfully finished.
echo start time: %start_time%
echo end time: %Time%

:finish
popd
set prompt=%savedPrompt%

echo on