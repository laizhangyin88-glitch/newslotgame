#!/bin/bash

set -e
# set -x

PATH=/usr/local/bin:$PATH

WORKSPACE_BASE_PATH="/private/tmp/unity_build"
EXPORT_PATH=""
UNITY_PATH="${UNITY_PATH:-/Applications/Unity/Unity.app}"
VERBOSE_MODE=""
UNITY_DEV_BUILD=""
UPDATE_STREAMING_ASSET=""
NATIVE_BUILD_TARGET=""
BUNDLE_VERSION_ARG=""
IGNORE_ASSETBUNDLE_DEPENDENCY=""
SKIP_LZ4HC_COMPRESSION=""
SERIAL=""
USERNAME=""
PASSWORD=""
APP_NAME=""

#Set Script Name variable
SCRIPT=`basename ${BASH_SOURCE[0]}`

#Help function
function HELP {
  echo -e "usage: $SCRIPT Target"\\n
  echo "-w path  --Sets the workspace path. Default is $WORKSPACE_BASE_PATH. Not allowed relative path."
  echo "-o path  --Sets the generated android project path. Not allowed relative path."
  echo "-s serial --Set the serial."
  echo "-u username --Set the username."
  echo "-p password --Set the password."
  echo "-v       --Displays Unity3d build logs in stdout."
  echo "-a path  --Build streaming asset bundle & asset bundle and copy them to 'path'."
  echo "-d       --Unity development build."
  echo "-g target --Canvas build target one of (TEAM, QA, ST, PROD). invalid target defaults to ApplicationSettings"
  echo "-b version --Set Bundle Version."
  echo "-n appName --Set appName"
  echo "-i       --Ignore assetbundle dependency and proceed build process when dependency found"
  echo -e "-h       --Displays this help message. No further functions are performed."\\n
  echo -e "Example: $SCRIPT -o $EXPORT_PATH" LOCAL\\n
  exit 1
}

UNITY_PLAYBACK_ENGINES_PATH=""
case "$OSTYPE" in
  darwin*)
    echo "On OS X" ;
    UNITY_BINARY_PATH="${UNITY_PATH}/Contents/MacOS/Unity";
    UNITY_PLAYBACK_ENGINES_PATH="${UNITY_PATH}/..";;

  linux*)
    echo "LINUX on Windows" ;
    UNITY_BINARY_PATH="${UNITY_PATH}/Editor/Unity.exe";
    UNITY_PLAYBACK_ENGINES_PATH="${UNITY_PATH}/Editor/Data";;
  *)
    echo "unknown: $OSTYPE";
    exit 1;;
esac


### Start getopts code ###

#Parse command line flags
#If an option should be followed by an argument, it should be followed by a ":".
#Notice there is no ":" after "h". The leading ":" suppresses error messages from
#getopts. This is required to get my unrecognized option code to work.

while getopts :w:s:u:p:n:vdha:o:b:g:ic FLAG; do
  case $FLAG in
    w)  #set option "w"
      WORKSPACE_BASE_PATH=$OPTARG
      ;;
    s)
      SERIAL="-serial $OPTARG"
      ;;
    u)
      USERNAME="-username $OPTARG"
      ;;
    p)
      PASSWORD="-password $OPTARG"
      ;;
    n)
      APP_NAME=$OPTARG
      ;;
    h)  #show help
      HELP
      ;;
    v)
      VERBOSE_MODE="-logFile -"
      ;;
    d)
      UNITY_DEV_BUILD="-profile:true"
      ;;
    a)
      UPDATE_STREAMING_ASSET="-assetBundlePath:"$OPTARG
      ;;
    g)
      NATIVE_BUILD_TARGET=$OPTARG
      ;;
    o)
      EXPORT_PATH=$OPTARG
      ;;
    b)
      BUNDLE_VERSION_ARG="-bundleVersion:"$OPTARG
      ;;
    i)
      IGNORE_ASSETBUNDLE_DEPENDENCY="-ignoreAssetbundleDependency:true"
      ;;
    c)
      SKIP_LZ4HC_COMPRESSION="-skipLz4HCCompression:true";
      ;;
    \?) #unrecognized option - show help
      echo -e \\n"Option -$OPTARG not allowed."
      HELP
      #If you just want to display a simple error message instead of the full
      #help, remove the 2 lines above and uncomment the 2 lines below.
      #echo -e "Use $SCRIPT -h to see the help documentation."\\n
      #exit 2
      ;;
  esac
done

shift $((OPTIND-1))  #This tells getopts to move on to the next argument.

### End getopts code ###


function getScriptPath {
  local SOURCE="${BASH_SOURCE[0]}"
  local DIR=""
  while [ -h "$SOURCE" ]; do # resolve $SOURCE until the file is no longer a symlink
    DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
    SOURCE="$(readlink "$SOURCE")"
    [[ $SOURCE != /* ]] && SOURCE="$DIR/$SOURCE" # if $SOURCE was a relative symlink, we need to resolve it relative to the path where the symlink file was located
  done
  DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
  echo $DIR
}

function getRepositoryPath {
  local repositoryPath=$(getScriptPath)
  echo $(dirname $repositoryPath) 
}

function getProjectName {
  cd $(getRepositoryPath)
  PROJECT_NAME=$(grep "productName" ./Assets/Meta/Environment/${APP_NAME}/EnvironmentSettings.asset | sed 's/.*: //')
  echo $PROJECT_NAME
}

# Assumes that this script resides in repository
function getRevision {
  cd $(getScriptPath)
  REV=$(git rev-parse --short HEAD)
  echo $REV
}

function build {
  local repoAbsPath=$1
  local target=`echo $2 | tr '[:lower:]' '[:upper:]'`
  local unityWorkspacePath=$3

  case "$target" in
    "DEV" | "PROD")
      ;;
    *)
      echo "Unkown target: $target"
      exit -1;
      ;;
  esac

  echo ${USERNAME}
  echo ${PASSWORD}

  if [ -z $APP_NAME ]; then
    echo "App name is null."
    APP_NAME="SLOTS1"
  fi

  local packageName="com.bagelcode.ClubVegas"
  case "$APP_NAME" in
    "SLOTS1")
      packageName="com.bagelcode.ClubVegas"
      ;;
    "SLOTS3")
      packageName="com.bagelcode.CashBillionaire"
      ;;
    *)
      echo "Unknown APP : $APP_NAME"
      packageName="com.bagelcode.ClubVegas"
      ;;
  esac

  echo "Make app icon"
  bash "$repoAbsPath/NativeHelper/AppIconMaker/Make_IOS_Icons.sh" $APP_NAME $repoAbsPath/NativeHelper

  echo "Copy skeleton project"
  cp -r "$repoAbsPath/NativeHelper/iOS" "$unityWorkspacePath"

  echo "Copy environment info.plist"
  cp -r "$unityWorkspacePath/Environment/$APP_NAME/$NATIVE_BUILD_TARGET.plist" "$unityWorkspacePath/info.plist"
  
  echo "Copy environment firebase GoogleService-Info.plist"
  case "$NATIVE_BUILD_TARGET" in
    "prod" | "qa")
      echo "GoogleService-Info_PROD.plist"
      cp -r "$unityWorkspacePath/Environment/$APP_NAME/GoogleService-Info_PROD.plist" "$unityWorkspacePath/GoogleService-Info.plist"
      ;;
    *)
      echo "GoogleService-Info_DEV.plist"
      cp -r "$unityWorkspacePath/Environment/$APP_NAME/GoogleService-Info_DEV.plist" "$unityWorkspacePath/GoogleService-Info.plist"
      ;;
  esac

  echo "Copy Adjust Signature xcframework files"
  cp -r "$unityWorkspacePath/Environment/$APP_NAME/AdjustSigSdk.xcframework" "$unityWorkspacePath"

  echo "Copy push_sound.caf"
  cp -r "$unityWorkspacePath/Environment/$APP_NAME/push_sound.caf" "$unityWorkspacePath/push_sound.caf"

  echo "Copy old clubvegas_push_sound.caf"
  cp -r "$unityWorkspacePath/Environment/$APP_NAME/push_sound.caf" "$unityWorkspacePath/clubvegas_push_sound.caf"

  if [ "$USERNAME" -eq "" ]; then
    cp "$unityWorkspacePath/process_symbols_without_unity_account_info.sh" "$unityWorkspacePath/process_symbols.sh"
  fi
  cp -r "$UNITY_PLAYBACK_ENGINES_PATH/PlaybackEngines/iOSSupport/Trampoline/Classes" "$unityWorkspacePath/Classes"
  cp -r "$UNITY_PATH"/Contents/Tools/macosx/* "$unityWorkspacePath"

  echo "Build Unity project:"
  set +e

  # Need buildTarget flag: refer to 'Note' in https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html
  "$UNITY_BINARY_PATH" -quit -batchmode -nographics ${SERIAL} ${USERNAME} ${PASSWORD} -buildTarget ios $VERBOSE_MODE -executeMethod BagelCode.Builder.BuildApplication -projectPath $repoAbsPath -buildPath:$unityWorkspacePath $UNITY_DEV_BUILD -pkgname:$packageName -target:$target -platform:iOS $UPDATE_STREAMING_ASSET -nativeBuildTarget:$NATIVE_BUILD_TARGET $BUNDLE_VERSION_ARG $IGNORE_ASSETBUNDLE_DEPENDENCY $SKIP_LZ4HC_COMPRESSION -appName:$APP_NAME
  RET=$?
  set -e

  if [ $RET -ne 0 ] ; then
    echo "Unity build returned with non-zero ret code $RET.";
    exit $RET
  # else
  #   echo "Post process"
  #   PROJECT_NAME=$(getProjectName)
  #   echo $PROJECT_NAME
  #   echo "$unityWorkspacePath/Unity-iPhone.xcodeproj/project.pbxproj"
  #   sed -i -e "s/PRODUCT_NAME = v3proto/PRODUCT_NAME = $PROJECT_NAME/g" "$unityWorkspacePath/Unity-iPhone.xcodeproj/project.pbxproj"
  #   sed -i -e "s/v3proto.app/$PROJECT_NAME.app/g" "$unityWorkspacePath/Unity-iPhone.xcodeproj/project.pbxproj"
  #   rm "$unityWorkspacePath/Unity-iPhone.xcodeproj/project.pbxproj-e"

  #   sed -i -e "s/v3proto.app/$PROJECT_NAME.app/g" "$unityWorkspacePath/Unity-iPhone.xcodeproj/project.pbxproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme"
  #   rm "$unityWorkspacePath/Unity-iPhone.xcodeproj/project.pbxproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme-e"
  fi

  echo "Finish generating iOS project: " $unityWorkspacePath
}

if [ $# -eq 0 ]; then
  echo  "Build target is missing."
  HELP
  exit -1
fi

if [ "$EXPORT_PATH" != "" ] && [ -e "$EXPORT_PATH" ]; then
  echo "Output path already exists: $EXPORT_PATH"
  exit -1
fi

REVISION=$(getRevision)
REPO_ABS_PATH=$(getRepositoryPath)
DATE=$(date +"%Y%m%d")

BLANK_REGEX=".* .*"
if [[ "$WORKSPACE_BASE_PATH" =~ $BLANK_REGEX ]]; then
  echo "blank is not allowed in the workspace path:" $WORKSPACE_BASE_PATH
  exit -1
fi

TARGET=$1
echo "Process $TARGET:"
mkdir -p "$WORKSPACE_BASE_PATH"
WORKSPACE_PATH="$WORKSPACE_BASE_PATH/ios_${DATE}_r${REVISION}_${TARGET}"
if [ -d "$WORKSPACE_PATH" ]; then
  echo "Workspace directory already exists. Delete it."
  rm -rf "$WORKSPACE_PATH"
fi
build $REPO_ABS_PATH $TARGET $WORKSPACE_PATH
if [ "$EXPORT_PATH" != "" ]; then
  cp -a $WORKSPACE_PATH $EXPORT_PATH
fi
