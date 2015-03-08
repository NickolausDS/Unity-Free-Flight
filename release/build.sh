#!/bin/bash


COMMAND="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
PROJECT_NAME="FreeFlight"
PROJECT_PATH="../Unity Free Flight"
LOG_FILE="build.out"
OPTIONS="-quit -batchmode -logFile $LOG_FILE -projectPath=$PROJECT_PATH"

PACKAGER="./package.sh"

BUILD_PATH="../release/FreeFlight"

BUILD_OSX="-buildOSX64Player $BUILD_PATH.app"
BUILD_OSX_ICON="${PROJECT_PATH}/Assets/${PROJECT_NAME}Demo/release/PlayerIcon.icns"
BUILD_WINDOWS="-buildWindows64Player $BUILD_PATH.exe"
BUILD_LINUX="-buildLinux64Player $BUILD_PATH.x86_64"
BUILD_WEB="-buildWebPlayerStreamed $BUILD_PATH"

#Assets with a space in their file name need to be backslashed
SA=Assets/FreeFlightDemo/Standard\ Assets

#Everything core to running Free Flight
BASIC_ASSETS=("Assets/FreeFlight" "Assets/Editor/FreeFlightEditor.cs" "Assets/Plugins/Pixelplacement")
DEMO_ASSETS=("${BASIC_ASSETS[@]} Assets/FreeFlightDemo")


touch $LOG_FILE
tail -n 0 -f "$LOG_FILE" & 
tail_pid=$!

$COMMAND $OPTIONS $BUILD_OSX &&
cp -v "$BUILD_OSX_ICON" "$PROJECT_NAME.app/Contents/Resources/" &&
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

