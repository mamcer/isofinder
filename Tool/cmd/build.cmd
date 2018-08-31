@echo off
SETLOCAL
@REM  ----------------------------------------------------------------------------
@REM  build.cmd
@REM
@REM  author: m4mc3r@gmail.com
@REM
@REM  optional arguments for this bat   ch file:
@REM  build type: defaults to Debug
@REM  ----------------------------------------------------------------------------

echo.
echo =========================================================
echo   Build Script                                          
echo =========================================================
echo.

set start_time=%time%
set returnErrorCode=true
set pause=false

set msbuild_folder="%ProgramFiles(x86)%\MSBuild\14.0\Bin"
set solution_folder="%CD%\..\..\Src"
set solution_name=IsoFinder.sln
set default_build_type=Debug

if "%1"=="/?" goto help

@REM If the first parameter is /q, pause at the end of execution
if /i "%1"=="/q" (
 set pause=true
 SHIFT
)

@REM If the first or second parameter is /i, do not return an error code on failure
if /i "%1"=="/i" (
 set returnErrorCode=false
 SHIFT
)

@REM  User can override default build type by specifiying a parameter to batch file (e.g. Build Debug)
if not "%1"=="" set default_build_type=%1

@REM  Shorten the command prompt for making the output easier to read
set savedPrompt=%prompt%
set prompt=$$$g$s

@REM Change to the directory where the solution file resides
pushd %solution_folder%

call %msbuild_folder%\msbuild /m %solution_name% /t:Rebuild /p:Configuration=%default_build_type%
@if %errorlevel%  NEQ 0  goto :error

@REM  Restore the command prompt and exit
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
echo process successfully finished
echo start time: %start_time%
echo end time: %Time%
if %pause%==true PAUSE

:finish
popd
set pause=
set solution_folder=
set default_build_type=
set returnErrorCode=
set prompt=%savedPrompt%
set savedPrompt=

ENDLOCAL
echo on