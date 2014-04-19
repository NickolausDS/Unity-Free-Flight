#!/bin/bash

VERSION=`cat version.txt | head -n 1`
NAME="FreeFlight"

#The base name for each package, plus the ".exe", ".app", or ".x86"
PACKAGENAME="$NAME$VERSION"
#the directory where we will store all of the packages
PACKAGEDEST="$VERSION"
LINUX_DEP=(${NAME}.x86 ${NAME}_Data)
MAC_DEP=(${NAME}.app)
WINDOWS_DEP=(${NAME}.exe ${NAME}_Data)

function sanity() {
	if [ `uname` != "Darwin" ] 
	then
		echo ""
		echo "WARNING: This packager was designed for Mac OSX only, you may get"
		echo "unpredictable results on your current platform."
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
	if [ -z "${1}" ]
	then
		echo "Skipped."
	elif [[ $1 -eq 0 ]] 
	then 
		echo  "Success!"
	else
		echo  "Failure!"
	fi
}

clear
printf "Welcome to the Unity Packager!\n\n"
printf "Make sure you build everything with the name: $NAME\n"
printf "Otherwise, the packager will not recognize it.\n\n" 
printf "$NAME Version: $VERSION\n"
printf "To be Packaged:\n" 
printf "\tLinux:   `has_dep ${LINUX_DEP[@]}`\n"
printf "\tMac:     `has_dep ${MAC_DEP[@]}`\n"
printf "\tWindows: `has_dep ${WINDOWS_DEP[@]}`\n"
printf "`sanity`"
printf "\n\n`check_packages_exist`\n\n"
printf "Continue? (y/n) "
read answer

if [ "$answer" == "y" ]
then
	#Make the destination if it doesn't exist
	mkdir -v "$PACKAGEDEST" 2> /dev/null

	if has_dep ${LINUX_DEP[@]} &> /dev/null
	then
		#Get rid of the old one, if it exists
		rm "$PACKAGEDEST/$PACKAGENAME.tar.gz" 2> /dev/null
		echo "Packaging for Linux..." &&
		mkdir $PACKAGENAME && 
		cp -r ${LINUX_DEP[@]} $PACKAGENAME/ && 
		tar -vczf "$PACKAGENAME.tar.gz" $PACKAGENAME &&
		mv -v "$PACKAGENAME.tar.gz" "$PACKAGEDEST" &&
		echo "Cleaning up..." &&
		rm -rf $PACKAGENAME
		LIN_RET=$?
	fi
	if has_dep ${WINDOWS_DEP[@]} &> /dev/null
	then
		#Get rid of the old one, if it exists
		rm "$PACKAGEDEST/$PACKAGENAME.zip" 2> /dev/null
		echo "Packaging for Windows..." &&
		mkdir $PACKAGENAME && 
		cp -r ${WINDOWS_DEP[@]} $PACKAGENAME/ && 
		zip -rv "$PACKAGENAME.zip" $PACKAGENAME &&
		mv -v "$PACKAGENAME.zip" "$PACKAGEDEST" &&
		echo "Cleaning up..." &&
		rm -rf $PACKAGENAME
		WIN_RET=$?
	fi
	if has_dep ${MAC_DEP[@]} &> /dev/null
	then
		rm "$PACKAGEDEST/$PACKAGENAME.dmg" 2> /dev/null
		echo "Packaging for Mac..." &&
		bash make_dmg.sh &&
		mv -v "$PACKAGENAME.dmg" "$PACKAGEDEST" &&
		echo "Cleaning up..." &&
		rm -rf $PACKAGENAME
		MAC_RET=$?	
	fi

	printf "\n\n"
	printf "Linux:   `print_status $LIN_RET`\n"
	printf "Windows: `print_status $WIN_RET`\n"
	printf "Mac:     `print_status $MAC_RET`\n"

	if [ "$WIN_RE" != "1" -a "$MAC_RET" != "1" -a "$LIN_RET" != "1" ] 
	then
		printf "\nFinishing cleanup...\n"
		rm -rf ${MAC_DEP[@]} ${LINUX_DEP[@]} ${WINDOWS_DEP[@]}
	else
		printf "\nAborting cleanup due to errors.\n"
	fi
else
	echo "Aborted."
fi

