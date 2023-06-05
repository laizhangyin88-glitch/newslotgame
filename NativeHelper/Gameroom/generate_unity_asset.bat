@echo on

SET curdir=%~dp0
SET projectPath=%curdir%..\..
SET assetPath=d:\assetFB

"c:\Program Files\Unity_2017.1.1p3\Editor\Unity.exe" -quit -batchmode -buildTarget win32 -logFile fuckwinFb.log -executeMethod BagelCode.Builder.BuildApplication -projectPath %projectPath% -buildPath:%curdir%ClubVegas.exe -pkgName:com.bagelcode.v3test -target:DEV -platform:Gameroom -assetBundlePath:%assetPath% -develop:true -gameroomBuildTarget:TEAM
