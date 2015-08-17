#!/bin/bash

VERSION_SUFFIX="$1"

COMMAND="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="../FreeFlight"
LOG_FILE="build.out"
OPTIONS="-quit -batchmode -logFile $LOG_FILE -projectPath=$PROJECT_PATH"

DLLBUILDER="./makeDLL.sh"
DLLPATH="Assets/FreeFlight/FreeFlight.dll"

PACKAGER="./package.sh"

BUILD_PATH="../release/FreeFlight"

BUILD_OSX="-buildOSX64Player $BUILD_PATH"
BUILD_WINDOWS="-force-d3d11 -buildWindows64Player $BUILD_PATH.exe"
BUILD_LINUX="-buildLinux64Player $BUILD_PATH.x86_64"
BUILD_WEB="-buildWebPlayerStreamed $BUILD_PATH"

#Assets with a space in their file name need to be backslashed
SA=Assets/FreeFlightDemo/Standard\ Assets

#Everything core to running Free Flight
BASIC_ASSETS=("Assets/FreeFlight")
DEMO_ASSETS=("${BASIC_ASSETS[@]} Assets/FreeFlightDemo")

trap interrupt INT

function interrupt() {
	echo "Caught Ctrl^C! Stopping!"
	cleanup
}

function cleanup() {
	kill $tail_pid &> /dev/null
	wait $tail_pid &> /dev/null
	rm -vf $LOG_FILE
	rm -vf $VERSION_FILE
	rm -vf "${PROJECT_PATH}/${DLLPATH}"
	rm -vf "${PROJECT_PATH}/${DLLPATH}.meta"
	rm -vf  "*.pdb"
}

touch $LOG_FILE
tail -n 0 -f "$LOG_FILE" & 
tail_pid=$!


VERSION_FILE="$(dirname $BUILD_PATH)/version.txt"
#Get version number
#Build target needs to be set to a platform which can write to a file, or else this will fail.
$COMMAND $OPTIONS -buildTarget win64 -executemethod Build.writeVersion $VERSION_FILE &&

#Checks whether the version actually got produced. Sometimes this will fail if Unity couldn't
#Switch the -buildTarget correctly. 
test $(cat $VERSION_FILE) &&
#Add the version suffix if there was one
echo "$(cat $VERSION_FILE)$VERSION_SUFFIX" > "$VERSION_FILE" &&

$COMMAND $OPTIONS $BUILD_OSX &&
$PACKAGER "osx" &&

$COMMAND $OPTIONS $BUILD_WINDOWS &&
$PACKAGER "windows" &&

$COMMAND $OPTIONS $BUILD_LINUX &&
$PACKAGER "linux" &&

$COMMAND $OPTIONS $BUILD_WEB &&
$PACKAGER "web" &&

$DLLBUILDER &&
mv $PROJECT_PATH/Assets/FreeFlight/Scripts $PROJECT_PATH/Assets/FreeFlight/.Scripts &&
mv $PROJECT_PATH/Assets/FreeFlight/Editor $PROJECT_PATH/Assets/FreeFlight/.Editor &&
mv $PROJECT_PATH/Assets/FreeFlight/Scripts.meta $PROJECT_PATH/Assets/FreeFlight/.Scripts.meta &&
mv $PROJECT_PATH/Assets/FreeFlight/Editor.meta $PROJECT_PATH/Assets/FreeFlight/.Editor.meta &&

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

#If export builds were done, we need to clean them up.
mv $PROJECT_PATH/Assets/FreeFlight/.Scripts $PROJECT_PATH/Assets/FreeFlight/Scripts &> /dev/null
mv $PROJECT_PATH/Assets/FreeFlight/.Editor $PROJECT_PATH/Assets/FreeFlight/Editor &> /dev/null
mv $PROJECT_PATH/Assets/FreeFlight/.Scripts.meta $PROJECT_PATH/Assets/FreeFlight/Scripts.meta &> /dev/null
mv $PROJECT_PATH/Assets/FreeFlight/.Editor.meta $PROJECT_PATH/Assets/FreeFlight/Editor.meta &> /dev/null

cleanup
