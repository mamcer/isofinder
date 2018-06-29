@echo off
SETLOCAL
@REM  ----------------------------------------------------------------------------
@REM  db.deploy.cmd
@REM
@REM  author: mario.moreno@live.com
@REM
@REM  optional arguments for this batch file:
@REM  build type: defaults to Debug
@REM  ----------------------------------------------------------------------------

echo.
echo =========================================================
echo   DB Deploy script                                          
echo =========================================================
echo.

set start_time=%time%
set returnErrorCode=true
set pause=false

set msbuild_folder="%ProgramFiles(x86)%\MSBuild\12.0\Bin"
set webdeploy_folder="%ProgramW6432%\IIS\Microsoft Web Deploy V3"
set solution_folder="..\..\Src"
set solution_name=IsoFinder.sln
set default_build_type=Release
set data_source=(localdb)\v11.0
set database=IsoFinder
set user_id=isofinder
set password=isofinder

if "%1"=="/?" goto help

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

@REM  ----------------------------------------------------
@REM  User can override default build type by specifiying
@REM  a parameter to batch file (e.g. Build Debug).
@REM  ----------------------------------------------------
if not "%1"=="" set default_build_type=%1

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

call %msbuild_folder%\msbuild /m %solution_name% /t:Rebuild /p:Configuration=%default_build_type%
@if %errorlevel%  NEQ 0  goto :error

call %webdeploy_folder%\msdeploy -verb:sync -source:dbDacFx=%CD%\Mentoring.Database\bin\Release\Mentoring.Database.dacpac -dest:dbDacFx="Data Source=%data_source%;Database=%database%;User Id=%user_id%;Password=%password%"
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

:help
echo usage: build [/q] [/i] [build type] 
echo /q, do not pause at the end of execution.
echo /i, do not return an error code on failure.
echo.
echo Examples:
echo.
echo    "build" - builds a Debug build      
echo    "build Release" - builds a Release build
echo.

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