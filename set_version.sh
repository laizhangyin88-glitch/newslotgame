#!/bin/bash
ROOT_PATH=$(pwd)
MAJOR_VERSION=""
MINOR_VERSION=""
MIDDLE_VERSION="0"
VERSION_GAP="64"

# // Environment files
ENVIRONMENT_SLOTS1="Assets/Meta/Environment/SLOTS1/EnvironmentSettings.asset"
ENVIRONMENT_SLOTS3="Assets/Meta/Environment/SLOTS3/EnvironmentSettings.asset"
ENVIRONMENT_SLOTS4="Assets/Meta/Environment/SLOTS4/EnvironmentSettings.asset"

APPLICATION_SETTINGS="Assets/Meta/Resources/ApplicationSettings.asset"
PRODUCT_SETTINGS="Assets/Meta/Resources/ProductSettings.asset"

function getEnvironmentVersion {
    local witch=$1
    local version=$(grep "clientVersion" $ROOT_PATH/$ENVIRONMENT_SLOTS1 | sed 's/.*: //')
    case "$witch" in
        "CBN")
        local version=$(grep "clientVersion" $ROOT_PATH/$ENVIRONMENT_SLOTS3 | sed 's/.*: //')
        ;;
    esac
    echo $version
}

function getMinorVersion {
    local minorVersion=$(grep "clientVersion" $ROOT_PATH/$ENVIRONMENT_SLOTS1 | sed 's/.*: [0-9]*.[0-9]*.//')
    echo $minorVersion
}

function getMiddleVersion {
    local majorVersion=$(grep "clientVersion" $ROOT_PATH/$ENVIRONMENT_SLOTS1 | sed 's/.*: [0-9]*.//' | sed 's/\.[0-9]*//')
    echo $majorVersion
}

function getMajorVersion {
    local majorVersion=$(grep "clientVersion" $ROOT_PATH/$ENVIRONMENT_SLOTS1 | sed 's/.*: //' | sed 's/\.[0-9]*.[0-9]*//')
    echo $majorVersion
}

ORIGINAL_MAJOR_VERSION=$(getMajorVersion)
ORIGINAL_MIDDLE_VERSION=$(getMiddleVersion)

function getVersion {
    local witch=$1
    local version="$MAJOR_VERSION.$MIDDLE_VERSION.$MINOR_VERSION"
    case "$witch" in
        "CBN")
        version="`expr $MAJOR_VERSION - $VERSION_GAP`.$MIDDLE_VERSION.$MINOR_VERSION"
        ;;
    esac
    echo $version
}

function SetVersion
{
    local cvs_version=$1
    local cbn_version=$2

    sed -i "" "s/clientVersion: [0-9]*.[0-9]*.[0-9]*/clientVersion: $cvs_version/" "$ROOT_PATH/$ENVIRONMENT_SLOTS1"
    sed -i "" "s/productVersion: [0-9]*.[0-9]*.[0-9]*/productVersion: $cvs_version/" "$ROOT_PATH/$ENVIRONMENT_SLOTS1"

    sed -i "" "s/clientVersion: [0-9]*.[0-9]*.[0-9]*/clientVersion: $cvs_version/" "$ROOT_PATH/$ENVIRONMENT_SLOTS3"
    sed -i "" "s/productVersion: [0-9]*.[0-9]*.[0-9]*/productVersion: $cbn_version/" "$ROOT_PATH/$ENVIRONMENT_SLOTS3"

    sed -i "" "s/clientVersion: [0-9]*.[0-9]*.[0-9]*/clientVersion: $cvs_version/" "$ROOT_PATH/$ENVIRONMENT_SLOTS4"
    sed -i "" "s/productVersion: [0-9]*.[0-9]*.[0-9]*/productVersion: $cvs_version/" "$ROOT_PATH/$ENVIRONMENT_SLOTS4"

    sed -i "" "s/clientVersion: [0-9]*.[0-9]*.[0-9]*/clientVersion: $cvs_version/" "$ROOT_PATH/$APPLICATION_SETTINGS"
    sed -i "" "s/productVersion: [0-9]*.[0-9]*.[0-9]*/productVersion: $cvs_version/" "$ROOT_PATH/$PRODUCT_SETTINGS"

    echo "Version up. CVS $cvs_version, CBN $cbn_version."
}

v1=$1
r1=${v1//[0-9]/}
v2=$2
r2=${v2//[0-9]/}
v3=$3
r3=${v3//[0-9]/}

if [ -z "$r1" ] && [ -z "$r2" ] && [ -z "$r3" ] ; then
    ORIGINAL_CVS_VERSION=$(getEnvironmentVersion "CVS")
    ORIGINAL_CBN_VERSION=$(getEnvironmentVersion "CBN")

    if [ -z $1 ]; then
        MAJOR_VERSION=$ORIGINAL_MAJOR_VERSION
    else
        MAJOR_VERSION=$1
    fi

    if [ -z $2 ]; then
        if [ $ORIGINAL_MAJOR_VERSION -eq $MAJOR_VERSION ] ; then
            MIDDLE_VERSION=$ORIGINAL_MIDDLE_VERSION
        else
            MIDDLE_VERSION="0"
        fi
    else
        MIDDLE_VERSION=$2
    fi

    if [ -z $3 ]; then
        if [ $ORIGINAL_MAJOR_VERSION -eq $MAJOR_VERSION ] && [ $ORIGINAL_MIDDLE_VERSION -eq $MIDDLE_VERSION ] ; then
            MINOR_VERSION=`expr $(getMinorVersion) + "1"`
        else
            MINOR_VERSION="0"
        fi
    else
        MINOR_VERSION=$3
    fi

    TARGET_CVS_VERSION=$(getVersion "CVS")
    TARGET_CBN_VERSION=$(getVersion "CBN")

    echo "CVS $ORIGINAL_CVS_VERSION -> $TARGET_CVS_VERSION"
    echo "CVS $ORIGINAL_CBN_VERSION -> $TARGET_CBN_VERSION"
    SetVersion $TARGET_CVS_VERSION $TARGET_CBN_VERSION
else
    echo "enter the number"
fi


