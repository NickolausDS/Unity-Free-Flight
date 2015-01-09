#!/bin/bash


COMMAND="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="../Unity Free Flight"
LOG_FILE="build.out"
OPTIONS="-quit -batchmode -logFile $LOG_FILE -projectPath=$PROJECT_PATH"

PACKAGER="./package.sh"

BUILD_PATH="../release/FreeFlight"

BUILD_OSX="-buildOSX64Player $BUILD_PATH"
BUILD_WINDOWS="-buildWindows64Player $BUILD_PATH.exe"
BUILD_LINUX="-buildLinux64Player $BUILD_PATH.x86_64"
BUILD_WEB="-buildWebPlayerStreamed $BUILD_PATH"

#Prefix to Free Flight assets
PFF="Assets/FreeFlight"
#Assets with a space in their file name need to be backslashed
SA=Assets/FreeFlight/Standard\ Assets

#Everything core to running Free Flight
BASIC_ASSETS=("$PFF/Scripts/FreeFlight" "Assets/Editor/FreeFlightEditor.cs" "Assets/Plugins/Pixelplacement")

DEMO_ASSETS=${BASIC_ASSETS[@]}
DEMO_ASSETS+=("$PFF/Models" "$PFF/Prefabs" "$PFF/Scenes" "$PFF/Sounds/Flight")
DEMO_ASSETS+=("$PFF/Sounds/sources.txt" "$PFF/Scripts" "$PFF/Animations" "$PFF/Animators")


touch $LOG_FILE
tail -n 0 -f "$LOG_FILE" & 
tail_pid=$!

$COMMAND $OPTIONS $BUILD_OSX &&
$PACKAGER "osx" &&

$COMMAND $OPTIONS $BUILD_WINDOWS &&
$PACKAGER "windows" &&

$COMMAND $OPTIONS $BUILD_LINUX &&
$PACKAGER "linux" &&

$COMMAND $OPTIONS $BUILD_WEB &&
$PACKAGER "web" &&

$COMMAND $OPTIONS -exportPackage ${BASIC_ASSETS[@]} "${BUILD_PATH}.unitypackage" &&
$PACKAGER "unitypackage" &&

$COMMAND $OPTIONS -exportPackage ${DEMO_ASSETS[@]} "$SA" "${BUILD_PATH}Demo.unitypackage" &&
$PACKAGER "unitydemopackage" 

if [ "$?" -eq "0" ]
then
	echo "SUCCESS! All builds pass!!!"
else
	echo "FAIL! Stopping build process..."
fi

kill $tail_pid &> /dev/null
wait $tail_pid
rm $LOG_FILE

