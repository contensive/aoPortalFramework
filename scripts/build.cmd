rem 
rem Must be run from the projects git\project\scripts folder - everything is relative
rem run >build [versionNumber]
rem versionNumber is YY.MM.DD.build-number, like 20.7.7.1
rem

c:
cd \Git\aoAdminFramework\scripts

rem all paths are relative to the git scripts folder
rem
rem GIT folder
rem     -- aoSample
rem			-- collection
rem				-- Sample
rem					unzipped collection files, must include one .xml file describing the collection
rem			-- server 
rem 			(all files related to server code)
rem				-- aoSample (visual studio project folder)
rem			-- ui 
rem				(all files related to the ui
rem			-- etc 
rem				(all misc files)

rem -- name of the collection on the site (should NOT include ao prefix). This is the name as it appears on the navigator
set collectionName=Portal Framework

rem -- name of the collection folder, (should NOT include ao prefix)
set collectionPath=..\collections\Portal Framework\

rem -- name of the solution. SHOULD include ao prefix
set solutionName=aoPortalFramework.sln

rem -- path to the compiled project files
set binPath=..\server\aoPortalFramework\bin\debug\net472\

rem -- path to msbuild
set msbuildLocation=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\

rem -- deployment folder, where final collection and created nuget packages will be copied
set deploymentFolderRoot=C:\Deployments\aoPortalFramework\Dev\

rem -- folder where nuget packages are copied
set NuGetLocalPackagesFolder=C:\NuGetLocalPackages\

rem @echo off
rem Setup deployment folder
set year=%date:~12,4%
set month=%date:~4,2%
if %month% GEQ 10 goto monthOk
set month=%date:~5,1%
:monthOk
set day=%date:~7,2%
if %day% GEQ 10 goto dayOk
set day=%date:~8,1%
:dayOk
set versionMajor=%year%
set versionMinor=%month%
set versionBuild=%day%
set versionRevision=1
rem
rem if deployment folder exists, delete it and make directory
rem
:tryagain
set versionNumber=%versionMajor%.%versionMinor%.%versionBuild%.%versionRevision%
if not exist "%deploymentFolderRoot%%versionNumber%" goto :makefolder
set /a versionRevision=%versionRevision%+1
goto tryagain
:makefolder
md "%deploymentFolderRoot%%versionNumber%"

rem ==============================================================
rem
echo copy resources into collection folder 
rem

cd "..\collections\Portal Framework"
copy ..\..\ui\*.html .
copy ..\..\ui\*.css .
copy ..\..\ui\*.js .

cd ..\..\scripts

rem ==============================================================
rem
echo build 
rem
cd ..\server

dotnet build aoPortalFramework/aoPortalFramework.csproj --configuration Debug --no-dependencies /property:Version=%versionNumber% /property:AssemblyVersion=%versionNumber% /property:FileVersion=%versionNumber%
if errorlevel 1 (
   echo failure building aoPortalFramework
   pause
   exit /b %errorlevel%
)

rem pause

rem "%msbuildLocation%msbuild.exe" %solutionName%
rem if errorlevel 1 (
rem    echo failure building
rem    pause
rem    exit /b %errorlevel%
rem )
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
xcopy "%collectionName%.zip" "%deploymentFolderRoot%%versionNumber%" /Y
cd ..\..\scripts

rem pause

rem ==============================================================
rem
echo build api Nuget
rem
cd ..\server\aoPortalFramework
IF EXIST "*.nupkg" (
	del "*.nupkg" /Q
)
"nuget.exe" pack "Contensive.PortalApi.nuspec" -version %versionNumber%
if errorlevel 1 (
   echo failure in nuget
   pause
   exit /b %errorlevel%
)
xcopy "Contensive.PortalApi.%versionNumber%.nupkg" "%NuGetLocalPackagesFolder%" /Y
xcopy "Contensive.PortalApi.%versionNumber%.nupkg" "%deploymentFolderRoot%%versionNumber%" /Y
cd ..\..\scripts

rem pause

rem ==============================================================
rem
echo clean collection folder (only xml should remain)
rem

cd "..\collections\Portal Framework"
del *.html
del *.css
del *.js
del *.dll
rem -- leave collection zip so it can be installed -- del *.zip

cd ..\..\scripts

rem pause

