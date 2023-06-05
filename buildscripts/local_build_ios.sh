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

# dependency check
if [ ! -d "/Applications/Xcode.app" ]; then
  echo "Xcode is not installed"
fi

# credential for code signing
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

if [ "$MATCH_PASSWORD" == "" ]; then
  echo -e "\x1b[37mNOTE\x1b[0m: FASTLANE_PASSWORD environment variable is not set. try get from 1Password"
  login1Password
  MATCH_PASSWORD="`~/1password-cli/op get item yfhtax7frraxpc4jf27qcakoje | jq '.details.password' | sed -e 's/^\"//g' -e 's/\"$//g'`"
fi
if [ "$FASTLANE_PASSWORD" == "" ]; then
  echo -e "\x1b[37mNOTE\x1b[0m: MATCH_PASSWORD environment variable is not set. try get from 1Password"
  login1Password
  FASTLANE_PASSWORD="`~/1password-cli/op get item 2fvtrqbrgrhbrefxiiq3kksm4a | jq '.details.fields[] | select(.designation=="password").value' | sed -e 's/^\"//g' -e 's/\"$//g'`"
fi
if [ ! -e "$SCRIPT_PATH/id_rsa" ]; then
  echo -e "\x1b[37mNOTE\x1b[0m: git ssh key is not downloaded yet. try get from 1Password"
  login1Password
  set +e
  ~/1password-cli/op get document qyogdwojebc5zjt5uisyyl432u > "$SCRIPT_PATH/id_rsa"
  RET=$?
  set -e
  if [ "$RET" == "0" ]; then
    chmod 400 "$SCRIPT_PATH/id_rsa"
    ssh-add "$SCRIPT_PATH/id_rsa"
  else
    echo -e "\x1b[31mWARNING\x1b[0m: Failed to get ssh key. If your default ssh key is authroized to match repository, iOS build will fail."
  fi
else
  chmod 400 "$SCRIPT_PATH/id_rsa"
  ssh-add "$SCRIPT_PATH/id_rsa"
fi
echo -e "\x1b[37mNOTE\x1b[0m: update Spaceauth cookie with Beta build machine's cookie"
set +e
mkdir -p ~/.fastlane/spaceship
rm -rf ~/.fastlane/spaceship/company\@bagelcode.com
scp -i "$SCRIPT_PATH/id_rsa" -r bagelcode@172.16.80.49:~/.fastlane/spaceship/company\@bagelcode.com ~/.fastlane/spaceship/
if [ $? -ne 0 ]; then
  echo -e "\x1b[31mWARNING\x1b[0m: Failed to get Spaceauth cookie. You will be asked for 6 digits code for 2FA"
fi
set -e

export MATCH_PASSWORD="$MATCH_PASSWORD"
export FASTLANE_PASSWORD="$FASTLANE_PASSWORD"

# clean build results
rm -rf "$ASSETBUNDLE" "$WORKSPACE" "$NATIVE_PROJECT"

# Build Stage: Unity
bash "$SCRIPT_PATH/generate_ios_project.sh" -a "$ASSETBUNDLE" -v -w "$WORKSPACE" -o "$NATIVE_PROJECT" -b $GIT_COMMIT $PROFILE_OPT $IGNORE_ASSETBUNDLE_DEPENDENCY $SKIP_LZ4HC_COMPRESSION $UNITY_BUILD_TARGET

# Build Stage: iOS
pushd "$NATIVE_PROJECT"
# TODO: set OUTPUT_NAME and ADHOC_BASE_URL env variables
/usr/local/bin/fastlane build_team
find output/ -name *.ipa -exec echo -e RESULT: $(realpath {}) \;
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
  ASSETBUNDLE_UPLOAD_URL="https://admin-team.slots1.bagelgames.com:21001/admin_api/client_asset_setting/upsert/IOS/$CLIENT_VERSION_NUMBER"
  ulimit -n 2048
  python "$SCRIPT_PATH/upload_assetbundle.py" "$ASSETBUNDLE" "$ASSETBUNDLE_UPLOAD_URL" "$X_BYPASS_KEY" "Local_`logname`" "$GIT_COMMIT" "false"
fi

popd
set +e
