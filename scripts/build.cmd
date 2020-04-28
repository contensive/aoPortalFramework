
@echo off

rem 
rem Must be run from the projects git\project\scripts folder - everything is relative
rem run >build [deploymentNumber]
rem deploymentNumber is YYMMDD.build-number, like 190824.5
rem
rem Setup deployment folder
rem

call env.cmd
set deploymentNumber=%1
set year=%date:~12,4%
set month=%date:~4,2%
set day=%date:~7,2%

rem
rem if deployment number not entered, set it to date.1
rem
IF [%deploymentNumber%] == [] (
	echo No deployment folder provided on the command line, use current date
	set deploymentTimeStamp=%year%%month%%day%
)
rem
rem if deployment folder exists, delete it and make directory
rem

set suffix=1
:tryagain
set deploymentNumber=%deploymentTimeStamp%.%suffix%
if not exist "%deploymentFolderRoot%%deploymentNumber%" goto :makefolder
set /a suffix=%suffix%+1
goto tryagain
:makefolder
md "%deploymentFolderRoot%%deploymentNumber%"

rem ==============================================================
rem
echo build 
rem
cd ..\server
"%msbuildLocation%msbuild.exe" %solutionName%
if errorlevel 1 (
   echo failure building
   pause
   exit /b %errorlevel%
)
cd ..\scripts

rem pause

rem ==============================================================
rem
echo Build addon collection
rem

rem remove old DLL files from the collection folder
del "%collectionPath%"\*.DLL
del "%collectionPath%"\*.config

rem copy bin folder assemblies to collection folder
copy "%binPath%*.dll" "%collectionPath%"

rem create new collection zip file
c:
cd %collectionPath%
del "%collectionName%.zip" /Q
"c:\program files\7-zip\7z.exe" a "%collectionName%.zip"
xcopy "%collectionName%.zip" "%deploymentFolderRoot%%deploymentNumber%" /Y
cd ..\..\scripts

