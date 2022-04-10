
rem all paths are relative to the git scripts folder

rem -- the application on the local server where this collection will be installed
set appName=menucrm0210

call build.cmd

rem upload to contensive application
c:
cd %collectionPath%
cc -a %appName% --installFile "%collectionName%.zip"
cd ..\..\scripts

pause