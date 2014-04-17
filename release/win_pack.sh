
VERSION=`cat version.txt | head -n 1`
NAME="FreeFlight"
DIR="$NAME$VERSION"

mkdir "$DIR"
mv FreeFlight.exe FreeFlight_Data/ UnityPlayer_Symbols.pdb "$DIR"
zip -r "$DIR.zip" "$DIR"
