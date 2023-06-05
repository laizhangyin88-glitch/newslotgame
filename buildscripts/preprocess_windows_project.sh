#!/bin/bash

set -e

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

function getWindowsStoreIdentityPackageName {
  local nativeBuildTarget=$1;
  local appName=$2;
  case "$appName" in
    "SLOTS1")
      case "$nativeBuildTarget" in
        "team")
          echo "BagelcodeInc.SpinitVegas"
          ;;
        "team_contents")
          echo "BagelcodeInc.SpinitVegas"
          ;;
        "qa")
          echo "BagelcodeInc.ClubVegas-QA"
          ;;
        "prod")
          echo "BagelcodeInc.47921C88A920C"
          ;;
        *)
          echo ""
          ;;
      esac
      ;;
    "SLOTS3")
      case "$nativeBuildTarget" in
        "team")
          echo "BagelcodeInc.SpinitVegas"
          ;;
        "team_contents")
          echo "BagelcodeInc.SpinitVegas"
          ;;
        "qa")
          echo "BagelcodeInc.ClubVegas-QA"
          ;;
        "prod")
          echo "BagelcodeInc.413C6E9F6A7BE"
          ;;
        *)
          echo ""
          ;;
      esac
      ;;
    *)
      echo "Unknown APP : $appName"
      packageName=""
      ;;
  esac
}

function getWindowsStorePackageAppName {
  local nativeBuildTarget=$1;
  case "$nativeBuildTarget" in
    "team")
      echo " - Team"
      ;;
    "team_contents")
      echo " - Team"
      ;;
    "qa")
      echo " - QA"
      ;;
    "prod")
      echo ""
      ;;
    *)
      echo " - unknown"
      ;;
  esac
}

NATIVE_PROJECT=$1
NATIVE_BUILD_TARGET=$2
APP_NAME=$3
CLIENT_VERSION_STRING=$4

REPO_ABS_PATH=$(getRepositoryPath)
PRODUCT_NAME=$(getProjectName)
LOWERCASE_NATIVE_BUILD_TARGET=$(echo "${NATIVE_BUILD_TARGET}" | tr '[:upper:]' '[:lower:]'| cut -d : -f2)

if [ -z $APP_NAME ]; then
  echo "App name is null."
  APP_NAME="SLOTS1"
fi

if [ -z $CLIENT_VERSION_STRING ]; then
  echo "CLIENT_VERSION_STRING is null."
  CLIENT_VERSION_STRING="`cat \"$REPO_ABS_PATH/Assets/Meta/Resources/ApplicationSettings.asset\" | grep 'clientVersion: ' | cut -d ':' -f2 | xargs`"
fi

echo "Replace default Resources.resw with build target corresponding one"
cp "$REPO_ABS_PATH/NativeHelper/Windows/${APP_NAME}/Resources_${LOWERCASE_NATIVE_BUILD_TARGET}.resw" "$NATIVE_PROJECT/$PRODUCT_NAME/Resources.resw"

echo "Replace icon resource"
cp -r "$REPO_ABS_PATH/NativeHelper/Windows/${APP_NAME}/Assets" "$NATIVE_PROJECT/$PRODUCT_NAME"

echo "Replace version information at Packcage.appxmanifest"
# VS_CLIENT_VERSION="`cat \"$REPO_ABS_PATH/Assets/Meta/Resources/ProductSettings.asset\" | grep 'productVersion: ' | cut -d ':' -f2 | xargs`".0
VS_CLIENT_VERSION=$CLIENT_VERSION_STRING'.0'
echo $VS_CLIENT_VERSION
sed -i -e "s/Identity\(.*\)Version=\"[0-9]*.[0-9]*.[0-9]*.[0-9]*\"/Identity\\1Version=\"$VS_CLIENT_VERSION\"/" "$NATIVE_PROJECT/$PRODUCT_NAME/Package.appxmanifest"

echo "Replace package name at Packcage.appxmanifest"
MICROSOFT_STORE_PACKAGE_NAME=$(getWindowsStoreIdentityPackageName $LOWERCASE_NATIVE_BUILD_TARGET $APP_NAME)
echo $MICROSOFT_STORE_PACKAGE_NAME
sed -i -e "s/Identity\(.*\)Name=\"[^\"]*\"/Identity\\1Name=\"$MICROSOFT_STORE_PACKAGE_NAME\"/" "$NATIVE_PROJECT/$PRODUCT_NAME/Package.appxmanifest"

echo "Replace package app name at Packcage.appxmanifest"
PACKAGE_APP_NAME=$(getProjectName)$(getWindowsStorePackageAppName $LOWERCASE_NATIVE_BUILD_TARGET)
echo $PACKAGE_APP_NAME
sed -i -e "s/<DisplayName>.*<\\/DisplayName>/<DisplayName>$PACKAGE_APP_NAME<\\/DisplayName>/" "$NATIVE_PROJECT/$PRODUCT_NAME/Package.appxmanifest"
sed -i -e "s/uap:VisualElements\(.*\)DisplayName=\"[^\"]*\"/uap:VisualElements\\1DisplayName=\"$PACKAGE_APP_NAME\"/" "$NATIVE_PROJECT/$PRODUCT_NAME/Package.appxmanifest"
