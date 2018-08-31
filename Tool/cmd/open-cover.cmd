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
echo   OpenCover Script                                          
echo =========================================================
echo.

pushd %~dp0
set start_time=%time%
set msbuild_bin="%PROGRAMFILES(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
set solution_name=IsoFinder.sln
set solution_path="%CD%\..\..\Src"
set opencover_bin="..\tool\open-cover\OpenCover.Console.exe"
set mstest_bin="%PROGRAMFILES(x86)%\Microsoft Visual Studio 14.0\Common7\IDE\mstest.exe"
set opencover_proj=open-cover.proj

@REM  shorten the command prompt for making the output easier to read
set savedPrompt=%prompt%
set prompt=$$$g$s

@REM rebuild solution
cd %solution_path%
call %msbuild_bin% /m %solution_name% /t:Rebuild /p:Configuration=Debug
@if %errorlevel% NEQ 0 GOTO error

@REM run tests
call %msbuild_bin% /p:WorkingDirectory=%solution_path% /p:OpenCoverBinPath=%opencover_bin% /p:MSTestBinPath=%mstest_bin% %opencover_proj%
@if %errorlevel% NEQ 0 goto error
goto success

:error
echo an error has occurred.
GOTO finish

:success
echo process successfully finished
echo start time: %start_time%
echo end time: %Time%

:finish
popd
set prompt=%savedPrompt%

echo on