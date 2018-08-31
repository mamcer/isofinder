@echo off
SETLOCAL
@REM  ----------------------------------------------------------------------------
@REM  open-cover.cmd
@REM
@REM  author: m4mc3r@gmail.com
@REM
@REM  ----------------------------------------------------------------------------

echo.
echo =========================================================
echo   VSTest Console Script                                          
echo =========================================================
echo.

pushd %~dp0
set start_time=%time%
set msbuild_bin=%PROGRAMFILES(X86)%\MSBuild\14.0\Bin\MSBuild.exe
set working_directory="%CD%\..\..\Src"
set vstestconsole_bin=%PROGRAMFILES(X86)%\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe
set vstestconsole_proj_path="%CD%\..\..\Src\vstest-console.proj"

@REM  Shorten the command prompt for making the output easier to read
set savedPrompt=%prompt%
set prompt=$$$g$s

"%msbuild_bin%" %vstestconsole_proj_path% /p:WorkingDirectory="%working_directory%" /p:VSTestConsoleBinPath="%vstestconsole_bin%"
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