@echo on

SET curdir=%~dp0
SET projectPath=%curdir%..\..
SET assetPath=d:\assetFB

"c:\Program Files\Unity_2017.1.1p3\Editor\Unity.exe" -quit -batchmode -buildTarget webgl -logFile webGLBuild.log -executeMethod BagelCode.Builder.BuildApplication -projectPath %projectPath% -buildPath:%curdir%/canvasBuildOut -pkgName:com.bagelcode.v3test -target:DEV -platform:Canvas -assetBundlePath:%assetPath% -develop:true -canvasBuildTarget:TEAM
