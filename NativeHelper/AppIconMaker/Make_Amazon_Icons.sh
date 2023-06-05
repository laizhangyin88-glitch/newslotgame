#!/bin/bash

PROJECT_PATH=$(pwd)/../
TARGET_PROJECT_PATH=$(pwd)/../Amazon
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

#Target Project folder. 
if [ -z $3 ]; then
    echo "S3 Null"
else
    TARGET_PROJECT_PATH="$3"
fi

TARGET_FOLDER_PATH="app/src/$APP_NAME/res"
ICON_PATH="$PROJECT_PATH/AppIconMaker/$APP_NAME/Amazon"
TARGET_PATH="$TARGET_PROJECT_PATH/$TARGET_FOLDER_PATH"

echo $PROJECT_PATH
echo $TARGET_PATH
echo $APP_NAME

cp -r "$ICON_PATH"/icon.png "$TARGET_PATH"/drawable-mdpi/ic_launcher.png
cp -r "$ICON_PATH"/icon.png "$TARGET_PATH"/drawable-mdpi/notification_icon.png

cp -r "$ICON_PATH"/icon.png "$TARGET_PATH"/drawable-hdpi/ic_launcher.png
cp -r "$ICON_PATH"/icon.png "$TARGET_PATH"/drawable-hdpi/notification_icon.png

cp -r "$ICON_PATH"/icon.png "$TARGET_PATH"/drawable-xhdpi/ic_launcher.png
cp -r "$ICON_PATH"/icon.png "$TARGET_PATH"/drawable-xhdpi/notification_icon.png

cp -r "$ICON_PATH"/icon.png "$TARGET_PATH"/drawable-xxhdpi/ic_launcher.png
cp -r "$ICON_PATH"/icon.png "$TARGET_PATH"/drawable-xxhdpi/notification_icon.png

cp -r "$ICON_PATH"/icon.png "$TARGET_PATH"/drawable-xxxhdpi/ic_launcher.png
cp -r "$ICON_PATH"/icon.png "$TARGET_PATH"/drawable-xxxhdpi/notification_icon.png

sips -Z 48  "$TARGET_PATH"/drawable-mdpi/ic_launcher.png
sips -Z 24  "$TARGET_PATH"/drawable-mdpi/notification_icon.png

sips -Z 72  "$TARGET_PATH"/drawable-hdpi/ic_launcher.png
sips -Z 36  "$TARGET_PATH"/drawable-hdpi/notification_icon.png

sips -Z 96  "$TARGET_PATH"/drawable-xhdpi/ic_launcher.png
sips -Z 48  "$TARGET_PATH"/drawable-xhdpi/notification_icon.png

sips -Z 144 "$TARGET_PATH"/drawable-xxhdpi/ic_launcher.png
sips -Z 72  "$TARGET_PATH"/drawable-xxhdpi/notification_icon.png

sips -Z 192 "$TARGET_PATH"/drawable-xxxhdpi/ic_launcher.png
sips -Z 96  "$TARGET_PATH"/drawable-xxxhdpi/notification_icon.png
