#!/bin/bash

# 서로 동일한 폴더 구조를 복사하여 guid만 달라졌을때 돌리는 스크립트.
# 예) Environment/Slots1/test.meta -> Environment/Slots3/test.meta의 guid를 변경된 guid로 마이그레이션함.

function RecurciveGuidMigration
{
    local originalPath=$1
    local sourcePath=$2
    local targetPath=$3

    for fileName in $(ls "$targetPath" | tr ' ' '$');
    do
        let FILECOUNT=FILECOUNT+1

        local pathName=$(echo $fileName | tr '$' '\ ')

        if [ -d "$targetPath/$pathName" ]; then
            RecurciveGuidMigration $originalPath "$sourcePath/$pathName" "$targetPath/$pathName"
        else
            if [[ "$fileName" == *.meta ]]; then
                sourFileName=$(echo $sourcePath/$fileName | tr '$' ' ')
                targetFileName=$(echo $targetPath/$fileName | tr '$' ' ')

                if [ -f "$sourFileName" ] && [ -f "$targetFileName" ]; then
                    local sourceGuid=$(GetGUID $sourcePath $fileName)
                    local targetGuid=$(GetGUID $targetPath $fileName)

                    if [ ! -z $sourceGuid ] && [ ! -z $targetGuid ]; then
                        echo "Begin Searching files($sourFileName $sourceGuid)"
                        let TOTAL_SEARCH_COUNT=0
                        let REPLACE=0
                        let IgnoreFiles=0
                        let IgnoreFolders=0
                        ReplaceGUID $originalPath $sourceGuid $targetGuid
                        echo "Search $TOTAL_SEARCH_COUNT(Ignore $IgnoreFolders Folders, $IgnoreFiles Files) files matches $REPLACE files"
                        echo `date +%H:%M:%S`
                        echo ""
                    fi
                else
                    echo "$FILECOUNT) $path Not Exist"
                fi
            fi
        fi
    done
}

function RecurciveRename
{
    local targetPath=$1
    local appName=$2
    local targetAppName=$3

    for fileName in $(ls $targetPath | tr ' ' '$');
    do
        if [ -d "$targetPath/$fileName" ]; then
            RecurciveRename "$targetPath/$fileName" $appName $targetAppName
        else
            if [[ "$fileName" == *"$appName"* ]]; then
                sourFileName=$(echo $fileName | tr '$' ' ')
                newFileName=$(echo $sourFileName | tr $appName $targetAppName)
                echo $newFileName
                mv "$targetPath/$sourFileName" "$targetPath/$newFileName"
            fi
        fi
    done
}

function GetGUID
{
    local path=$1
    local targetFile=$2
    if [[ "$targetFile" == *.meta ]]; then
        local fullPath=$(echo $path/$targetFile | tr '$' ' ')
        # echo $fullPath
        local guid=$(grep "guid" "$fullPath" | sed 's/.*: //')
        echo $guid
    fi
}

function ReplaceGUID
{
    local targetPath=$1
    local sourceGUID=$2
    local targetGUID=$3

    for fileName in $(ls "$targetPath" | tr ' ' '$');
    do
        let TOTAL_SEARCH_COUNT=TOTAL_SEARCH_COUNT+1
        #  
        if [[ ! "$fileName" == *.meta ]] && [[ ! "$fileName" == *.png ]]&& [[ ! "$fileName" == *.wav ]]; then
            local pathName=$(echo $fileName | tr '$' '\ ')
            if [ -d "$targetPath/$pathName" ]; then
                let IgnoreFolders=IgnoreFolders+1
                ReplaceGUID "$targetPath/$pathName" $sourceGUID $targetGUID
            else
                local targetFileName=$(echo $targetPath/$fileName | tr '$' ' ')
                if [[ ! -z `grep "$sourceGUID" "$targetFileName"` ]]; then
                    let REPLACE=REPLACE+1
                    sed -i '' "s/$sourceGUID/$targetGUID/g" "$targetFileName"
                    echo "$REPLACE) $fileName Replace $sourceGUID -> $targetGUID"
                fi
            fi
        else
            let REPLACE=REPLACE+1
            let IgnoreFiles=IgnoreFiles+1
        fi
    done
}

ROOT_PATH=$(pwd)
SOURCE_PATH=""
TARGET_PATH=""
APP_NAME="SLOTS1"
TARGET_APP_NAME="SLOTS1"

if [ -z $1 ]; then
    echo "S1 Null"
else
    SOURCE_PATH=$1
fi

if [ -z $2 ]; then
    echo "S2 Null"
else
    TARGET_PATH=$2
fi

if [ -z $3 ]; then
    echo "S3 Null"
else
    APP_NAME=$3
fi

if [ -z $4 ]; then
    echo "S4 Null"
else
    TARGET_APP_NAME=$4
fi

START_TIME=`date +%H:%M:%S`

echo $SOURCE_PATH
echo $TARGET_PATH
echo $APP_NAME
echo $TARGET_APP_NAME
echo $ROOT_PATH
echo "================================================================="
RecurciveGuidMigration "$ROOT_PATH/$TARGET_PATH" "$ROOT_PATH/$SOURCE_PATH" "$ROOT_PATH/$TARGET_PATH"
RecurciveRename "$ROOT_PATH/$TARGET_PATH" $APP_NAME $TARGET_APP_NAME
echo "Start $START_TIME"
END_TIME=`date +%H:%M:%S`
echo "End $END_TIME"
echo "================================================================="
