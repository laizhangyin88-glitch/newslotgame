#!/bin/bash

set -e
#set -x

PATH=/usr/local/bin:$PATH

UNITY_PATH="${UNITY_PATH:-/Applications/Unity/Unity.app}"
VERBOSE_MODE=""
UPDATE_STREAMING_ASSET="$HOME/Desktop"
BUNDLE_VERSION_ARG=""
IGNORE_ASSETBUNDLE_DEPENDENCY=""
SERIAL=""
USERNAME=""
PASSWORD=""

# need it?
SUB_BUILD_TARGET=""

#Set Script Name variable
SCRIPT=`basename ${BASH_SOURCE[0]}`

#Help function
function HELP {
  echo -e "usage: $SCRIPT Target"\\n
  echo "-s serial --Set the serial."
  echo "-u username --Set the username."
  echo "-p password --Set the password."
  echo "-v       --Displays Unity3d build logs in stdout."
  echo "-a path  --Build streaming asset bundle & asset bundle and copy them to 'path'."
  echo -e "-h       --Displays this help message. No further functions are performed."\\n
  echo "-g target --Canvas/Gameroom only. One of (TEAM, QA, ST, PROD)."
  echo "-b version --Set Bundle Version."
  echo "-n appName --Set appName"
  echo -e "Example: $SCRIPT -a $UPDATE_STREAMING_ASSET dev android"
  exit 1
}

case "$OSTYPE" in
  darwin*)
    echo "On OS X" ;
    UNITY_BINARY_PATH="${UNITY_PATH}/Contents/MacOS/Unity";;
  linux*)
    echo "LINUX on Windows" ;
    UNITY_BINARY_PATH="${UNITY_PATH}/Editor/Unity.exe";;
  msys*)
    echo "Windows Git-Bash" ;
    UNITY_BINARY_PATH="${UNITY_PATH}/Editor/Unity.exe";;
  *)
    echo "unknown os type from generate_assets.sh: $OSTYPE";
    exit 1;;
esac

### Start getopts code ###

#Parse command line flags
#If an option should be followed by an argument, it should be followed by a ":".
#Notice there is no ":" after "h". The leading ":" suppresses error messages from
#getopts. This is required to get my unrecognized option code to work.

while getopts :s:u:p:n:vdha:g:b:i FLAG; do
  case $FLAG in
    h)  #show help
      HELP
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
    v)
      VERBOSE_MODE="-logFile -"
      ;;
    a)
      UPDATE_STREAMING_ASSET="-assetBundlePath:"$OPTARG
      ;;
    g)
      SUB_BUILD_TARGET=$OPTARG
      ;;
    b)
      BUNDLE_VERSION_ARG="-bundleVersion:"$OPTARG
      ;;
    i)
      IGNORE_ASSETBUNDLE_DEPENDENCY="-ignoreAssetbundleDependency:true"
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

function build {
  local repoAbsPath=$1;
  local target=`echo $2 | tr '[:lower:]' '[:upper:]'`
  local conceptualPlatform=`echo $3 | tr '[:upper:]' '[:lower:]'`;

  local player='';
  local platform='';
  local additionalFlags='';

  case "$target" in
    "DEV" | "PROD")
      ;;
    *)
      echo "Unkown target: $target"
      exit -1;
      ;;
  esac

  case "$conceptualPlatform" in
    "ios")
      player="ios"
      platform="iOS"
    ;;
    "android")
      player="android"
      platform="Android"
      ;;
    "amazon")
      player="android"
      platform="Amazon"
      ;;
    "windows")
      player="wsaplayer"
      platform="Windows"
      ;;
    "gameroom")
      player="win32"
      platform="Gameroom"
      if [ "$SUB_BUILD_TARGET" != "" ]; then
        additionalFlag="-nativeBuildTarget:"$SUB_BUILD_TARGET
      fi
      ;;
    "canvas")
      player="webgl"
      platform="Canvas"
      if [ "$SUB_BUILD_TARGET" != "" ]; then
        additionalFlag="-nativeBuildTarget:"$SUB_BUILD_TARGET
      fi
      ;;
    *)
      echo "Unkown platform: $conceptualPlatform"
      exit -1;
      ;;
  esac

  set +e
  echo "Build Unity project:"
  # Need buildTarget flag: refer to 'Note' in https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html
  "$UNITY_BINARY_PATH" -quit -batchmode -nographics ${SERIAL} ${USERNAME} ${PASSWORD} -buildTarget $player $VERBOSE_MODE -executeMethod BagelCode.Builder.BuildApplication -projectPath $repoAbsPath -target:$target -platform:$platform $UPDATE_STREAMING_ASSET -skipPlayer:true $additionalFlag $BUNDLE_VERSION_ARG $IGNORE_ASSETBUNDLE_DEPENDENCY -appName:$APP_NAME
  RET=$?
  set -e

  echo "Unity build returned with ret code $RET."
  if [ $RET -ne 0 ] ; then
    echo "Unity build returned with non-zero ret code $RET."
    exit $RET
  fi
}

if [ $# -ne 2 ]; then
  echo  "Build target / platform is missing."
  HELP
  exit -1
fi

REPO_ABS_PATH=$(getRepositoryPath)

TARGET=$1
PLATFORM=$2
echo "Process $TARGET:"
build $REPO_ABS_PATH $TARGET $PLATFORM
