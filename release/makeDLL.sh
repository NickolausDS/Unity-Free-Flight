#!/bin/bash
#
#Create a DLL using the current version of the MonoDevelop compiler and Unity
#Libraries. 
#

#Mono Compiler
COMPILER="/Applications/Unity/MonoDevelop.app/Contents/Frameworks/Mono.framework/Versions/Current/bin/mcs"

#Unity Libraries to use. 
LIBRARIES="/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll,\
/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEditor.dll,\
/Applications/Unity/Unity.app/Contents/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll"

#Tell monodevelop to create a DLL
TARGET_TYPE="library"

#Source path to code project
SPATH="../FreeFlight/Assets/FreeFlight/"
#Sources to include. The first file path given becomes the name of the 
#Output DLL, and it's recommended to rename the file to it's intended value
#by calling mv below
SOURCES="$SPATH/Scripts/FreeFlight.cs $SPATH/Scripts/FreeFlightPhysics/ \
$SPATH/Editor $SPATH/Scripts/Mechanics/"

#The desired name to output
OUTPUT="${SPATH}FreeFlight.dll"

#Build the list of sources
SRC_LIST=$(find ${SOURCES} -name *.cs)

${COMPILER} -r:${LIBRARIES} -target:${TARGET_TYPE} ${SRC_LIST[@]}

#MCS creates the DLL with the path and name after the first file given to it.
#Move it to a desired position. 
mv -v "${SPATH}/Scripts/FreeFlight.dll" "${OUTPUT}"
