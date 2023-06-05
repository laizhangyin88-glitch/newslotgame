#! /bin/bash

set -e
SCRIPT_PATH="$( cd "$(dirname "$0")" ; pwd -P )"
pushd "$SCRIPT_PATH/.."

# result export path
ASSETBUNDLE="ASSETBUNDLE"
WORKSPACE="WORKSPACE"
NATIVE_PROJECT="NATIVE_PROJECT"

# version info
GIT_COMMIT="`git rev-parse HEAD`"

# build options
PROFILE_OPT="" # set "-d" if you want profiling
IGNORE_ASSETBUNDLE_DEPENDENCY="-i"
SKIP_LZ4HC_COMPRESSION="-c"
UNITY_BUILD_TARGET="DEV"

# sdk. ndk path (for both Unity & Android build)
export ANDROID_HOME="${ANDROID_HOME:-`realpath ~/Library/Android/sdk`}"
export ANDROID_NDK_HOME="${ANDROID_NDK_HOME:-`realpath ~/Documents/android-ndk`}"

# dependency check
if [ ! -d "$ANDROID_HOME" ]; then
  echo -e "\x1b[31mERROR\x1b[0m: Android SDK is not installed"
  exit 1
fi
if [ ! -d "$ANDROID_NDK_HOME" ]; then
  echo -e "\x1b[31mERROR\x1b[0m: Android NDK is not installed"
  exit 1
fi

function login1Password {
  if [ "$OP_SESSION_bagelcode" == "" ]; then
    set +e
    cat ~/.op/config | grep '"shorthand": "bagelcode"' > /dev/null
    RET=$?
    set -e
    if [ "$RET" == "0" ]; then
      LOGIN_RESULT="`~/1password-cli/op signin bagelcode`"
      if [ $? -ne 0 ]; then
        echo -e "\x1b[31mERROR\x1b[0m: Failed to login 1Password"
        exit 1
      fi
      eval $LOGIN_RESULT
    else
      echo -e "\x1b[34m1Password Account (e.g. company@bagelcode.com)\x1b[0m: \c"
      read op_account
      eval $(~/1password-cli/op signin bagelcode.1password.com "$op_account")
    fi
  fi
}

# Build Stage: Unity
# skip build
if [ -d "$NATIVE_PROJECT" ]; then
echo -e "\x1b[34mNATIVE_PROJECT Folder already exist. skip generate_android_project.sh (Native build only)?(y/n)\x1b[0m: \c"
while read -r -t 0; do read -r; done
read is_skip
fi

rm -f "skipAndroidBuild"
if [ "$is_skip" == "y" ]; then
    echo "skip bash generate_android_project.sh"
    touch "skipAndroidBuild"
else
    # clean build results
    rm -rf "$ASSETBUNDLE" "$WORKSPACE" "$NATIVE_PROJECT"
fi

# Build Stage: Unity
bash "$SCRIPT_PATH/generate_android_project.sh" -a "$ASSETBUNDLE" -v -w "$WORKSPACE" -o "$NATIVE_PROJECT" -b $GIT_COMMIT $PROFILE_OPT $IGNORE_ASSETBUNDLE_DEPENDENCY $SKIP_LZ4HC_COMPRESSION $UNITY_BUILD_TARGET
rm -f "skipAndroidBuild"
pushd "$NATIVE_PROJECT"

# Build Stage: Android
/usr/local/bin/fastlane build_team
find app/build -name *.apk -exec echo -e RESULT: $(realpath {}) \;
popd

# Build Stage: Upload Assetbundle
echo -e "\x1b[34mUpload Assetbundle? (y/n)\x1b[0m: \c"
while read -r -t 0; do read -r; done
read upload
if [ "$upload" == "y" ]; then
  login1Password
  X_BYPASS_KEY="`~/1password-cli/op get item cwrp7zj5crgafhvya2lbmi327y | jq '.details.password'| sed -e 's/^\"//g' -e 's/\"$//g'`"
  CLIENT_VERSION="`cat 'Assets/Meta/Resources/ApplicationSettings.asset' | grep 'clientVersion:' | cut -d ':' -f2 | xargs | sed -e 's/\\./ /g'`"
  CLIENT_VERSION_NUMBER="`printf '%02d%02d%02d' $CLIENT_VERSION`"
  ASSETBUNDLE_UPLOAD_URL="https://admin-team.slots1.bagelgames.com:21001/admin_api/client_asset_setting/upsert/ANDROID/$CLIENT_VERSION_NUMBER"
  ulimit -n 2048
  python "$SCRIPT_PATH/upload_assetbundle.py" "$ASSETBUNDLE" "$ASSETBUNDLE_UPLOAD_URL" "$X_BYPASS_KEY" "Local_`logname`" "$GIT_COMMIT" "false"
fi

popd
set +e
