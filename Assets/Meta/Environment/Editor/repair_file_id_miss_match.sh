#!/bin/bash
ROOT_PATH=$(pwd)
ORIGINAL_FILE_ID=""
TARGET_PATH=""
CHANGE_FILE_ID=""

if [ -z $1 ]; then
    echo "S1 Null"
else
    TARGET_PATH=$1
fi

if [ -z $2 ]; then
    echo "S2 Null"
else
    ORIGINAL_FILE_ID=$2
fi

if [ -z $3 ]; then
    echo "S3 Null"
else
    CHANGE_FILE_ID=$3
fi

echo "================================================================="
echo $ROOT_PATH
echo "-"
echo $TARGET_PATH
echo "-"
echo $ORIGINAL_FILE_ID
echo "-"
echo $CHANGE_FILE_ID
echo "s/$ORIGINAL_FILE_ID/$CHANGE_FILE_ID/" "$ROOT_PATH/$TARGET_PATH"
sed -i "" "s/$ORIGINAL_FILE_ID/$CHANGE_FILE_ID/" "$ROOT_PATH/$TARGET_PATH"
echo "================================================================="
