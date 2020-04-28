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

rem -- the application on the local server where this collection will be installed
set appName=app200402

rem -- major version 5, minor does not matter set 1
set majorVersion=5
set minorVersion=1

rem -- name of the collection on the site (should NOT include ao prefix). This is the name as it appears on the navigator
set collectionName=Portal Framework

rem -- name of the collection folder, (should NOT include ao prefix)
set collectionPath=..\collections\Portal Framework\

rem -- name of the solution. SHOULD include ao prefix
set solutionName=aoPortalFramework.sln

rem -- name of the solution. SHOULD include ao prefix
set binPath=..\server\aoPortalFramework\bin\debug\

rem -- name of the solution. SHOULD include ao prefix
set msbuildLocation=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\

rem -- name of the solution. SHOULD include ao prefix
set deploymentFolderRoot=C:\Deployments\aoPortalFramework\Dev\

