#!/bin/bash
ROOT_PATH=$(pwd)
COPY_IMAGE_COUNT=0

SLOT_IMAGE_ROOT_PATH="$ROOT_PATH/Assets/Contents/Contents Group 1/_Slot Images/Slot Image/Textures"
DEFULT_PATH="$SLOT_IMAGE_ROOT_PATH/Textures Default"
TYPE_A_PATH="$SLOT_IMAGE_ROOT_PATH/Textures Type A"
TYPE_B_PATH="$SLOT_IMAGE_ROOT_PATH/Textures Type B"

function getContentsVersion {
    local CONTENTS_INFO_PATH="$ROOT_PATH/Assets/Contents/Common/contentsinfo.json"
    # echo $CONTENTS_INFO_PATH

    local contentsVersion=$(grep "version" $CONTENTS_INFO_PATH | sed 's/.*": "//' | sed 's/",//')
    echo $contentsVersion
}

# Get contents version
CONTENTS_VERSION=$(getContentsVersion)
echo $CONTENTS_VERSION

# make image folder. Slot Images {version} 
TARGET_FOLDER="$ROOT_PATH/Slot_Images($CONTENTS_VERSION)"
if [ ! -d $TARGET_FOLDER ]; then
    mkdir $TARGET_FOLDER
    echo "Maked folder : $TARGET_FOLDER"
fi

function copyFile {
    local targetFile=$1
    local count=0

    if [ -e "$SLOT_IMAGE_ROOT_PATH/$targetFile" ]; then
        cp -r "$SLOT_IMAGE_ROOT_PATH/$targetFile" "$TARGET_FOLDER/$targetFile"
        count=$((count+1))
        showLog "WARNING_FILE_PATH" "$targetFile" "$TARGET_FOLDER/$targetFile"
    fi

    if [ -e "$DEFULT_PATH/$targetFile" ]; then
        cp -r "$DEFULT_PATH/$targetFile" "$TARGET_FOLDER/$targetFile"
        count=$((count+1))
        showLog "SUCCESS DEFAULT" "$targetFile" "$TARGET_FOLDER/$targetFile"
    fi

    if [ -e "$TYPE_A_PATH/$targetFile" ]; then
        if [ ! -d "$TARGET_FOLDER/Type A" ]; then
            mkdir "$TARGET_FOLDER/Type A"
        fi
        cp -r "$TYPE_A_PATH/$targetFile" "$TARGET_FOLDER/Type A/$targetFile"
        count=$((count+1))
        showLog "SUCCESS TYPE A" "$targetFile" "$TARGET_FOLDER/$targetFile"
    fi
    
    if [ -e "$TYPE_B_PATH/$targetFile" ]; then
        if [ ! -d "$TARGET_FOLDER/Type B" ]; then
            mkdir "$TARGET_FOLDER/Type B"
        fi
        cp -r "$TYPE_B_PATH/$targetFile" "$TARGET_FOLDER/Type B/$targetFile"
        count=$((count+1))
        showLog "SUCCESS TYPE B" "$targetFile" "$TARGET_FOLDER/$targetFile"
    fi

    if (("$count" <= "0")); then
        showLog "NOT_EXIST_FILE" "$targetFile" ""
    fi
}

function showLog {
    local result=$1
    local fileName=$2
    local checkFilePath=$3
    local fileStatus=""

    if [ -e "$checkFilePath" ]; then
        fileSize=$(wc -c "$checkFilePath" | awk '{print $1}')
        if [ $fileSize -gt 1024000 ];then
          fileStatus="\033[0;31m($fileSize) size is greater than 1MB\033[0m"
        fi
    fi

    if [[ "$result" == *SUCCESS* ]]; then
        COPY_IMAGE_COUNT=$((COPY_IMAGE_COUNT+1))
        echo -e "\033[0;32m($result)\033[0m$fileName$fileStatus"
    elif [[ "$result" == *WARNING_FILE_PATH* ]]; then
        COPY_IMAGE_COUNT=$((COPY_IMAGE_COUNT+1))
        echo -e "\033[0;33m(Warning)\033[0m$fileName check to file path$fileStatus"
    else
        echo -e "\033[0;31m(Error)\033[0m$fileName not exist$fileStatus"
    fi
}

function copyImages {
    COPY_IMAGE_COUNT=0
    for i in $@
    do
        local gameTitle=`echo $i | tr '[:lower:]' '[:upper:]'`

        local smallImageFileName="Slot Image Small $gameTitle.png"
        copyFile "$smallImageFileName"

        local bigImageFileName="Slot Image Big $gameTitle.png"
        copyFile "$bigImageFileName"

        # showLog $result "$bigImageFileName"
    done
    echo "Image count :$COPY_IMAGE_COUNT"
}

##################################################################
echo "params = $@"

# copy image assets
copyImages $@

# print result text
if (("$COPY_IMAGE_COUNT" <= "0")); then
    echo "not found images"
else
    if (("$COPY_IMAGE_COUNT" == "1")); then
        echo "$COPY_IMAGE_COUNT image copy"
    else
        echo "$COPY_IMAGE_COUNT images copy"
    fi
fi
##################################################################
