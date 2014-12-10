#!/bin/bash

VERSION=`cat version.txt | head -n 1`
NAME="FreeFlight"

#The base name for each package, plus the ".exe", ".app", or ".x86"
PACKAGENAME="$NAME$VERSION"
#the directory where we will store all of the packages
PACKAGEDEST="$VERSION"
LINUX_DEP=(${NAME}.x86_64 ${NAME}_Data)
MAC_DEP=(${NAME}.app)
WINDOWS_DEP=(${NAME}.exe ${NAME}_Data)
WEB_DEP=(${NAME})
UNITYPACKAGE_DEP=(${NAME}.unitypackage)


function sanity() {
	if [ `uname` != "Darwin" ] 
	then
		echo ""
		echo "WARNING: This packager was designed for Mac OSX only, you may get"
		echo "unpredictable results on your current platform."
		echo ""
	fi
	local ret1=`has_dep ${LINUX_DEP[@]}`
	local ret2=`has_dep ${WINDOWS_DEP[@]}`
	if [ "$ret1" == "Yes" -a "$ret2" == "Yes" ]
	then
		echo ""
		echo "ERROR: Both windows and linux packages detected. You can't build"
		echo "both at the same time! Unity's latest build has overwritten the"
		echo "other's files, rendering one build useless!" 
		echo "..."
		echo "YOU SHOULD NOT CONTINUE!!!"
		echo "..."
		echo "Please redo the builds for both windows and linux to ensure there isn't"
		echo "corruption. Anything else not using '${NAME}_Data' should be fine."
		echo ""
	fi
}

#Function is actually multipurpose. We can check the return value
#if we want to call it programmatically, or print the output for a 
#nice string representation.
function has_dep() {
	stat $@ &> /dev/null
	local ret=$?
	if [[ ret -eq 0 ]] 
	then 
		echo "Yes"
	else
		echo "No"
	fi
	return $ret
}

function check_packages_exist() {
	stat "$PACKAGEDEST" &> /dev/null &&
	echo "WARNING: New packages will overwrite the current ones in "$PACKAGEDEST"/!" &&
	echo "Add entry to the top of Version.txt to keep $PACKAGEDEST packages."
}

function print_status() {
	case "$1" in 
	0) echo "Success!"
	;;
	1) echo "Skipped."
	;;
	*) echo "Failed!"
	;;
	esac
	return $1
}

function package_linux() {

	if has_dep ${LINUX_DEP[@]} &> /dev/null
	then
		#Get rid of the old one, if it exists
		rm "$PACKAGEDEST/$PACKAGENAME.tar.gz" 2> /dev/null
		echo "Packaging for Linux..." &&
		mkdir $PACKAGENAME && 
		cp -r ${LINUX_DEP[@]} $PACKAGENAME/ && 
		tar -vczf "$PACKAGENAME.tar.gz" $PACKAGENAME &&
		mv -vf "$PACKAGENAME.tar.gz" "$PACKAGEDEST" &&
		echo "Cleaning up..." &&
		rm -rf $PACKAGENAME
		return 0
		return -1
	fi
	echo "Build not detected, skipping Linux..."
	return 1
}

function package_osx() {
	if has_dep ${MAC_DEP[@]} &> /dev/null
	then
		rm "$PACKAGEDEST/$PACKAGENAME.dmg" 2> /dev/null
		echo "Packaging for Mac..." &&
		bash make_dmg.sh &&
		mv -vf "$PACKAGENAME.dmg" "$PACKAGEDEST" &&
		echo "Cleaning up..." &&
		rm -rf $PACKAGENAME &&
		return 0
		return -1

	fi
	echo "Build not detected, skipping OSX..."
	return 1
}

function package_windows() {

	if has_dep ${WINDOWS_DEP[@]} &> /dev/null
	then
		#Get rid of the old one, if it exists
		rm "$PACKAGEDEST/$PACKAGENAME.zip" 2> /dev/null
		echo "Packaging for Windows..." &&
		mkdir $PACKAGENAME && 
		cp -r ${WINDOWS_DEP[@]} $PACKAGENAME/ && 
		zip -rv "$PACKAGENAME.zip" $PACKAGENAME &&
		mv -vf "$PACKAGENAME.zip" "$PACKAGEDEST" &&
		echo "Cleaning up..." &&
		rm -rf $PACKAGENAME &&
		return 0
		return -1
	fi
	echo "Build not detected, skipping Windows..."
	return 1

}

function package_web() {
	if has_dep ${WEB_DEP[@]} &> /dev/null
	then
		WEBNAME="${NAME}Web${VERSION}"
		rm -rf "$PACKAGEDEST/$WEBNAME" 2> /dev/null
		echo "Moving Web Files..." &&
		mv -vf "$NAME" "$PACKAGEDEST/$WEBNAME" &&
		return 0
		return -1
	fi
	echo "Build not detected, skipping Web..."
	return 1
}

function package_unitypackage() {
	if has_dep ${UNITYPACKAGE_DEP[@]} &> /dev/null
	then
		rm "$PACKAGEDEST/$PACKAGENAME.unitypackage" 2> /dev/null
		echo "Moving Unity Package..." &&
		mv -v "$NAME.unitypackage" "$PACKAGEDEST/$PACKAGENAME.unitypackage" &&
		return 0
		return -1
	fi
	echo "Build not detected, skipping Unity Package..."
	return 1

}

function cleanup_all() {
	printf "\nFinishing cleanup...\n"
	rm -rf ${MAC_DEP[@]} ${LINUX_DEP[@]} ${WINDOWS_DEP[@]} ${WEB_DEP[@]} ${UNITYPACKAGE_DEP[@]}
}

function user_interface() {

	clear
	printf "Welcome to the Unity Packager!\n\n"
	printf "Make sure you build everything with the name: $NAME\n"
	printf "Otherwise, the packager will not recognize it.\n\n" 
	printf "$NAME Version: $VERSION\n"
	printf "To be Packaged:\n" 
	printf "\tLinux:   `has_dep ${LINUX_DEP[@]}`\n"
	printf "\tMac:     `has_dep ${MAC_DEP[@]}`\n"
	printf "\tWindows: `has_dep ${WINDOWS_DEP[@]}`\n"
	printf "\tWeb:     `has_dep ${WEB_DEP[@]}`\n"
	printf "\tUnity:   `has_dep ${UNITYPACKAGE_DEP[@]}`\n"
	printf "`sanity`"
	printf "\n\n`check_packages_exist`\n\n"
	printf "Continue? (y/n) "
	read answer

	if [ "$answer" == "y" ]
	then

		package_osx
		MAC_RET=$?
		package_windows
		WIN_RET=$?
		package_linux
		LIN_RET=$?
		package_web
		WEB_RET=$?
		package_unitypackage
		UNITYPACKAGE_RET=$?

		printf "\n\n"
		printf "Linux:   `print_status $LIN_RET`\n"
		printf "Windows: `print_status $WIN_RET`\n"
		printf "Mac:     `print_status $MAC_RET`\n"
		printf "Web:     `print_status $WEB_RET`\n"
		printf "Unity:   `print_status $UNITYPACKAGE_RET`\n"

		fail="0"
		for each in $LIN_RET $WIN_RET $MAC_RET $WEB_RET $UNITYPACKAGE_RET
		do
			if [ "$each" -gt "1" ]
			then
				fail="1"
			fi
		done

		if [ "$fail" -eq "0" ] 
		then
			cleanup_all
		else
			printf "\nAborting cleanup due to errors.\n"
		fi
	else
		echo "Aborted."
	fi
}

#Make the destination if it doesn't exist
mkdir -v "$PACKAGEDEST" 2> /dev/null

case "$1" in

windows)
	package_windows && cleanup_all
	exit $?
	;;
osx)  
	package_osx && cleanup_all
	exit $?
	;;
linux)
	package_linux && cleanup_all
	exit $?
	;;
web)
	package_web && cleanup_all
	exit $?
	;;
unitypackage)
	package_unitypackage && cleanup_all
	exit $?
	;;
"") user_interface
	;;
*) echo "Usage: $0 [windows, osx, linux, web, unitypackage, *nothing for interactive*]"
   ;;
esac

