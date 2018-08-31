@echo off
SETLOCAL
@REM  ----------------------------------------------------------------------------
@REM  web-deploy.cmd
@REM
@REM  author: m4mc3r@gmail.com
@REM
@REM  optional arguments for this batch file:
@REM  build type: defaults to Debug
@REM  ----------------------------------------------------------------------------

echo.
echo =========================================================
echo   Web Deploy script                                          
echo =========================================================
echo.

set start_time=%time%
set returnErrorCode=true
set pause=false

set msbuild_folder="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin"
set webdeploy_folder="C:\Program Files\IIS\Microsoft Web Deploy V3"
set solution_folder="%CD%\..\..\Src"
set solution_name=IsoFinder.sln
set remote_computer=192.168.1.100
set website=IsoFinder
set user=[user-name]
set password=[password]
set param_file="%CD%\..\..\Common\Dev.Deployment.SetParameters.xml"

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

REM Generate Package
%msbuild_folder%\msbuild %solution_name% /p:Configuration=Release /p:DeployOnBuild=true /p:DeployTarget=Package /p:CreatePackageOnBuild=True
@if %errorlevel%  NEQ 0  goto :error

REM Recycle AppPool
%webdeploy_folder%\msdeploy -verb:sync -source:recycleApp -dest:recycleApp="%website%",recycleMode="RecycleAppPool",computerName=%remote_computer%
@if %errorlevel%  NEQ 0  goto :error

REM Deploy Package
Mentoring.Web\obj\Release\Package\Mentoring.Web.deploy.cmd /Y /M:%remote_computer% /U:%user% /P:%password% -allowUntrusted -setParamFile:%param_file%
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