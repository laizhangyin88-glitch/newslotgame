#!/bin/bash

PROJECT_PATH=$(pwd)/../
APP_NAME="Slots1"

# native build target
if [ -z $1 ]; then
    echo "S1 Null"
else
    APP_NAME=$1
fi

#Proejct folder. 
if [ -z $2 ]; then
    echo "S2 Null"
else
    PROJECT_PATH="$2"
fi

TARGET_FOLDER_PATH="iOS/Unity-iPhone/Images.xcassets/AppIcon$APP_NAME.appiconset"
ICON_PATH="$PROJECT_PATH/AppIconMaker/$APP_NAME/IOS"

TARGET_PATH="$PROJECT_PATH/$TARGET_FOLDER_PATH"

echo $PROJECT_PATH
echo $TARGET_PATH
echo $APP_NAME

cp -r "$ICON_PATH/icon.png" "$TARGET_PATH/Icon-72.png"
cp -r "$ICON_PATH/icon.png" "$TARGET_PATH/Icon-76.png"
cp -r "$ICON_PATH/icon.png" "$TARGET_PATH/Icon-120.png"
cp -r "$ICON_PATH/icon.png" "$TARGET_PATH/Icon-144.png"
cp -r "$ICON_PATH/icon.png" "$TARGET_PATH/Icon-152.png"
cp -r "$ICON_PATH/icon.png" "$TARGET_PATH/Icon-167.png"
cp -r "$ICON_PATH/icon.png" "$TARGET_PATH/Icon-180.png"
cp -r "$ICON_PATH/icon.png" "$TARGET_PATH/Icon.png"
cp -r "$ICON_PATH/icon.png" "$TARGET_PATH/Icon@2x.png"
cp -r "$ICON_PATH/icon.png" "$TARGET_PATH/Icon-1024.png"

sips -z 72  72  "$TARGET_PATH/Icon-72.png"
sips -Z 76  "$TARGET_PATH/Icon-76.png"
sips -Z 120 "$TARGET_PATH/Icon-120.png"
sips -Z 144 "$TARGET_PATH/Icon-144.png"
sips -Z 152 "$TARGET_PATH/Icon-152.png"
sips -Z 167 "$TARGET_PATH/Icon-167.png"
sips -Z 180 "$TARGET_PATH/Icon-180.png"
sips -Z 57  "$TARGET_PATH/Icon.png"
sips -Z 114 "$TARGET_PATH/Icon@2x.png"
