
rem all paths are relative to the git scripts folder

call Env.cmd

@echo off
cls
echo create deployment for %collectionName%
echo upload to %appName%

call BuildCollection.cmd

rem upload to contensive application
c:
cd %collectionPath%
cc -a %appName% --installFile "%collectionName%.zip"
cd ..\..\scripts

pause