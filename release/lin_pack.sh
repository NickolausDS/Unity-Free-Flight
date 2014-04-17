
VERSION=`cat version.txt | head -n 1`
NAME="FreeFlight"
PLATFORM="Linux"
DIR="$NAME$PLATFORM$VERSION"

mkdir "$DIR"
mv FreeFlight.x86 FreeFlight_Data/ "$DIR"
tar -czf "$DIR.tar.gz" "$DIR"
