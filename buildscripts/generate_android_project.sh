#!/bin/bash

set -e
#set -x

PATH=/usr/local/bin:$PATH

WORKSPACE_BASE_PATH="/private/tmp/unity_build"
EXPORT_PATH="$HOME/Desktop"
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

case "$OSTYPE" in
  darwin*)
    echo "On OS X" ;
    UNITY_BINARY_PATH="${UNITY_PATH}/Contents/MacOS/Unity";;
  linux*)
    echo "LINUX on Windows" ;
    UNITY_BINARY_PATH="${UNITY_PATH}/Editor/Unity.exe";;
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
    o)
      EXPORT_PATH=$OPTARG
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

# Assumes that this script resides in repository
function getRevision {
  cd $(getScriptPath)
  REV=$(git rev-parse --short HEAD)
  echo $REV
}

function getDeepLinkScheme {
  cd $(getRepositoryPath)
  PROJECT_NAME=$(grep "deeplinkUriScheme" ./Assets/Meta/Environment/${APP_NAME}/EnvironmentSettings.asset | sed 's/.*: //')
  echo $PROJECT_NAME
}

function build {
  local repoAbsPath=$1;
  local target=`echo $2 | tr '[:lower:]' '[:upper:]'`
  local workspacePath=$3;

  case "$target" in
    "DEV" | "PROD")
      ;;
    *)
      echo "Unkown target: $target"
      exit -1;
      ;;
  esac

  if [ -z $APP_NAME ]; then
    echo "App name is null."
    APP_NAME="SLOTS1"
  fi

  local packageName="com.bagelcode.clubvegas"
  case "$APP_NAME" in
    "SLOTS1")
      packageName="com.bagelcode.clubvegas"
      ;;
    "SLOTS3")
      packageName="com.bagelcode.CashBillionaire"
      ;;
    "SLOTS4")
      packageName="com.bagelcode.cashworld"
      ;;
    *)
      echo "Unknown APP : $APP_NAME"
      packageName="com.bagelcode.clubvegas"
      ;;
  esac

  local APP_NAME_LOWER=`echo $APP_NAME | tr '[:upper:]' '[:lower:]'`
  echo "App name lower $APP_NAME_LOWER"

  set +e
  echo "Build Unity project:"
  # Need buildTarget flag: refer to 'Note' in https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html
  "$UNITY_BINARY_PATH" -quit -batchmode -nographics ${SERIAL} ${USERNAME} ${PASSWORD} -buildTarget android $VERBOSE_MODE -executeMethod BagelCode.Builder.BuildApplication -projectPath $repoAbsPath -buildPath:$workspacePath $UNITY_DEV_BUILD -pkgname:$packageName -target:$target -platform:Android $UPDATE_STREAMING_ASSET -nativeBuildTarget:$NATIVE_BUILD_TARGET $BUNDLE_VERSION_ARG $IGNORE_ASSETBUNDLE_DEPENDENCY $SKIP_LZ4HC_COMPRESSION -appName:$APP_NAME
  RET=$?
  set -e

  echo "Unity build returned with ret code $RET."
  if [ $RET -ne 0 ] ; then
    echo "Unity build returned with non-zero ret code $RET."
    exit $RET
  fi

  echo "Convert to Gradle project:"
  # rsync -ar "$repoAbsPath/NativeHelper/Android/" "$workspacePath/"
  pushd $repoAbsPath
  pushd NativeHelper/Android
  {
      git checkout-index -a --prefix="$workspacePath"/
  }
  popd

  find "$workspacePath/NativeHelper/Android/" -mindepth 1 -maxdepth 1 -exec mv {} "$workspacePath" \;

  echo "Make app icon"
  bash "$repoAbsPath/NativeHelper/AppIconMaker/Make_Android_Icons.sh" $APP_NAME $repoAbsPath/NativeHelper $workspacePath

  echo "copy push sound"
  cp "$workspacePath/Environment/$APP_NAME/push_sound.mp3" "$workspacePath/app/src/main/res/raw/push_sound.mp3"

  echo "Copy environment firebase google-services.json -> google-services.json"
  cp "$workspacePath/Environment/$APP_NAME/google-services.json" "$workspacePath/app/google-services.json"

  echo "Copy environment release.keystore"
  cp "$workspacePath/Environment/$APP_NAME/release.keystore" "$workspacePath/app/release.keystore"

  echo "Copy Adjust Signature V2 aar file"
  cp "$workspacePath/Environment/$APP_NAME/adjust_sig_v2_v2_8_2.aar" "$workspacePath/app/libs/adjust_sig_v2_lib.aar"

  echo "Set deeplink scheme"
  DEEP_LINK_SCHEME=$(getDeepLinkScheme)
  echo $DEEP_LINK_SCHEME
  sed -i -e "s/<data android:scheme=\"custom_app_scheme\"/<data android:scheme=\"$DEEP_LINK_SCHEME\"/g" "$workspacePath/app/src/main/AndroidManifest.xml"

  rmdir "$workspacePath/NativeHelper/Android"
  rmdir "$workspacePath/NativeHelper"
  popd

  pushd "$workspacePath"
  if [ -d "app/unity" ]; then
    rm -rf app/unity
  fi
  mv "unityLibrary" app/unity
  popd
}

if [ $# -eq 0 ]; then
  echo  "Build target is missing."
  HELP
  exit -1
fi

# if using local_build_android.sh, there is a skip option.
if [ -e "skipAndroidBuild" ]; then
  echo -e "\x1b[34mOnly Native build is in progress. If ther is a problem, please proceed with full build or delete NATIVE_PROJECT folder.\x1b[0m"
else
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
  WORKSPACE_PATH="$WORKSPACE_BASE_PATH/android_${DATE}_r${REVISION}_${TARGET}"
  if [ -d "$WORKSPACE_PATH" ]; then
    echo "Output directory already exists. Delete it."
    rm -rf "$WORKSPACE_PATH"
  fi
  mkdir -p "$WORKSPACE_PATH"
  build $REPO_ABS_PATH $TARGET $WORKSPACE_PATH
  if [ "$EXPORT_PATH" != "" ]; then
    cp -a $WORKSPACE_PATH $EXPORT_PATH
  fi
fi

