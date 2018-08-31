@echo off
SETLOCAL
@REM  ----------------------------------------------------------------------------
@REM  sonar.cmd
@REM
@REM  author: m4mc3r@gmail.com
@REM
@REM  optional arguments for this batch file:
@REM  build type: defaults to Debug
@REM  ----------------------------------------------------------------------------

echo.
echo =========================================================
echo   Build script                                          
echo =========================================================
echo.

set start_time=%time%
set returnErrorCode=true
set pause=false

set sonar_runner=C:\root\bin\sonar-runner\bin\sonar-runner
set solution_folder=%CD%

@REM  ----------------------------------------------------
@REM  If the first parameter is /q, pause
@REM  at the end of execution.
@REM  ----------------------------------------------------
if /i "%1"=="/q" (
 set pause=true
 SHIFT
)

@REM  ----------------------------------------------------
@REM  If the first or second parameter is /i, do not 
@REM  return an error code on failure.
@REM  ----------------------------------------------------
if /i "%1"=="/i" (
 set returnErrorCode=false
 SHIFT
)

@REM  ------------------------------------------------
@REM  Shorten the command prompt for making the output
@REM  easier to read.
@REM  ------------------------------------------------
set savedPrompt=%prompt%
set prompt=$$$g$s

@REM -------------------------------------------------------
@REM Change to the directory where the solution file resides
@REM -------------------------------------------------------
pushd %solution_folder%
cd %solution_folder%

call %sonar_runner% -e
@if %errorlevel%  NEQ 0  goto :error

@REM  ----------------------------------------
@REM  Restore the command prompt and exit
@REM  ----------------------------------------
@goto :success

@REM  -------------------------------------------
@REM  Handle errors
@REM
@REM  Use the following after any call to exit
@REM  and return an error code when errors occur
@REM
@REM  if errorlevel 1 goto :error	
@REM  -------------------------------------------
:error
if %returnErrorCode%==false goto finish

@ECHO An error occured in build.cmd - %errorLevel%
if %pause%==true PAUSE
@exit errorLevel

:success
echo process successfully finished.
echo start time: %start_time%
echo end time: %Time%
if %pause%==true PAUSE

:finish
popd
set prompt=%savedPrompt%

ENDLOCAL
echo on