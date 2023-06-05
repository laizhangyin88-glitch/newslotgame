#!/bin/bash

set -e
#set -x

PATH=/usr/local/bin:$PATH

WORKSPACE_BASE_PATH="/private/tmp/unity_build"
UNITY_PATH="${UNITY_PATH:-/Applications/Unity/Unity.app}"
VERBOSE_MODE=""
UNITY_PROFILE_ENABLE=""
IGNORE_ASSETBUNDLE_DEPENDENCY=""

#Set Script Name variable
SCRIPT=`basename ${BASH_SOURCE[0]}`

#Help function
function HELP {
  echo -e "usage: $SCRIPT [DEV|PROD]"\\n
  echo "-v       --Displays Unity3d build logs in stdout."
  echo "-p       --Unity profiling."
  echo "-i       --Ignore Assetbundle dependency."
  echo -e "-h       --Displays this help message. No further functions are performed."\\n
  exit 1
}

### Start getopts code ###

#Parse command line flags
#If an option should be followed by an argument, it should be followed by a ":".
#Notice there is no ":" after "h". The leading ":" suppresses error messages from
#getopts. This is required to get my unrecognized option code to work.

while getopts :hivp FLAG; do
  case $FLAG in
    h)  #show help
      HELP
      ;;
    i)
      IGNORE_ASSETBUNDLE_DEPENDENCY="-ignoreAssetbundleDependency:true"
      ;;
    v)
      VERBOSE_MODE="-logFile -"
      ;;
    p)
      UNITY_PROFILE_ENABLE="-profile"
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
  local scriptPath=$(getScriptPath)
  echo $(dirname $(dirname $scriptPath))
}

function getProjectName {
  cd $(getRepositoryPath)
  PROJECT_NAME=$(grep "productName" ./ProjectSettings/ProjectSettings.asset | sed 's/.*: //')
  echo $PROJECT_NAME
}

if [ $# -eq 0 ]; then
  echo  "Build target is missing."
  HELP
  exit -1
fi

REPO_ABS_PATH=$(getRepositoryPath)
PROJECT_NAME=$(getProjectName)
TARGET_PATH="$(getScriptPath)/app/unity";
if [ -d "$TARGET_PATH" ]; then
  echo "Output directory already exits. Delete it.";
  rm -rf $TARGET_PATH
fi

TARGET=$1
echo "Process $TARGET:"
WORKSPACE_PATH=$(mktemp -d /tmp/generate_unity_asset.XXXXXX)

set +e
echo "Build Unity project:"
# Need buildTarget flag: refer to 'Note' in https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html
"$UNITY_PATH"/Contents/MacOS/Unity -quit -batchmode -buildTarget android $VERBOSE_MODE -executeMethod BagelCode.Builder.BuildApplication -projectPath $REPO_ABS_PATH -buildPath:$WORKSPACE_PATH $UNITY_PROFILE_ENABLE -pkgname:com.bagelcode.v3test -target:$TARGET -platform:Amazon -assetBundlePath:$WORKSPACE_PATH $IGNORE_ASSETBUNDLE_DEPENDENCY
RET=$?
set -e

echo "Unity build returned with ret code $RET."
if [ $RET -ne 0 ] ; then
    echo "Unity build returned with non-zero ret code $RET."
    exit $RET
fi

# mkdir -p "$TARGET_PATH"
mv "$WORKSPACE_PATH/$PROJECT_NAME" "$TARGET_PATH"

echo "Unity assets are generated successfully. Go ahead!"
echo "You may find assetbundles from $WORKSPACE_PATH/Amazon"
