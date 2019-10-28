
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
rem remove project folders
rem
rd /S /Q "..\source\%projectName%\bin"
rd /S /Q "..\source\%projectName%\obj"

rem ==============================================================
rem
echo build 
rem
cd ..\source
"%msbuildLocation%msbuild.exe" "%solutionName%.sln"
if errorlevel 1 (
   echo failure building
   pause
   exit /b %errorlevel%
)
cd ..\scripts

rem ==============================================================
rem
echo build Nuget
rem
cd ..\source\%solutionName%

IF EXIST "Contensive.%nugetName%.%majorVersion%.%minorVersion%.%deploymentNumber%.nupkg" (
	del "Contensive.%nugetName%.%majorVersion%.%minorVersion%.%deploymentNumber%.nupkg" /Q
)
"nuget.exe" pack "Contensive.%nugetName%.nuspec" -version %majorVersion%.%minorVersion%.%deploymentNumber%

if errorlevel 1 (
   echo failure in nuget
   pause
   exit /b %errorlevel%
)
xcopy "Contensive.%nugetName%.%majorVersion%.%minorVersion%.%deploymentNumber%.nupkg" "C:\Users\jay\Documents\nugetLocalPackages" /Y
xcopy "Contensive.%nugetName%.%majorVersion%.%minorVersion%.%deploymentNumber%.nupkg" "%deploymentFolderRoot%%deploymentNumber%" /Y
cd ..\..\scripts

rem ==============================================================
rem
echo Build addon collection
rem

rem copy bin folder assemblies to collection folder
copy "%binPath%*.dll" "%collectionPath%"

rem create new collection zip file
c:
cd %collectionPath%
del "%collectionName%.zip" /Q
"c:\program files\7-zip\7z.exe" a "%collectionName%.zip"
xcopy "%collectionName%.zip" "%deploymentFolderRoot%%deploymentNumber%" /Y
cd ..\..\scripts

@echo Success
pause