#! /bin/bash

set -e

# Utility functions
function vercomp () {
  if [[ $1 == $2 ]]
  then
    return 0
  fi
  local IFS=.
  local i ver1=($1) ver2=($2)
  # fill empty fields in ver1 with zeros
  for ((i=${#ver1[@]}; i<${#ver2[@]}; i++))
  do
    ver1[i]=0
  done
  for ((i=0; i<${#ver1[@]}; i++))
  do
    if [[ -z ${ver2[i]} ]]
    then
      # fill empty fields in ver2 with zeros
      ver2[i]=0
    fi
    if ((10#${ver1[i]} > 10#${ver2[i]}))
    then
      return 1
    fi
    if ((10#${ver1[i]} < 10#${ver2[i]}))
    then
      return 2
    fi
  done
  return 0
}

# TODO: install Unity
if [ ! -d "/Applications/Unity" ] ; then
  echo -e "\x1b[31mERROR\x1b[0m: Automatic Unity install is not supported yet. Please install Unity manually."
  exit 1
else
  echo -e "\x1b[36mSKIP\x1b[0m: Unity is already installed."
fi
echo -e "\x1b[32mDONE\x1b[0m: Unity"

# TODO: install Xcode
if [ ! -d "/Applications/Xcode.app" ]; then
  echo -e "\x1b[31mERROR\x1b[0m: Automatic Xcode install is not supported yet. Please install Xcode from App Store manually."
  exit 1
else
  XCODE_VERSION="$(cat /Applications/Xcode.app/Contents/version.plist | grep CFBundleShortVersionString -A1 | grep '<string>' | sed -e 's/<string>\(.*\)<\/string>/\1/' | xargs)"
  echo -e "\x1b[36mSKIP\x1b[0m: Xcode(version: $XCODE_VERSION) is already installed."
  TARGET_XCODE_VERSION="10.1"
  set +e
  vercomp $XCODE_VERSION $TARGET_XCODE_VERSION
  COMPARISON_RESULT=$?
  set -e
  if [ "$COMPARISON_RESULT" -eq "2" ]; then
    echo -e "\x1b[31mWARNING\x1b[0m: current Xcode version is lower than target version($TARGET_XCODE_VERSION). Please update."
  fi
fi
echo -e "\x1b[32mDONE\x1b[0m: Xcode"

# TODO: install JDK
set +e
JAVAC_VERSION="$(javac -version 2>&1)"
RET=$?
set -e
if [ $RET -ne 0 ]; then
  echo -e "\x1b[31mERROR\x1b[0m: Automatic JDK install is not supported yet. Please follow dialogue box and install JDK from Orcale webpage."
  exit 1
else
  echo -e "\x1b[36mSKIP\x1b[0m: JDK(version: $JAVAC_VERSION) is already installed."
fi
echo -e "\x1b[32mDONE\x1b[0m: JDK"

# install Android SDK
if [ ! -d ~/Library/Android/sdk ]; then
  echo -e "\x1b[34mINSTALL\x1b[0m: Android SDK is not installed."
  curl "https://dl.google.com/android/repository/sdk-tools-darwin-4333796.zip" -o sdk-tools.zip
  unzip sdk-tools.zip -d ~/Library/Android/sdk
  yes | ~/Library/Android/sdk/tools/bin/sdkmanager "platform-tools"
  yes | ~/Library/Android/sdk/tools/bin/sdkmanager "platforms;android–28"
  yes | ~/Library/Android/sdk/tools/bin/sdkmanager "build-tools;28.0.2"
  rm sdk-tools.zip
else
  echo -e "\x1b[36mSKIP\x1b[0m: Android SDK is already installed."
fi
echo -e "\x1b[32mDONE\x1b[0m: Android SDK"

# install Android NDK
TARGET_NDK_VERSION="r16b"
if [ ! -d ~/Documents/android-ndk-$TARGET_NDK_VERSION ]; then
  echo -e "\x1b[34mINSTALL\x1b[0m: Android NDK(version: $TARGET_NDK_VERSION) is not installed."
  curl https://dl.google.com/android/repository/android-ndk-$TARGET_NDK_VERSION-darwin-x86_64.zip?hl=ko -o android-ndk-$TARGET_NDK_VERSION.zip
  unzip android-ndk-$TARGET_NDK_VERSION.zip -d ~/Documents
  ln -sf ~/Documents/android-ndk-$TARGET_NDK_VERSION ~/Documents/android-ndk
  rm android-ndk-$TARGET_NDK_VERSION.zip
else
  echo -e "\x1b[36mSKIP\x1b[0m: Android NDK(version: $TARGET_NDK_VERSION) is already installed."
fi
echo -e "\x1b[32mDONE\x1b[0m: Android NDK"

# install command line tools
set +e
xcode-select --install
if [ $? -ne 0 ]; then
  echo -e "\x1b[37mNOTE\x1b[0m: Command Line Tools is already installed or installation failed"
fi
set -e
echo -e "\x1b[32mDONE\x1b[0m: Command Line Tools"

# install brew
set +e
which brew
RET=$?
set -e
if [ $RET -ne 0 ];  then
  /usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"    
else
  echo -e "\x1b[36mSKIP\x1b[0m: Brew is already installed."
fi
echo -e "\x1b[32mDONE\x1b[0m: Brew"

# install fastlane
if [ ! -d ~/.fastlane ]; then
  echo -e "\x1b[34mINSTALL\x1b[0m: Fastlane is not installed."
  brew update
  brew cask install fastlane
else
  echo -e "\x1b[36mSKIP\x1b[0m: Fastlane is already installed."
fi
echo -e "\x1b[32mDONE\x1b[0m: Fastlane"

# install command-line 1password
if [ ! -d ~/1password-cli ]; then
  echo -e "\x1b[34mINSTALL\x1b[0m: command-line 1Password is not installed."
  curl https://cache.agilebits.com/dist/1P/op/pkg/v0.7.1/op_darwin_amd64_v0.7.1.zip -o 1password-cli.zip
  unzip 1password-cli.zip -d ~/1password-cli
  rm 1password-cli.zip
else
  echo -e "\x1b[36mSKIP\x1b[0m: command-line 1Password is already installed."
fi
echo -e "\x1b[32mDONE\x1b[0m: command-line 1Password"

# install jq
set +e
which jq
RET=$?
set -e
if [ $RET -ne 0 ]; then
  echo -e "\x1b[34mINSTALL\x1b[0m: jq is not installed."
  brew install jq
else
  echo -e "\x1b[36mSKIP\x1b[0m: jq is already installed."
fi
echo -e "\x1b[32mDONE\x1b[0m: jq"

# install python requests library
set +e
python -c "import requests"
RET=$?
set -e
if [ $RET -ne 0 ]; then
  echo -e "\x1b[34mINSTALL\x1b[0m: python requests library is not installed."
  sudo python -m ensurepip
  sudo pip install requests
else
  echo -e "\x1b[36mSKIP\x1b[0m: python requests library is already installed."
fi
echo -e "\x1b[32mDONE\x1b[0m: python requests library"


echo -e "\x1b[35mFINISH\x1b[0m: build dependency installs are complete"
set +e
